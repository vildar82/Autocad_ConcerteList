using System;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    using System.Windows.Media;
    using NetLib.WPF;
    using NetLib.WPF.Data;

    public class PanelDetailViewModel : BaseModel
    {
        private static readonly Brush BadValue = new SolidColorBrush(Colors.Brown);
        private static readonly Brush NoParameterColor = new SolidColorBrush(Colors.Gray);

        private readonly IIPanel panelDetail;
        private readonly IIPanel panelFirst;

        public PanelDetailViewModel (IIPanel itemDetail, IIPanel firstPanel)
        {
            panelDetail = itemDetail;
            panelFirst = firstPanel;
            Show = new RelayCommand(OnShowExecute);
        }

        public IIPanel PanelDetail => panelDetail;

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>
        public string MarkAtr
        {
            get => panelDetail.Mark;
            set { }
        }

        public Brush MarkAtrBackground
        {
            get => panelDetail.Mark == panelFirst.Mark ? null : BadValue;
            set { }
        }

        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName
        {
            get => panelDetail.BlName;
            set => panelDetail.BlName = value;
        }

        public Brush BlockNameBackground
        {
            get => panelDetail.BlName == panelFirst.BlName ? null : BadValue;
            set { }
        }

        /// <summary>
        /// Длина
        /// </summary>
        public short? Length
        {
            get => panelDetail.Length;
            set => panelDetail.Length = value;
        }

        public Brush LengthBackground
        {
            get
            {
                if (panelDetail.LengthHasProperty)
                    return panelDetail.Length == panelFirst.Length ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set { }
        }

        public string LengthDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height
        {
            get => panelDetail.Height;
            set => panelDetail.Height = value;
        }

        public Brush HeightBackground
        {
            get
            {
                if (panelDetail.HeightHasProperty)
                    return panelDetail.Height == panelFirst.Height ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set { }
        }

        public string HeightDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness
        {
            get => panelDetail.Thickness;
            set => panelDetail.Thickness = value;
        }

        public Brush ThicknessBackground
        {
            get
            {
                if (panelDetail.ThicknessHasProperty)
                    return panelDetail.Thickness == panelFirst.Thickness ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set { }
        }

        public string ThicknessDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight
        {
            get => panelDetail.Weight;
            set => panelDetail.Weight = value;
        }

        public Brush WeightBackground
        {
            get
            {
                if (panelDetail.WeightHasProperty)
                    return panelDetail.Weight == panelFirst.Weight ? null : BadValue;
                return NoParameterColor;
            }
            set { }
        }

        public string WeightDesc => panelDetail.ApertureHasProperty ? "" : "Нет атрибута";

        /// <summary>
        /// Проем
        /// </summary>
        public string Aperture
        {
            get => panelDetail.Aperture;
            set => panelDetail.Aperture = value;
        }

        public Brush ApertureBackground
        {
            get
            {
                if (panelDetail.ApertureHasProperty)
                    return panelDetail.Aperture == panelFirst.Aperture ? null : BadValue;
                else
                    return NoParameterColor;
            }
            set { }
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
