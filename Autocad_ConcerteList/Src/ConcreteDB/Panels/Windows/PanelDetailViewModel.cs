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

        public Panel PanelDetail {
            get { return panelDetail; }
        }

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>               
        public string MarkAtr {
            get { return panelDetail.Mark; }
            set { RaisePropertyChanged(); }
        }
        public Brush MarkAtrBackground {
            get { return panelDetail.Mark == panelFirst.Mark ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Имя блока
        /// </summary>               
        public string BlockName {
            get { return panelDetail.BlockName; }
            set { RaisePropertyChanged(); }
        }
        public Brush BlockNameBackground {
            get { return panelDetail.BlockName == panelFirst.BlockName ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get { return panelDetail.Lenght; }
            set {RaisePropertyChanged();}
        }
        public Brush LengthBackground {
            get { return panelDetail.Lenght == panelFirst.Lenght ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }        

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get { return panelDetail.Height; }
            set { RaisePropertyChanged(); }
        }
        public Brush HeightBackground {
            get { return panelDetail.Height == panelFirst.Height ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }                
        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get { return panelDetail.Thickness; }
            set { RaisePropertyChanged(); }
        }
        public Brush ThicknessBackground {
            get { return panelDetail.Thickness == panelFirst.Thickness ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }                                
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get { return panelDetail.Weight; }
            set { RaisePropertyChanged(); }
        }
        public Brush WeightBackground {
            get { return panelDetail.Weight == panelFirst.Weight ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }

        /// <summary>
        /// Проем
        /// </summary>
        public string Aperture {
            get { return panelDetail.Aperture; }
            set { RaisePropertyChanged(); }
        }
        public Brush ApertureBackground {
            get { return panelDetail.Aperture == panelFirst.Aperture ? null : BadValue; }
            set { RaisePropertyChanged(); }
        }

        public string Section {
            get {
                return panelDetail.WS?.Section;
            }
        }
        /// <summary>
        /// Этаж
        /// </summary>
        public string Floor {
            get {
                return panelDetail.WS?.Floor;
            }
        }

        /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public ICommand Show { get { return new RelayCommand(() => panelDetail.Show(), () => panelDetail.CanShow()); } }

        internal void UpdateCheckPanel ()
        {
            panelDetail.Checks();
            this.Aperture = null;
            this.ApertureBackground = null;
            this.BlockName = null;
            this.BlockNameBackground = null;
            this.Height = null;
            this.HeightBackground = null;
            this.Length = null;
            this.LengthBackground = null;
            this.MarkAtr = null;
            this.MarkAtrBackground = null;
            this.Thickness = null;
            this.ThicknessBackground = null;
            this.Weight = null;
            this.WeightBackground = null;
        }
    }
}
