using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows;
using Autodesk.AutoCAD.ApplicationServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class RegColors
    {
        private List<Panel> panels;

        public RegColors (List<Panel> panels)
        {
            this.panels = panels;
        }

        public void Registry ()
        {
            // отобрать панели с колористикой
            var panelsColor = panels.Where(p => !string.IsNullOrWhiteSpace(p.Color)).ToList();

            // Если есть панели не найденные в базе - то это ошибка.
            var panelsNoInDb = panelsColor.Where(p => p.DbItem == null);
            if (panelsNoInDb.Any ())
            {
                Inspector.AddError($"Нельзя выполнить регистрацию колористических индексов, т.к. не все панели найдены в базе. См. ошибки.");
                foreach (var item in panelsNoInDb)
                {
                    Inspector.AddError($"Панель не найдена в базе - {item.Mark}. ", item.IdBlRef, System.Drawing.SystemIcons.Error);
                }
                return;
            }

            if (panelsColor.Any (p=>p.ErrorStatus!= ErrorStatusEnum.None))
            {
                Inspector.AddError($"Нельзя выполнить регистрацию колористических индексов, т.к. есть панели с ошибками.");
                return;
            }

            // Форма регистрации колористики
            var windowRegColor = new WindowRegColors();
            var regColorVM = new RegColorViewModel(panelsColor);
            windowRegColor.DataContext = regColorVM;
            var dialogRes = Application.ShowModalWindow(windowRegColor);
            if (dialogRes.HasValue && dialogRes.Value)
            {
                DbService.RegisterColors(panelsColor);
            }
        }
    }
}
