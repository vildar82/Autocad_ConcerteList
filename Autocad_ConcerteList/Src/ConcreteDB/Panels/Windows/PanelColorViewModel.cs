using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public enum ColorStatusEnum
    {
        [Description("Новая")]
        New,
        [Description("Старая")]
        old
    }
    public class PanelColorViewModel : ObservableObject
    {
        public iPanel panel;
        public PanelColorViewModel(iPanel panel)
        {
            this.panel = panel;
            Mark = panel.Mark;
            Color = panel.ColorMark;
            MarkWithColor = panel.GetMarkWithColor();
            if (DbService.HasColorFullHandMark(panel.GetMarkWithColor()))
            {
                Status = ColorStatusEnum.old;                
            }
            else
            {
                Status = ColorStatusEnum.New;
                StatusBackground = new SolidColorBrush(Colors.Lime);;
            }
        }

        public string Mark { get; set; }
        public string Color { get; set; }
        public string MarkWithColor { get; set; }
        public ColorStatusEnum Status { get; set; }
        public Brush StatusBackground { get; set; }
    }
}
