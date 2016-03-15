using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.ConcreteDB;
using Autocad_ConcerteList.RegystryPanel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autocad_ConcerteList.Commands))]

namespace Autocad_ConcerteList
{
    public class Commands
    {
        /// <summary>
        /// Создание марки панели ЖБИ - в базе и передача параметров панели в лисп функцию для создания блока в чертеже
        /// </summary>
        [CommandMethod("PIK", "SB_RegistrationPanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SB_RegistrationPanel()
        {
            Logger.Log.StartCommand(nameof(SB_RegistrationPanel));
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            try
            {
                // Проверка доступа. Только Лукашовой.
                if (!Access.Success())
                {
                    doc.Editor.WriteMessage("\nОтказано в доступе.");
                    return;
                }
                Inspector.Clear();

                // Форма формирования марки
                ItemForm itemForm = new ItemForm();
                Application.ShowModalDialog(itemForm);

                // Показ ошибок если они есть.
                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains("Отменено пользователем"))
                {
                    Logger.Log.Error(ex, $"{nameof(SB_RegistrationPanel)}. {doc.Name}");
                }
            }
        }

        /// <summary>
        /// Регистрация списка изделий при создании спецификации.
        /// Из лиспа вызывается эта функция с передачей списка изделий спецификации для регистрации в базе ЖБИ.
        /// </summary>
        [CommandMethod("PIK", "SB_RegistrationPanels", CommandFlags.Modal)]
        public void SB_RegistrationPanels()
        {            
            Logger.Log.StartCommand(nameof(SB_RegistrationPanels));
            Document doc = Application.DocumentManager.MdiActiveDocument;            

            try
            {
                // Проверка доступа. Только Лукашовой?????
                if (!Access.Success())
                {
                    doc.Editor.WriteMessage("\nОтказано в доступе.");
                    // Прерывание создания групповой спецификации
                    return;
                }
                Inspector.Clear();

                // Вызов лисп функции - сбора блоков и их параметров.
                var rb = InvokeLisp.CheckBlocks();

                // Парсинг переданного списка - превращение в список панелей
                ParserRb parserRb = new ParserRb(rb);
                parserRb.Parse();
                if (parserRb.Panels == null || parserRb.Panels.Count ==0)
                {
                    doc.Editor.WriteMessage("\nПрерывание. Ошибки в блоках при обработки панелей.");
                    return;                    
                }

                // Регистрация ЖБИ изделий в базе.
                RegystryPanels registryPanels = new RegystryPanels(parserRb.Panels);
                int regPanelsCount = registryPanels.Registry();

                doc.Editor.WriteMessage($"\nЗарегистрированно {regPanelsCount} ЖБИ.");

                Inspector.Show();              
            }
            catch (System.Exception ex)
            {                
                doc.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                if (!ex.Message.Contains("Отменено пользователем"))
                { 
                    // Непредвиденная ошибка
                    Logger.Log.Fatal(ex, $"{nameof(SB_RegistrationPanels)}. {doc.Name}");
                }                
            }            
        }        
    }
}
