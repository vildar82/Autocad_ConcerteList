using System.Collections.Generic;
using System.Linq;
using Autocad_ConcerteList.ConcreteDB.Panels.Windows;
using Autocad_ConcerteList.Errors;
using Autodesk.AutoCAD.ApplicationServices;

namespace Autocad_ConcerteList.ConcreteDB.Panels
{
    public class RegColors
    {
        private readonly List<IPanel> panels;

        public RegColors (List<IPanel> panels)
        {
            this.panels = panels;
        }

        public void Registry ()
        {
            // отобрать панели с колористикой
            //var panelsWithColor = panels.Where(p => !string.IsNullOrWhiteSpace(p.Color)).ToList();
            // Если есть панели не найденные в базе - то это ошибка.            
            var panelsColor = new List<IPanel>();
            var panelsNew = new List<IPanel>();
            var panelsErr = new List<IPanel>();
            foreach (var item in panels)
            {
                if (string.IsNullOrWhiteSpace(item.ColorMark)) continue;

                if (item.DbItem == null)
                {
                    panelsNew.Add(item);
                }
                else
                {
                    panelsColor.Add(item);
                }                
            }

            if (panelsNew.Any())
            {
                Inspector.AddError($"!Перед регистрацией колористики, нужно зарегистрировать новые панели (не найденные в базе). См.список. Для этих панелей не будет зарегистрирована колористика.");                
                foreach (var item in panelsNew.GroupBy(g=>g.Mark))
                {
                    Inspector.AddError($"Новая панель (не найдена в базе) - {item.Key}. ", item.First().IdBlRef,
                        System.Drawing.SystemIcons.Error);
                }
            }            

            if (panelsColor.Any (p=>p.ErrorStatus!= ErrorStatusEnum.None))
            {
                Inspector.AddError($"!Необходимо проверить панели с ошибками перед регистрацией колористики. См. ошибки. Для этих панелей будет зарегистрирована колористика.");
                var panelsWithErr = panelsColor.Where(p => p.ErrorStatus != ErrorStatusEnum.None);
                foreach (var item in panelsWithErr)
                {
                    Inspector.AddError($"Панель с ошибками - {item.Mark}. {item.GetErrorStatusDesc()}", item.IdBlRef, System.Drawing.SystemIcons.Error);
                }                
            }

            Inspector.ShowDialog();

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
