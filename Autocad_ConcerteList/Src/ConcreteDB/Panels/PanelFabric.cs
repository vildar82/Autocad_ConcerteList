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
        Dictionary<string, Type> dictPanelTypes = new Dictionary<string, Type>();
                
        /// <summary>
        /// Определение панели. Должна быть запущена транзакция!
        /// </summary>
        /// <param name="objectId">Entity</param>
        /// <returns>Панель или null если это не панель</returns>
        public static iPanel Define(ObjectId objectId)
        {
            iPanel panel = null;

            var blRef = objectId.GetObject(OpenMode.ForRead, false, true) as BlockReference;
            if (blRef != null)
            {
                if (blRef.BlockTableRecord.IsNull)
                {
                    Logger.Log.Error("blRef.BlockTableRecord.IsNull");
                    Commands.HasNullObjectId = true;                    
                }
                else
                {
                    var blName = blRef.GetEffectiveName();
                    if (IsIgnoredBlockName(blName))
                    {
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nПропущен блок '{blName}'");                        
                    }
                    else
                    {
                        var markAtr = GetAtrMark(blRef);
                        if (markAtr != null)
                        {
                            // Есть атрибут Марки - значит это блок панели
                            MarkPart markPart;
                            var resDefMarkPart = ParserMark.DefineParts(markAtr, out markPart);
                            if (resDefMarkPart.Success)
                            {
                                
                            }
                            else
                            {
                                Inspector.AddError(resDefMarkPart.Error, blRef, System.Drawing.SystemIcons.Error);
                            }
                        }
                    }                    
                }
            }
            return panel;
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
