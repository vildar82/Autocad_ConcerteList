using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using AcadLib;
using AcadLib.Extensions;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.ApplicationServices;
using AcadLib.Errors;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Создание панели
    /// </summary>
    public static class PanelFactory
    {
        private const string GroupKeyStair = "ЛМ";
        private const string GroupKeyPL = "ПЛ";
        //private const string GroupKeyExteriorWall = "НС";
        //private const string GroupKeyInteriorWall = "В";
        /// <summary>
        /// Типы панелей по ключевому имени группы
        /// </summary>
        static Dictionary<string, Type> dictPanelTypesByGroupKey = new Dictionary<string, Type>() {
            { GroupKeyStair, typeof(Stair) },
            { GroupKeyPL, typeof(PL) }
        };
        /// <summary>
        /// Типы панелей по item_group_long_id 
        /// </summary>
        static Dictionary<int, Type> dictPanelTypesByGroupLongId = new Dictionary<int, Type>() {
            { 12, typeof(InternalPanel) }, // Внутренние стеновые панели
            { 13, typeof(ExternalPanel) }, // Наружные стеновые панели
        };

        /// <summary>
        /// Определение панели. Должна быть запущена транзакция!
        /// </summary>
        /// <param name="objectId">Entity</param>
        /// <returns>Панель или null если это не панель</returns>
        public static iPanel Define(ObjectId objectId, out BlockReference blRef, out string blName)
        {
            iPanel panel = null;            
            string markAtr;            

            if (IsPanel(objectId, out blRef, out markAtr, out blName))
            {
                // Есть атрибут Марки - значит это блок панели
                MarkPart markPart;
                var resDefMarkPart = ParserMark.DefineParts(markAtr, out markPart);
                if (resDefMarkPart.Success)
                {
                    Type typePanel;
                    if (!dictPanelTypesByGroupKey.TryGetValue(GetGroupKey(markPart.PartGroup), out typePanel))
                    {
                        var groupLongId = DbService.GetGroupLongId(markPart.PartGroup);
                        if (!dictPanelTypesByGroupLongId.TryGetValue(groupLongId, out typePanel))
                        {
                            // Общая панель
                            typePanel = typeof(Panel);
                        }
                    }
                    panel = Activator.CreateInstance(typePanel, markPart, blRef, blName) as Panel;
                }
                else
                {
                    Inspector.AddError(resDefMarkPart.Error, blRef, System.Drawing.SystemIcons.Error);
                }
            }            
            return panel;
        }

        private static string GetGroupKey(string partGroup)
        {
            if (partGroup == GroupKeyStair)
            {
                return GroupKeyStair;
            }
            else if (partGroup == GroupKeyPL)
            {
                return GroupKeyPL;
            }
            //else if (partGroup.IndexOf(GroupKeyExteriorWall) != -1)
            //{
            //    return GroupKeyExteriorWall;
            //}
            //else if (partGroup.IndexOf(GroupKeyInteriorWall) != -1)
            //{
            //    return GroupKeyInteriorWall;
            //}
            return partGroup;
        }

        private static bool IsPanel(ObjectId objectId, out BlockReference blRef, out string markAtr, out string blName)
        {
            markAtr = null;
            blName = null;
            blRef = objectId.GetObject(OpenMode.ForRead, false, true) as BlockReference;
            if (blRef == null) return false;

            blName = blRef.GetEffectiveName();
            if (blRef.BlockTableRecord.IsNull)
            {
                Logger.Log.Error("blRef.BlockTableRecord.IsNull");
                Commands.HasNullObjectId = true;
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nПропущен блок '{blName}'");
            }
            else
            {
                if (IsIgnoredBlockName(blName))
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nПропущен блок '{blName}'");
                }
                else
                {
                    markAtr = GetAtrMark(blRef);
                    if (string.IsNullOrWhiteSpace(markAtr))
                    {
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nПропущен блок '{blName}'");
                        markAtr = null;
                    }
                }
            }
            return markAtr != null;
        }

        private static string GetAtrMark(BlockReference blRef)
        {
            if (blRef.AttributeCollection == null) return null;
            return blRef.EnumerateAttributes()?.FirstOrDefault(a => a.Tag.Equals(Panel.AtrTagMark, StringComparison.OrdinalIgnoreCase))?.Text?.Trim();
        }

        /// <summary>
        /// проверка - это игнорируемое имя блока - точно не изделие ЖБИ, например ММС
        /// </summary>
        public static bool IsIgnoredBlockName(string blName)
        {
            return Regex.IsMatch(blName, @"^ММС|^_|оси|ось|узел|узлы|формат|rab_obl|жук|\$|@|^\*|^РМВ");            
        }
    }
}
