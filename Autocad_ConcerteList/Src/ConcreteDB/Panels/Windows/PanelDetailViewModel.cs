using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelDetailViewModel : ObservableObject
    {
        private static Brush BadValue = new SolidColorBrush(Colors.Brown);        

        private Panel panelDetail;
        private Panel panelFirst; 

        public PanelDetailViewModel (Panel itemDetail, Panel firstPanel)
        {
            panelDetail = itemDetail;
            panelFirst = firstPanel;
        }        

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>               
        public string MarkAtr {
            get { return panelDetail.Mark; }
        }
        public Brush MarkAtrBackground {
            get { return panelDetail.Mark == panelFirst.Mark ? null : BadValue; }
        }
        /// <summary>
        /// Имя блока
        /// </summary>               
        public string BlockName {
            get { return panelDetail.BlockName; }
        }
        public Brush BlockNameBackground {
            get { return panelDetail.BlockName == panelFirst.BlockName ? null : BadValue; }
        }
        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get { return panelDetail.Lenght; }
        }
        public Brush LengthBackground {
            get { return panelDetail.Lenght == panelFirst.Lenght ? null : BadValue; }
        }        

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get { return panelDetail.Height; }
        }
        public Brush HeightBackground {
            get { return panelDetail.Height == panelFirst.Height ? null : BadValue; }
        }                
        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get { return panelDetail.Thickness; }
        }
        public Brush ThicknessBackground {
            get { return panelDetail.Thickness == panelFirst.Thickness ? null : BadValue; }
        }                                
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get { return panelDetail.Weight; }
        }
        public Brush WeightBackground {
            get { return panelDetail.Weight == panelFirst.Weight ? null : BadValue; }
        }         

        /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public ICommand Show { get { return new RelayCommand(() => panelDetail.Show(), () => panelDetail.CanShow()); } }
    }
}
