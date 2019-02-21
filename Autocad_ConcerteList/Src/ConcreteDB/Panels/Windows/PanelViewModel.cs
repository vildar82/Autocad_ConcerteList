using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using NetLib.WPF;
using NetLib.WPF.Data;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelViewModel : BaseModel
    {
        private static readonly Brush BadValueColor = new SolidColorBrush(Colors.Red);
        private static readonly Brush NewPanelColor = new SolidColorBrush(Colors.LawnGreen);

        private readonly PanelsBaseView model;
        internal IIPanel panel;
        private PanelDetailViewModel _selectedPanel;

        /// <summary>
        /// Панели на чертеже
        /// </summary>
        public ObservableCollection<PanelDetailViewModel> PanelsInModel { get; set; }

        public PanelDetailViewModel SelectedPanel
        {
            get => _selectedPanel;
            set
            {
                _selectedPanel = value;
                _selectedPanel.Show.Execute(null);
            }
        }

        public PanelViewModel (IIPanel panel, List<IIPanel> blocks, PanelsBaseView model)
        {
            this.model = model;
            this.panel = panel;
            PanelsInModel = new ObservableCollection<PanelDetailViewModel>();
            foreach (var item in blocks)
            {
                PanelsInModel.Add(new PanelDetailViewModel(item, panel));
            }

            Show = new RelayCommand(OnShowExecute);
        }

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>
        public string MarkAtr => panel.Mark;

        /// <summary>
        /// Марка по формуле
        /// </summary>
        public string MarkByFormula => panel.MarkByFormula;

        public Brush MarkByFormulaBackground => MarkAtr?.Replace(" ", "")== MarkByFormula?.Replace(" ", "") ? null : BadValueColor;

        public string MarkByFormulaDesc
        {
            get
            {
                if (MarkAtr == MarkByFormula)
                {
                    return $"Марки совпадают, из атр и по формуле.";
                }
                else
                {
                    return $"Марки отличаются атр={MarkAtr}, по формуле={MarkByFormula}.";
                }
            }
        }

        /// <summary>
        /// Наличие в базе
        /// </summary>
        public string ExistsInBase
        {
            get
            {
                if (panel.IsNew == null)
                {
                    return "Не определено";
                }

                return panel.IsNew.Value ? "Новая" : "Есть";
            }
        }

        public Brush ExistsInBaseBackground => (panel.IsNew == null || !panel.IsNew.Value) ? null : NewPanelColor;

        /// <summary>
        /// Группа
        /// </summary>
        public string Group => panel.Item_group;

        public Brush GroupBackground => panel.IsItemGroupOk ? null : BadValueColor;

        public string GroupDesc => panel.ItemGroupDesc;

        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get => panel.Length;
            set {
                if (value != null)
                {
                    var len = panel.UpdateLength(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());
                    if (len != null)
                    {
                        // обновление поля в деталях
                        foreach (var item in PanelsInModel)
                        {
                            item.Length = len;
                            item.UpdateCheckPanel();
                        }

                        // Обновление всех значений
                        UpdateRow();
                    }
                }
            }
        }

        public Brush LengthBackground
        {
            get => panel.IsLengthOk ? null : BadValueColor;
            set { }
        }

        public string LengthDesc
        {
            get => panel.LengthDesc;
            set => panel.LengthDesc = value;
        }

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height
        {
            get => panel.Height;
            set
            {
                if (value != null)
                {
                    var height = panel.UpdateHeight(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());
                    if (height != null)
                    {
                        // обновление поля в деталях
                        foreach (var item in PanelsInModel)
                        {
                            item.Height = height;
                            item.UpdateCheckPanel();
                        }

                        // Обновление всех значений
                        UpdateRow();
                    }
                }
            }
        }
        public Brush HeightBackground
        {
            get => panel.IsHeightOk ? null : BadValueColor;
            set { }
        }

        public string HeightDesc
        {
            get => panel.HeightDesc;
            set => panel.HeightDesc = value;
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get => panel.Thickness;
            set {
                if (value != null)
                {
                    var thick = panel.UpdateThickness(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());
                    if (thick != null)
                    {
                        // обновление поля в деталях
                        foreach (var item in PanelsInModel)
                        {
                            item.Thickness = thick;
                            item.UpdateCheckPanel();
                        }

                        // Обновление всех значений
                        UpdateRow();
                    }
                }
            }
        }
        public Brush ThicknessBackground
        {
            get => panel.IsThicknessOk ? null : BadValueColor;
            set { }
        }

        public string ThicknessDesc
        {
            get => panel.ThicknessDesc;
            set => panel.ThicknessDesc = value;
        }

        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork => panel.Formwork;

        public string MountElement => panel.Mount_element;

        public string Prong => panel.Prong;

        /// <summary>
        /// Балкон
        /// </summary>
        public string BalconyDoor => panel.Balcony_door;

        public string BalconyDoorDesc => "Балкон " + panel.Balcony_door_modif?.Side;

        /// <summary>
        /// Подрезка
        /// </summary>
        public string BalconyCut => panel.Balcony_cut;

        public string BalconyCutDesc => "Подрезка " + panel.Balcony_cut_modif?.Measure + " " + panel.Balcony_cut_modif?.Side;

        /// <summary>
        /// Электрика
        /// </summary>
        public string Electrics => panel.Electrics;

        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight
        {
            get => panel.Weight;
            set
            {
                if (value != null)
                {
                    var wei = panel.UpdateWeight(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());
                    if (wei != null)
                    {
                        // обновление поля в деталях
                        foreach (var item in PanelsInModel)
                        {
                            item.Weight = wei;
                            item.UpdateCheckPanel();
                        }

                        // Обновление всех значений
                        UpdateRow();
                    }
                }
            }
        }

        public Brush WeightBackground
        {
            get => panel.IsWeightOk ? null : BadValueColor;
            set { }
        }

        public string WeightDesc
        {
            get => panel.WeightDesc;
            set => panel.WeightDesc = value;
        }

        /// <summary>
        /// Проем
        /// </summary>
        public string Aperture
        {
            get => panel.Aperture;
            set
            {
                if (value != null)
                {
                    var res = panel.UpdateAperture(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());

                    // обновление поля в деталях
                    foreach (var item in PanelsInModel)
                    {
                        item.Aperture = res;
                        item.UpdateCheckPanel();
                    }

                    UpdateRow();
                }
            }
        }

        public int? StepHeightIndex => panel.Step_height;

        public string StepHeightDesc => panel.Step_height_modif?.Measure?.ToString();
        public int? StepCount => panel.Steps;

        public int? StepFirst => panel.First_step;

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Warning
        {
            get => panel.Warning;
            set => panel.Warning = value;
        }

        public string ErrorStatus
        {
            get => panel.ErrorStatus.ToString("D");
            set { }
        }

        public Brush ErrorStatusBackground
        {
            get => panel.ErrorStatus == ErrorStatusEnum.None? null : BadValueColor;
            set { }
        }

        public string ErrorStatusDesc
        {
            get => panel.GetErrorStatusDesc();
            set { }
        }

        private void UpdateRow ()
        {
            this.ErrorStatus = null;
            this.ErrorStatusBackground = null;
            this.ErrorStatusDesc = null;
            this.HeightBackground = null;
            this.HeightDesc = null;
            this.LengthBackground = null;
            this.LengthDesc = null;
            this.ThicknessBackground = null;
            this.ThicknessDesc = null;
            this.Warning = null;
            this.WeightBackground = null;
            this.WeightDesc = null;
            this.Aperture = null;
            model.CheckState();
        }

        /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public RelayCommand Show { get; set; }

        /// <summary>
        /// Исключение
        /// </summary>
        public ICommand Delete { get { return new RelayCommand(DeleteRow, () => true); } }
        public void DeleteRow ()
        {
            model.DeleteRow(this);
            model.CheckState();
        }

        private void OnShowExecute()
        {
            panel?.Show();
        }
    }
}
