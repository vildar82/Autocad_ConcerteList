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
            Logger.Log.StartCommand(nameof(SB_CheckPanel));
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            try
            {
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
                        panel.DefineDbParams(false);
                        panel.Check();

                        if (panel.HasErrors)
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
                // Показ ошибок если они есть.
                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, $"{nameof(SB_CheckPanel)}. {doc.Name}");
                }
            }
        }

        /// <summary>
        /// Проверка блоков
        /// </summary>
        [CommandMethod("PIK", "SB-CheckPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_CheckPanels()
        {
            Logger.Log.StartCommand(nameof(SB_CheckPanels));
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            try
            {
                Inspector.Clear();

                DbService.Init();
                
                // Поиск изделей в чертеже
                FilterPanel filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;
                // группировка панелей - с одинаковыми параметрами
                var groupedPanels = panels.GroupBy(p=>p).OrderBy(p=>p.Key).ToList();
                // Определение параметров панелей из базы данных
                foreach (var item in groupedPanels)
                {
                    try
                    {
                        item.Key.DefineDbParams(true);
                        item.Key.Check();
                    }
                    catch (System.Exception ex)
                    {
                        Inspector.AddError($"Ошибка обработки панели {item.Key.Mark} - {ex}");
                    }
                }
                
                // Панели для показа в форме проверки - новые и с ошибками
                var checkPanels = groupedPanels.Where(p => p.Key.IsNew || p.Key.HasErrors).ToList();
                if (checkPanels.Count == 0)
                {
                    ed.WriteMessage($"\nНет новых панелей и нет панелей с ошибками.");
                }
                else
                {                    
                    WindowCheckPanels winPanels = new WindowCheckPanels(checkPanels, "Панели с ошибками");
                    Application.ShowModelessWindow(winPanels);
                }
                ed.WriteMessage($"\nОбработано {panels.Count} блоков панелей.");

                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains(General.CanceledByUser))
                {
                    Logger.Log.Error(ex, $"{nameof(SB_CheckPanels)}. {doc.Name}");
                }
            }
        }

        /// <summary>
        /// Регистрация новых панелей
        /// </summary>
        [CommandMethod("PIK", "SB-RegPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_RegPanels()
        {
            Logger.Log.StartCommand(nameof(SB_RegPanels));
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

            try
            {
                // Проверка доступа. Только Лукашовой?????
                if (!Access.Success())
                {
                    doc.Editor.WriteMessage("\nОтказано в доступе.");
                    return;
                }

                Inspector.Clear();

                DbService.Init();

                FilterPanel filter = new FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                RegPanels regPanels = new RegPanels(panels);
                int regCount = regPanels.Registry();
                ed.WriteMessage($"\nЗарегистрировано {regCount} панелей.");

                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                {
                    Logger.Log.Error(ex, $"{nameof(SB_RegPanels)}. {doc.Name}");
                }
            }
        }

        public void Terminate()
        {            
        }
    }
}
