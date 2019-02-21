using System.Collections.Generic;
using System.Linq;
using AcadLib;
using AcadLib.Errors;
using Autocad_ConcerteList.Src.ConcreteDB;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;
using Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autocad_ConcerteList.Commands))]
[assembly: ExtensionApplication(typeof(Autocad_ConcerteList.Commands))]

namespace Autocad_ConcerteList
{
    public class Commands : IExtensionApplication
    {
        public static Document Doc { get; set; }
        public static bool HasNullObjectId;

        public void Initialize()
        {
            LoadService.LoadEntityFramework();
        }

        /// <summary>
        /// Проверка одного блока - соответствия марки и параметров.
        /// </summary>
        [CommandMethod("PIK", "SB-CheckPanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_CheckPanel ()
        {
            CommandStart.Start(doc =>
            {   
                var ed = doc.Editor;
                var db = doc.Database;
                Inspector.Clear();
                // Запрос выбора блока
                var selOpt = new PromptEntityOptions("Выберите один блок панели для проверки");
                selOpt.SetRejectMessage("Это не блок");
                selOpt.AddAllowedClass(typeof(BlockReference), true);
                var sel = ed.GetEntity(selOpt);
                if (sel.Status == PromptStatus.OK)
                {
                    using (var t = db.TransactionManager.StartTransaction())
                    {
                        DbService.Init();
                        var panel = PanelFactory.Define(sel.ObjectId, out BlockReference blRef, out string blName);
                        if (panel == null)
                        {
                            ed.WriteMessage("\nБлок панели не определен.");
                            return;
                        }                                                        
                        
                        // Проверка соответствия марки и параметров в блоке панели                        
                        panel.Checks();                        

                        if (panel.HasErrors || !panel.IsWeightOk)
                        {
                            var checkPanel = new List<KeyValuePair<IIPanel, List<IIPanel>>>
                            {
                                new KeyValuePair<IIPanel, List<IIPanel>>(panel, new List<IIPanel> { panel })
                            };
                            var model = new CheckPanelsViewModel (checkPanel);
                            var winPanels = new WindowCheckPanels(model);
                            Application.ShowModalWindow(winPanels);                            
                        }
                        else
                        {
                            var msg = "\nОшибок в панели не обнаружено. " + (panel.DbItem == null ? "В базе НЕТ." : "В базе ЕСТЬ.");
                            if (!string.IsNullOrEmpty(panel.Warning))
                            {
                                msg += " Предупреждения: " + panel.Warning;
                            }
                            ed.WriteMessage(msg);
                        }
                        t.Commit();
                    }
                }                
            });
        }        

        /// <summary>
        /// Проверка блоков
        /// </summary>
        [CommandMethod("PIK", "SB-CheckPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_CheckPanels ()
        {
            CommandStart.Start(doc =>
            {
                HasNullObjectId = false;
                var ed = doc.Editor;
                DbService.Init();

                // Поиск изделей в чертеже
                var filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                // группировка панелей - по уникальной марке (без пробелов)
                var groupedMarkPanels = panels.GroupBy(p=>p.MarkWoSpace).OrderBy(o=>o.Key, NetLib.Comparers.AlphanumComparator.New);

                // Проверка одинаковости панелей в группе (должны быть одинаковыми все параметры)
                var checkedPanels = new List<KeyValuePair<IIPanel, List<IIPanel>>> ();
                foreach (var item in groupedMarkPanels)
                {
                    var first = item.FirstOrDefault(i => i.IsNew != null && !i.IsNew.Value);
                    if (first == null)
                    {
                        first = item.First();
                    }
                    var someParams = item.GroupBy(g => g);
                    if (someParams.Skip(1).Any())
                    {
                        // Ошибка - разные параметры в панелях с одной маркой
                        first.ErrorStatus |= ErrorStatusEnum.DifferentParamInGroup;
                        first.Warning += " Разные атрибуты в блоках, см. детальный вид. ";
                    }
                    checkedPanels.Add(new KeyValuePair<IIPanel, List<IIPanel>>(first, item.ToList()));
                }
                
                if (checkedPanels.Count == 0)
                {
                    ed.WriteMessage($"\nПанели не найдены.");
                }
                else
                {
                    var model = new CheckPanelsViewModel (checkedPanels);
                    var winPanels = new WindowCheckPanels(model);
                    Application.ShowModelessWindow(winPanels);
                }
                ed.WriteMessage($"\nОбработано {panels.Count} блоков панелей.");

                CheckNullObjectId();
            });
        }

        /// <summary>
        /// Регистрация новых панелей
        /// </summary>
        [CommandMethod("PIK", "SB-RegPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_RegPanels()
        {
            CommandStart.Start(doc =>
            {   
                var ed = doc.Editor;                
                // Проверка доступа. Только Лукашовой?????
                if (!Access.Success())
                {
                    ed.WriteMessage("\nОтказано в доступе.");
                    return;
                }               

                DbService.Init();

                var filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                var regPanels = new RegPanels(panels);
                regPanels.Registry();                
            });
        }

        [CommandMethod("PIK", "SB-RegColors", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_RegColors()
        {
            CommandStart.Start(doc =>
            {
                var ed = doc.Editor;
                // Проверка доступа. Только Лукашовой?????
                if (!Access.Success())
                {
                    ed.WriteMessage("\nОтказано в доступе.");
                    return;
                }

                DbService.Init();

                var filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                var regPanels = new RegColors(panels);
                regPanels.Registry();
            });
        }

        private void CheckNullObjectId ()
        {
            if (HasNullObjectId)
            {
                Inspector.AddError($"Найдены объекты с ошибками, рекомендуется выполнить проверку чертежа с исправлением ошибок (команда _audit).",
                    System.Drawing.SystemIcons.Warning);
            }
        }

        public void Terminate()
        {            
        }
    }
}