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
        //private const string GroupKeyStair = "ЛМ";
        //private const string GroupKeyPL = "ПЛ";
        //private const string GroupKeyExteriorWall = "НС";
        //private const string GroupKeyInteriorWall = "В";
        /// <summary>
        /// Типы панелей по ключевому имени группы
        /// </summary>
        static Dictionary<PanelTypeEnum, Type> dictPanelTypesByGroupKey = new Dictionary<PanelTypeEnum, Type>() {
            { PanelTypeEnum.Stair, typeof(Stair) },
            { PanelTypeEnum.PL, typeof(PL) },
            { PanelTypeEnum.WallInner, typeof(InternalPanel) },
            { PanelTypeEnum.WallOuter, typeof(ExternalPanel) }
        };        

        /// <summary>
        /// Определение панели. Должна быть запущена транзакция!
        /// </summary>
        /// <param name="objectId">Entity</param>
        /// <returns>Панель или null если это не панель</returns>
        public static IIPanel Define(ObjectId objectId, out BlockReference blRef, out string blName)
        {
            IIPanel panel = null;
            if (IsPanel(objectId, out blRef, out string markAtr, out blName))
            {
                try
                {
                    // Есть атрибут Марки - значит это блок панели                
                    var resDefMarkPart = ParserMarkFactory.DefineParts(markAtr);
                    if (resDefMarkPart.Success)
                    {
                        MarkPart markPart = resDefMarkPart.Value;
                        if (!dictPanelTypesByGroupKey.TryGetValue(markPart.PanelType, out Type typePanel))
                        {
                            // Общая панель
                            typePanel = typeof(Panel);
                        }
                        panel = Activator.CreateInstance(typePanel, markPart, blRef, blName) as Panel;
                    }
                    else
                    {
                        Inspector.AddError(resDefMarkPart.Error, blRef, System.Drawing.SystemIcons.Error);
                    }
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(ex, $"Define - {blName}");
                    Inspector.AddError($"Ошибка при обработке блока '{blName}'. {ex.Message}", blRef);
                }
            }
            return panel;
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
