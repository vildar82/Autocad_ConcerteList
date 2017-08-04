using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.ConcreteDB.Panels.Windows
{
    public class PanelDetailViewModel : ObservableObject
    {
        private static readonly Brush BadValue = new SolidColorBrush(Colors.Brown);
        private static readonly Brush NoParameterColor = new SolidColorBrush(Colors.Gray);

        private readonly IPanel panelDetail;
        private readonly IPanel panelFirst; 

        public PanelDetailViewModel (IPanel itemDetail, IPanel firstPanel)
        {
            panelDetail = itemDetail;
            panelFirst = firstPanel;
            Show = new RelayCommand(OnShowExecute);
        }        

        public IPanel PanelDetail => panelDetail;

	    /// <summary>
        /// Марка панели из атрибута
        /// </summary>               
        public string MarkAtr {
            get => panelDetail.Mark;
		    set => RaisePropertyChanged();
	    }
        public Brush MarkAtrBackground {
            get => panelDetail.Mark == panelFirst.Mark ? null : BadValue;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Имя блока
        /// </summary>               
        public string BlockName {
            get => panelDetail.BlName;
	        set => RaisePropertyChanged();
        }
        public Brush BlockNameBackground {
            get => panelDetail.BlName == panelFirst.BlName ? null : BadValue;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get => panelDetail.Length;
	        set => RaisePropertyChanged();
        }
        public Brush LengthBackground {
            get {
                if (panelDetail.LengthHasProperty)
                    return panelDetail.Length == panelFirst.Length ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set => RaisePropertyChanged();
        }
        public string LengthDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

	    /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get => panelDetail.Height;
		    set => RaisePropertyChanged();
	    }
        public Brush HeightBackground {
            get {
                if (panelDetail.HeightHasProperty)
                    return panelDetail.Height == panelFirst.Height ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set => RaisePropertyChanged();
        }
        public string HeightDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

	    /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get => panelDetail.Thickness;
		    set => RaisePropertyChanged();
	    }
        public Brush ThicknessBackground {
            get {
                if (panelDetail.ThicknessHasProperty)
                    return panelDetail.Thickness == panelFirst.Thickness ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set => RaisePropertyChanged();
        }
        public string ThicknessDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

	    /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get => panelDetail.Weight;
	        set => RaisePropertyChanged();
        }
        public Brush WeightBackground {
            get {
                if (panelDetail.WeightHasProperty)
                    return panelDetail.Weight == panelFirst.Weight ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set => RaisePropertyChanged();
        }
        public string WeightDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

	    /// <summary>
        /// Проем
        /// </summary>
        public string Aperture {
            get => panelDetail.Aperture;
		    set => RaisePropertyChanged();
	    }
        public Brush ApertureBackground {
            get {
                if (panelDetail.ApertureHasProperty)
                    return panelDetail.Aperture == panelFirst.Aperture ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set => RaisePropertyChanged();
        }
        public string ApertureDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

	    public string Section => panelDetail.WS?.Section;

	    /// <summary>
        /// Этаж
        /// </summary>
        public string Floor => panelDetail.WS?.Floor;

	    /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public RelayCommand Show { get; set; }

        private void OnShowExecute ()
        {
            panelDetail?.Show();
        }

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
