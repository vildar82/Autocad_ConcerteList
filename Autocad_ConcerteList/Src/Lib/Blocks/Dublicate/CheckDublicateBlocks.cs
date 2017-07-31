﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autocad_ConcerteList.Errors;
using Autocad_ConcerteList.Lib.Blocks.Dublicate.Tree;
using Autocad_ConcerteList.Log;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Autocad_ConcerteList.Lib.Blocks.Dublicate
{
    /// <summary>
    /// Проверка наложения блоков в пространстве модели
    /// Inspector очищается до и после!
    /// </summary>
    public static class CheckDublicateBlocks
    {
        public static Tolerance Tolerance { get; set; } = new Tolerance(0.2, 10);
        public static int DEPTH = 5;        
        private static int curDepth;
        private static HashSet<ObjectId> attemptedblocks;
        private static HashSet<string> _ignoreBlocks;
        private static List<BlockRefDublicateInfo> AllDublicBlRefInfos;
        private static Dictionary<string, Dictionary<PointTree, List<BlockRefDublicateInfo>>> dictBlRefInfos;
        private static List<IError> _errors;

        public static void Check()
        {
            Check(null, null);            
        }

        public static void Check(HashSet<string> ignoreBlocks)
        {
            Check(null, ignoreBlocks);
        }

        public static void Check(IEnumerable idsBlRefs)
        {
            Check(idsBlRefs, null);
        }        

        public static void Check(IEnumerable idsBlRefs, HashSet<string> ignoreBlocks)
        {
            curDepth = 0;
            _ignoreBlocks = ignoreBlocks;
            var db = HostApplicationServices.WorkingDatabase;
            _errors = new List<IError>();
            attemptedblocks = new HashSet<ObjectId>();
            AllDublicBlRefInfos = new List<BlockRefDublicateInfo>();
            dictBlRefInfos = new Dictionary<string, Dictionary<PointTree, List<BlockRefDublicateInfo>>>();
            try
            {
                using (var t = db.TransactionManager.StartTransaction())
                {                    

                    if (idsBlRefs == null)
                    {
                        var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
                        idsBlRefs = ms;
                    }
                    GetDublicateBlocks(idsBlRefs, Matrix3d.Identity, 0);
                    t.Commit();
                }

                // дублирующиеся блоки
                AllDublicBlRefInfos = dictBlRefInfos.SelectMany(s => s.Value.Values).Where(w => w.Count > 1)
                                        .SelectMany(s => s.GroupBy(g => g).Where(w => w.Skip(1).Any()))
                                        .Select(s =>
                                           {
                                               var bi = s.First();
                                               bi.CountDublic = s.Count();
                                               bi.Dublicates = s.Skip(1).ToList();
                                               return bi;
                                           }).ToList();

                // Добавление дубликатов в результирующий список
                //AddTransformedToModelDublic(dublicBlRefInfos);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"CheckDublicateBlocks - {db.Filename}. {ex.StackTrace}");
                return;
            }

            if (AllDublicBlRefInfos.Count == 0)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nДубликаты блоков не найдены.");
            }
            else
            {
                foreach (var dublBlRefInfo in AllDublicBlRefInfos)
                {
                    var err = new Error($"Дублирование блоков '{dublBlRefInfo.Name}' - {dublBlRefInfo.CountDublic} шт. в точке {dublBlRefInfo.Position.ToString()}",
                       dublBlRefInfo.IdBlRef, dublBlRefInfo.TransformToModel, System.Drawing.SystemIcons.Error);
                    err.Tag = dublBlRefInfo;
                    _errors.Add(err);
                }
            }

            if (_errors.Count>0)
            {
                if (Inspector.ShowDialog(_errors) != true)
                {
                    Inspector.Show(_errors);
                    throw new OperationCanceledException();
                }
                //var formDublicates = new FormError(_errors, true);
                //formDublicates.Text = "Дублирование блоков";
                //formDublicates.EnableDublicateButtons();
                //if (Application.ShowModalDialog(formDublicates) != System.Windows.Forms.DialogResult.OK)
                //{
                //    formDublicates.EnableDialog(false);
                //    Application.ShowModelessDialog(formDublicates);
                //    throw new Exception("Отменено пользователем.");
                //}                
            }
        }

        private static void GetDublicateBlocks(IEnumerable ids, Matrix3d transToModel, double rotate)
        {            
            var idsBtrNext = new List<Tuple<ObjectId, Matrix3d, double>>();

            var isFirstDbo = true;
                  
            foreach (var item in ids)
            {
                if (!(item is ObjectId)) continue;
                var idEnt = (ObjectId)item;
                if (!idEnt.IsValidEx()) continue;
                var dbo = idEnt.GetObject(OpenMode.ForRead, false, true);
                // Проверялся ли уже такое определение блока                            
                if (isFirstDbo)
                {
                    isFirstDbo = false;
                    if (!attemptedblocks.Add(dbo.OwnerId))
                    {
                        continue;
                    }
                }

                var blRef = dbo as BlockReference;
                if (blRef == null || !blRef.Visible) continue;
                var blRefInfo = new BlockRefDublicateInfo(blRef, transToModel, rotate);

                if (_ignoreBlocks!=null && _ignoreBlocks.Contains(blRefInfo.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

				var ptTree = new PointTree(blRefInfo.Position.X, blRefInfo.Position.Y);

				if (!dictBlRefInfos.TryGetValue(blRefInfo.Name, out Dictionary<PointTree, List<BlockRefDublicateInfo>> dictPointsBlInfos))
                {
                    dictPointsBlInfos = new Dictionary<PointTree, List<BlockRefDublicateInfo>>();
                    dictBlRefInfos.Add(blRefInfo.Name, dictPointsBlInfos);
                }
				if (!dictPointsBlInfos.TryGetValue(ptTree, out List<BlockRefDublicateInfo> listBiAtPoint))
				{
					listBiAtPoint = new List<BlockRefDublicateInfo>();
					dictPointsBlInfos.Add(ptTree, listBiAtPoint);
				}
				listBiAtPoint.Add(blRefInfo);

                idsBtrNext.Add(new Tuple<ObjectId, Matrix3d, double>(item1: blRef.BlockTableRecord, item2: blRef.BlockTransform* transToModel, item3: blRef.Rotation + rotate));
            }

            // Нырок глубже
            if (curDepth < DEPTH)
            {
                curDepth++;
                foreach (var btrNext in idsBtrNext)
                {
                    var btr = btrNext.Item1.GetObject(OpenMode.ForRead) as BlockTableRecord;
                    GetDublicateBlocks(btr, btrNext.Item2, btrNext.Item3);
                }
            }
        }

        //private static void AddTransformedToModelDublic(List<BlockRefDublicateInfo> dublicBlRefInfos)
        //{
        //   // Трансформированные копии инфоблоков и добавление в результирующий список дубликатов
        //   var trancDublicBlRefInfos = dublicBlRefInfos.Select(b => b.TransCopy()).ToList();
        //   AllDublicBlRefInfos.AddRange(trancDublicBlRefInfos);
        //}

        public static void DeleteDublicates(List<IError> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return;
            }

            var blDublicatesToDel = errors.Where(e => e.Tag is BlockRefDublicateInfo)
				.SelectMany(e => ((BlockRefDublicateInfo)e.Tag).Dublicates);
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                using (var t = blDublicatesToDel.FirstOrDefault()?.IdBlRef.Database.TransactionManager.StartTransaction())
                {
                    foreach (var dublBl in blDublicatesToDel)
                    {
                        var blTodel = dublBl.IdBlRef.GetObject(OpenMode.ForWrite, false, true) as BlockReference;
                        blTodel.Erase();
                    }
                    t.Commit();
                }
            }            
        }
    }
}
