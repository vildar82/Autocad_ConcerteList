using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.RegystryPanel.IncorrectMark;

namespace Autocad_ConcerteList.RegystryPanel
{
    public static class IncorrectMarks
    {
        /// <summary>
        /// Показ несоответствующих марок панелей
        /// </summary>        
        public static bool Show(List<Panel> incorrectPanels)
        {
            if (incorrectPanels == null || incorrectPanels.Count == 0)
            {
                return true;
            }

            FormIncorrectMarks formIncorrect = new FormIncorrectMarks(incorrectPanels);
            return formIncorrect.ShowDialog()== System.Windows.Forms.DialogResult.OK ? true : false;
        }

        /// <summary>
        /// Исправленние панелей с некорректными марками на чертеже
        /// </summary>        
        public static void Fix(List<Panel> panelsIncorrectMark)
        {
            // Найти блоки со старой маркой и исправить на марку из базы.
        }
    }
}
