using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;
using Autocad_ConcerteList.Model.ConcreteDB;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autocad_ConcerteList.Model.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autocad_ConcerteList.Commands))]

namespace Autocad_ConcerteList
{
    public class Commands
    {
        /// <summary>
        /// Проверка одного блока - соответствия марки и параметров.
        /// </summary>
        [CommandMethod("PIK", "SB-CheckPanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_CheckPanel()
        {
            Logger.Log.StartCommand(nameof(SB_CheckPanel));
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;

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
                    DbService.Init();
                    Panel panel = new Panel();
                    var resDefine = panel.Define(sel.ObjectId);
                    if (resDefine.Failure)
                    {
                        ed.WriteMessage("\nНе определен блок панели - " + resDefine.Error);
                        return;
                    }
                    // Проверка соответствия марки и параметров в блоке панели
                    panel.Check();

                    if (panel.ErrorStatus != EnumErrorItem.None)
                    {
                        FormPanels panelForm = new FormPanels(new List<Panel> { panel });
                        panelForm.Text = "Панели с ошибками";
                        panelForm.BackColor = System.Drawing.Color.Red;
                        panelForm.buttonCancel.Visible = false;
                        panelForm.buttonOk.Visible = false;
                        Application.ShowModelessDialog(panelForm);
                        panelForm.listViewPanels.Items[0].Selected = true;
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
                }

                // Показ ошибок если они есть.
                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
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
                Model.Panels.FilterPanel filter = new Model.Panels.FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                Model.Panels.CheckPanels checkPanels = new Model.Panels.CheckPanels(panels);
                checkPanels.Check();

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

                Model.Panels.FilterPanel filter = new Model.Panels.FilterPanel();
                filter.Filter();
                var panels = filter.Panels;

                Model.Panels.RegPanels regPanels = new Model.Panels.RegPanels(panels);
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
    }
}
