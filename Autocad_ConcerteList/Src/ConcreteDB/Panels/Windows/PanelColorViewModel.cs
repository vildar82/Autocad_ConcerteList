using System.ComponentModel;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.ConcreteDB.Panels.Windows
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
        public IPanel panel;
        public PanelColorViewModel(IPanel panel)
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
