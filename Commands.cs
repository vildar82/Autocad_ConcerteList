using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.ConcreteDB;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autocad_ConcerteList.Commands))]

namespace Autocad_ConcerteList
{
   public class Commands
   {
      /// <summary>
      /// Создание марки панели ЖБИ - в базе и передача параметров панели в лисп функцию для создания блока в чертеже
      /// </summary>
      [CommandMethod("PIK", "SB_Concrete_CreatePanel", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
      public void SB_Concrete_CreatePanel()
      {
         Logger.Log.StartCommand(nameof(SB_Concrete_CreatePanel));
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
            doc.Editor.WriteMessage($"\nОшибка экспорта колористических индексов: {ex.Message}");
            if (!ex.Message.Contains("Отменено пользователем"))
            {
               Logger.Log.Error(ex, $"{nameof(SB_Concrete_CreatePanel)}. {doc.Name}");
            }
         }
      }
   }
}
