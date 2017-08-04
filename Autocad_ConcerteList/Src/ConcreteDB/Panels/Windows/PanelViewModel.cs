using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.ConcreteDB.Panels.Windows
{
    public class PanelViewModel : ObservableObject
    {
        private static readonly Brush BadValueColor = new SolidColorBrush(Colors.Red);
        private static readonly Brush NewPanelColor = new SolidColorBrush(Colors.LawnGreen);

        private readonly PanelsBaseView model;
        internal IPanel panel;
        private PanelDetailViewModel _selectedPanel;        

        /// <summary>
        /// Панели на чертеже
        /// </summary>
        public ObservableCollection<PanelDetailViewModel> PanelsInModel { get; set; }
        
        public PanelDetailViewModel SelectedPanel {
            get => _selectedPanel;
	        set {
                _selectedPanel = value;
                _selectedPanel.Show.Execute(null);
            }
        }        

        public PanelViewModel (IPanel panel, List<IPanel> blocks, PanelsBaseView model)
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
        public string MarkAtr {
            get => panel.Mark;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Марка по формуле
        /// </summary>
        public string MarkByFormula {
            get => panel.MarkByFormula;
	        set => RaisePropertyChanged();
        }
        public Brush MarkByFormulaBackground {
            get => MarkAtr?.Replace(" ", "")== MarkByFormula?.Replace(" ", "") ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string MarkByFormulaDesc {
            get {
                if (MarkAtr == MarkByFormula)
                {
                    return $"Марки совпадают, из атр и по формуле.";
                }
                else
                {
                    return $"Марки отличаются атр={MarkAtr}, по формуле={MarkByFormula}.";
                }
            }
            set => RaisePropertyChanged();
        }

        /// <summary>
        /// Наличие в базе
        /// </summary>
        public string ExistsInBase {
            get {
                if (panel.IsNew == null)
                {
                    return "Не определено";
                }
                return panel.IsNew.Value ? "Новая" : "Есть";
            }
            set => RaisePropertyChanged();
        }
        public Brush ExistsInBaseBackground {
            get => (panel.IsNew== null || !panel.IsNew.Value) ? null : NewPanelColor;
	        set => RaisePropertyChanged();
        }
        
        /// <summary>
        /// Группа
        /// </summary>
        public string Group {
            get => panel.Item_group;
	        set => RaisePropertyChanged();
        }
        public Brush GroupBackground {
            get => panel.IsItemGroupOk ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string GroupDesc {
            get => panel.ItemGroupDesc;
	        set => RaisePropertyChanged();
        }

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
                        RaisePropertyChanged();
                    }
                }                
            }
        }        

        public Brush LengthBackground {
            get => panel.IsLengthOk ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string LengthDesc {
            get => panel.LengthDesc;
	        set => RaisePropertyChanged();
        }

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get => panel.Height;
	        set {
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
                        RaisePropertyChanged();
                    }
                }                
            }
        }
        public Brush HeightBackground {
            get => panel.IsHeightOk ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string HeightDesc {
            get => panel.HeightDesc;
	        set => RaisePropertyChanged();
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
                        RaisePropertyChanged();
                    }
                }                
            }
        }
        public Brush ThicknessBackground {
            get => panel.IsThicknessOk ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string ThicknessDesc {
            get => panel.ThicknessDesc;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork {
            get => panel.Formwork;
	        set => RaisePropertyChanged();
        }

        public string MountElement {
            get => panel.Mount_element;
	        set => RaisePropertyChanged();
        }

        public string Prong {
            get => panel.Prong;
	        set => RaisePropertyChanged();
        }

        /// <summary>
        /// Балкон
        /// </summary>
        public string BalconyDoor {
            get => panel.Balcony_door;
	        set => RaisePropertyChanged();
        }
        public string BalconyDoorDesc {
            get => "Балкон " + panel.Balcony_door_modif?.Side;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Подрезка
        /// </summary>
        public string BalconyCut {
            get => panel.Balcony_cut;
	        set => RaisePropertyChanged();
        }
        public string BalconyCutDesc {
            get => "Подрезка " + panel.Balcony_cut_modif?.Measure + " " + panel.Balcony_cut_modif?.Side;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Электрика
        /// </summary>
        public string Electrics {
            get => panel.Electrics;
	        set => RaisePropertyChanged();
        }
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get => panel.Weight;
	        set {
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
                        RaisePropertyChanged();
                    }
                }                
            }
        }
        public Brush WeightBackground {
            get => panel.IsWeightOk ? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string WeightDesc {
            get => panel.WeightDesc;
	        set => RaisePropertyChanged();
        }

        /// <summary>
        /// Проем
        /// </summary>
        public string Aperture {
            get => panel.Aperture;
	        set {
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
                    RaisePropertyChanged();
                }
            }
        }

        public int? StepHeightIndex { get => panel.Step_height;
	        set => RaisePropertyChanged();
        }
        public string StepHeightDesc {
            get => panel.Step_height_modif?.Measure?.ToString();
	        set => RaisePropertyChanged();
        }
        public int? StepCount { get => panel.Steps;
	        set => RaisePropertyChanged();
        }
        public int? StepFirst { get => panel.First_step;
	        set => RaisePropertyChanged();
        }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Warning {
            get => panel.Warning;
	        set => RaisePropertyChanged();
        }

        public string ErrorStatus {
            get => panel.ErrorStatus.ToString("D");
	        set => RaisePropertyChanged();
        }
        public Brush ErrorStatusBackground {
            get => panel.ErrorStatus == ErrorStatusEnum.None? null : BadValueColor;
	        set => RaisePropertyChanged();
        }
        public string ErrorStatusDesc {
            get => panel.GetErrorStatusDesc();
	        set => RaisePropertyChanged();
        }

        private void UpdateRow ()
        {
            this.ErrorStatus = null;
            this.ErrorStatusBackground = null;
            this.ErrorStatusDesc = null;
            this.ExistsInBase = null;
            this.ExistsInBaseBackground = null;
            this.HeightBackground = null;
            this.HeightDesc = null;
            this.LengthBackground = null;
            this.LengthDesc = null;
            this.MarkAtr = null;
            this.MarkByFormula = null;
            this.MarkByFormulaBackground = null;
            this.MarkByFormulaDesc = null;
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
