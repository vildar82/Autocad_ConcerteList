using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CommandStart.Start((doc) =>
            {
                Editor ed = doc.Editor;
                Database db = doc.Database;
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
                        Panel panel = new Panel();
                        var resDefine = panel.Define(sel.ObjectId);
                        if (resDefine.Failure)
                        {
                            ed.WriteMessage("\nНе определен блок панели - " + resDefine.Error);
                            return;
                        }
                        // Проверка соответствия марки и параметров в блоке панели                        
                        panel.Checks();                        

                        if (panel.HasErrors || !panel.IsWeightOk)
                        {
                            WindowCheckPanels winPanels = new WindowCheckPanels(panel);
                            Application.ShowModalWindow(winPanels);
                            //FormPanels panelForm = new FormPanels(new List<Panel> { panel });
                            //panelForm.Text = "Панели с ошибками";
                            //panelForm.BackColor = System.Drawing.Color.Red;
                            //panelForm.buttonCancel.Visible = false;
                            //panelForm.buttonOk.Visible = false;
                            //Application.ShowModelessDialog(panelForm);
                            //panelForm.listViewPanels.Items[0].Selected = true;
                        }
                        else
                        {
                            string msg = "\nОшибок в панели не обнаружено. " + (panel.DbItem == null ? "В базе НЕТ." : "В базе ЕСТЬ.");
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
            CommandStart.Start((doc) =>
            {
                Editor ed = doc.Editor;
                DbService.Init();

                // Поиск изделей в чертеже
                FilterPanel filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                // группировка панелей - по уникальной марке (без пробелов)
                var groupedMarkPanels = panels.GroupBy(p=>p.MarkWoSpace).OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New);

                // Проверка одинаковости панелей в группе (должны быть одинаковыми все параметры)
                List<KeyValuePair<Panel, List<Panel>>> checkedPanels = new List<KeyValuePair<Panel, List<Panel>>> ();
                foreach (var item in groupedMarkPanels)
                {
                    var first = item.FirstOrDefault(i=>i.IsNew!=null && !i.IsNew.Value);
                    if (first == null)
                    {
                        first = item.First();
                    }
                    var someParams = item.GroupBy(g => g);
                    if (someParams.Skip(1).Any())
                    {
                        // Ошибка - разные параметры в панелях с одной маркой
                        first.ErrorStatus |= ErrorStatusEnum.DifferentParamInGroup;
                        first.Warning += $" Различие параметров в панелях этой марки {first.Mark}.";
                    }
                                     
                    if (first.IsNew != null && first.IsNew.Value || first.HasErrors | !first.IsWeightOk)
                    {
                        checkedPanels.Add(new KeyValuePair<Panel, List<Panel>>(first, item.ToList()));
                    }
                    else
                    {
                        ed.WriteMessage($"\nВ панеле марки {first.Mark} не найдено ошибок и она есть в базе.");
                    }
                }

                if (checkedPanels.Count == 0)
                {
                    ed.WriteMessage($"\nНет новых панелей и нет панелей с ошибками.");
                }
                else
                {
                    WindowCheckPanels winPanels = new WindowCheckPanels(checkedPanels, "Панели с ошибками");
                    Application.ShowModelessWindow(winPanels);
                }
                ed.WriteMessage($"\nОбработано {panels.Count} блоков панелей.");
            });
        }

        /// <summary>
        /// Регистрация новых панелей
        /// </summary>
        [CommandMethod("PIK", "SB-RegPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_RegPanels()
        {
            CommandStart.Start((doc) =>
            {
                Editor ed = doc.Editor;                
                // Проверка доступа. Только Лукашовой?????
                if (!Access.Success())
                {
                    ed.WriteMessage("\nОтказано в доступе.");
                    return;
                }               

                DbService.Init();

                FilterPanel filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                RegPanels regPanels = new RegPanels(panels);                
            });
        }

        public void Terminate()
        {            
        }
    }
}
