using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Проверка наличия панели в базе данных
        /// По переданным параметрам панели
        /// Возвращает nil или список панелей (handMark и марку по формуле)
        /// </summary>
        [LispFunction("KR_NR_CheckPanelInDb")]
        public ResultBuffer KR_NR_CheckPanelInDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();
            // Проверка наличия панели в базе по набору параметров            
            var dtItems = DbService.FindByParameters(panel);
            if (dtItems.Count == 0)
            {
                // Такой панели нет - return nil
                return null;
            }            
            // Формирование возвращаемого списка - найденных панелей (HandMark + марка по формуле)            
            ResultBuffer resVal = new ResultBuffer();
            resVal.Add(new TypedValue((int)LispDataType.ListBegin));
            foreach (var item in dtItems)
            {
                // HandMark - точечная пара                
                resVal.Add(new TypedValue((int)LispDataType.ListBegin));
                resVal.Add(new TypedValue((int)LispDataType.Text, "HandMark"));
                resVal.Add(new TypedValue((int)LispDataType.DottedPair));
                resVal.Add(new TypedValue((int)LispDataType.Text, item.HandMarkNoColour));
                resVal.Add(new TypedValue((int)LispDataType.ListEnd));
                // Марка по формуле - точечная пара
                resVal.Add(new TypedValue((int)LispDataType.ListBegin));
                resVal.Add(new TypedValue((int)LispDataType.Text, "ByFormula"));
                resVal.Add(new TypedValue((int)LispDataType.DottedPair));
                resVal.Add(new TypedValue((int)LispDataType.Text, panel.MarkDbWoSpace));
                resVal.Add(new TypedValue((int)LispDataType.ListEnd));
            }
            resVal.Add(new TypedValue((int)LispDataType.ListEnd));
            return resVal;
        }        

        /// <summary>
        /// Регистрация панели в базе
        /// </summary>
        [LispFunction("KR_NR_RegisterPanelInDb")]
        public void KR_NR_RegisterPanelInDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();            
            // Определение серии ПИК1
            var series = DbService.GetSeries();
            var serPik1 = series.First(s => s.Series.Equals("ПИК-1.0"));
            // Регистрация панели в базе
            DbService.Register(panel, serPik1);
        }

        /// <summary>
        /// Удаление панели из базы
        /// </summary>
        [LispFunction("KR_NR_RemovePanelFromDb")]
        public void KR_NR_RemovePanelFromDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();            
            DbService.RemovePanel(panel);
        }

        private static ParserRb ParseArgs(ResultBuffer args)
        {
            // Парсинг аргументов - параметров панели            
            ParserRb parserRb = new ParserRb(args);
            parserRb.Parse();
            if (parserRb.Panels == null || parserRb.Panels.Count == 0)
                throw new ArgumentException("Не определены параметры панели при парсинге переданных аргументов.");

            if (parserRb.Panels.Count > 1)
                throw new ArgumentException("Передано больше одной панели.");
            return parserRb;
        }

        ///// <summary>
        ///// Создание марки панели ЖБИ - в базе и передача параметров панели в лисп функцию для создания блока в чертеже
        ///// </summary>
        //[CommandMethod("PIK", "SB-RegistrationPanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        //public void SB_RegistrationPanel()
        //{
        //    Logger.Log.StartCommand(nameof(SB_RegistrationPanel));
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    if (doc == null) return;

        //    try
        //    {
        //        // Проверка доступа. Только Лукашовой.
        //        if (!Access.Success())
        //        {
        //            doc.Editor.WriteMessage("\nОтказано в доступе.");
        //            return;
        //        }
        //        Inspector.Clear();

        //        // Форма формирования марки
        //        ItemForm itemForm = new ItemForm();
        //        Application.ShowModalDialog(itemForm);

        //        // Показ ошибок если они есть.
        //        Inspector.Show();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
        //        if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
        //        {
        //            Logger.Log.Error(ex, $"{nameof(SB_RegistrationPanel)}. {doc.Name}");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Регистрация списка изделий при создании спецификации.
        ///// Из лиспа вызывается эта функция с передачей списка изделий спецификации для регистрации в базе ЖБИ.
        ///// </summary>
        //[CommandMethod("PIK", "SB-RegistrationPanels", CommandFlags.Modal)]
        //public void SB_RegistrationPanels()
        //{
        //    Logger.Log.StartCommand(nameof(SB_RegistrationPanels));
        //    Document doc = Application.DocumentManager.MdiActiveDocument;

        //    try
        //    {
        //        // Проверка доступа. Только Лукашовой?????
        //        //if (!Access.Success())
        //        //{
        //        //    doc.Editor.WriteMessage("\nОтказано в доступе.");
        //        //    // Прерывание создания групповой спецификации
        //        //    return;
        //        //}
        //        Inspector.Clear();

        //        // Вызов лисп функции - сбора блоков и их параметров.
        //        var rb = InvokeLisp.GetRbPanels();

        //        // Парсинг переданного списка - превращение в список панелей
        //        ParserRb parserRb = new ParserRb(rb);
        //        parserRb.Parse();
        //        if (parserRb.Panels == null || parserRb.Panels.Count == 0)
        //        {
        //            doc.Editor.WriteMessage("\nПрерывание. Ошибки в блоках при обработке панелей.");
        //            return;
        //        }

        //        var panels = parserRb.Panels.OrderBy(p => p.Mark).ToList();

        //        // Поиск блока для кажждой панели
        //        Panel.FindBlocks(panels);

        //        // Регистрация ЖБИ изделий в базе.
        //        RegystryPanels registryPanels = new RegystryPanels(panels);
        //        int regPanelsCount = registryPanels.Registry();

        //        doc.Editor.WriteMessage($"\nЗарегистрированно {regPanelsCount} ЖБИ.");

        //        Inspector.Show();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
        //        if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
        //        {
        //            // Непредвиденная ошибка
        //            Logger.Log.Fatal(ex, $"{nameof(SB_RegistrationPanels)}. {doc.Name}");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Проверка одного блока - соответствия марки и параметров.
        ///// </summary>
        //[CommandMethod("PIK", "SB-CheckPanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        //public void SB_CheckPanel()
        //{
        //    Logger.Log.StartCommand(nameof(SB_CheckPanel));
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    if (doc == null) return;
        //    Editor ed = doc.Editor;

        //    try
        //    {                
        //        Inspector.Clear();

        //        // Запрос выбора блока
        //        var selOpt =new PromptEntityOptions("Выберите один блок панели для проверки");
        //        selOpt.SetRejectMessage("Это не блок");
        //        selOpt.AddAllowedClass(typeof(BlockReference), true);                
        //        var sel = ed.GetEntity(selOpt);
        //        if (sel.Status == PromptStatus.OK)
        //        {
        //            DbService.Init();
        //            Panel panel = new Panel();
        //            var resDefine = panel.Define(sel.ObjectId);      
        //            if (resDefine.Failure)
        //            {
        //                ed.WriteMessage("\nНе определен блок панели - " + resDefine.Error);
        //                return;
        //            }
        //            // Проверка соответствия марки и параметров в блоке панели
        //            panel.Check();

        //            if(panel.ErrorStatus != EnumErrorItem.None)
        //            {
        //                FormPanels panelForm = new FormPanels(new List<Panel> { panel });
        //                panelForm.Text = "Панели с ошибками";
        //                panelForm.BackColor = System.Drawing.Color.Red;
        //                panelForm.buttonCancel.Visible = false;
        //                panelForm.buttonOk.Visible = false;
        //                Application.ShowModelessDialog(panelForm);
        //                panelForm.listViewPanels.Items[0].Selected = true;
        //            }
        //            else
        //            {
        //                string msg = "\nОшибок в панели не обнаружено. " + (panel.DbItem==null ? "В базе НЕТ." : "В базе ЕСТЬ.");
        //                if (!string.IsNullOrEmpty(panel.Warning))
        //                {
        //                    msg += " Предупреждения: " + panel.Warning;
        //                }                                                
        //                ed.WriteMessage(msg);
        //            }                    
        //        }                                

        //        // Показ ошибок если они есть.
        //        Inspector.Show();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
        //        if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
        //        {
        //            Logger.Log.Error(ex, $"{nameof(SB_CheckPanel)}. {doc.Name}");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Проверка блоков
        ///// </summary>
        //[CommandMethod("PIK", "SB-CheckPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        //public void SB_CheckPanels()
        //{
        //    Logger.Log.StartCommand(nameof(SB_CheckPanels));
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    if (doc == null) return;
        //    Editor ed = doc.Editor;

        //    try
        //    {
        //        Inspector.Clear();
        //        DbService.Init();
        //        //var sel = ed.SelectBlRefs("Выбор блоков");
        //        //var panels = Model.Panels.FilterPanel.Filter(sel);
        //        Model.Panels.FilterPanel filter = new Model.Panels.FilterPanel();
        //        filter.Filter();
        //        var panels = filter.Panels;

        //        Model.Panels.CheckPanels checkPanels = new Model.Panels.CheckPanels(panels);
        //        checkPanels.Check();

        //        Inspector.Show();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
        //        if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
        //        {
        //            Logger.Log.Error(ex, $"{nameof(SB_CheckPanels)}. {doc.Name}");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Регистрация новых панелей
        ///// </summary>
        //[CommandMethod("PIK", "SB-RegPanels", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        //public void SB_RegPanels()
        //{
        //    Logger.Log.StartCommand(nameof(SB_RegPanels));
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    if (doc == null) return;
        //    Editor ed = doc.Editor;

        //    try
        //    {
        //        // Проверка доступа. Только Лукашовой?????
        //        if (!Access.Success())
        //        {
        //            doc.Editor.WriteMessage("\nОтказано в доступе.");                    
        //            return;
        //        }

        //        Inspector.Clear();

        //        DbService.Init();                

        //        Model.Panels.FilterPanel filter = new Model.Panels.FilterPanel();
        //        filter.Filter();
        //        var panels = filter.Panels;

        //        Model.Panels.RegPanels regPanels = new Model.Panels.RegPanels(panels);
        //        int regCount = regPanels.Registry();
        //        ed.WriteMessage($"\nЗарегистрировано {regCount} панелей.");

        //        Inspector.Show();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
        //        if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
        //        {
        //            Logger.Log.Error(ex, $"{nameof(SB_RegistrationPanel)}. {doc.Name}");
        //        }
        //    }
        //}
    }
}
