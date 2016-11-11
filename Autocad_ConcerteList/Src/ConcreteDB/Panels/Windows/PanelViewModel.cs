using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelViewModel : ObservableObject
    {
        private static Brush BadValueColor = new SolidColorBrush(Colors.Red);
        private static Brush NewPanelColor = new SolidColorBrush(Colors.LawnGreen);

        private PanelsBaseView model;
        internal Panel panel;
        private PanelDetailViewModel _selectedPanel;        

        /// <summary>
        /// Панели на чертеже
        /// </summary>
        public ObservableCollection<PanelDetailViewModel> PanelsInModel { get; set; }
        
        public PanelDetailViewModel SelectedPanel {
            get {
                return _selectedPanel;                
            }
            set {
                _selectedPanel = value;
                _selectedPanel.Show.Execute(null);
            }
        }        

        public PanelViewModel (Panel panel, List<Panel> blocks, PanelsBaseView model)
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
            get { return panel.Mark; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Марка по формуле
        /// </summary>
        public string MarkByFormula {
            get { return panel.MarkByFormula; }
            set { RaisePropertyChanged(); }
        }
        public Brush MarkByFormulaBackground {
            get {                return MarkAtr?.Replace(" ", "")== MarkByFormula?.Replace(" ", "") ? null : BadValueColor;            }
            set { RaisePropertyChanged(); }
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
            set { RaisePropertyChanged(); }
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
            set { RaisePropertyChanged(); }
        }
        public Brush ExistsInBaseBackground {
            get { return (panel.IsNew== null || !panel.IsNew.Value) ? null : NewPanelColor; }
            set { RaisePropertyChanged(); }
        }
        
        /// <summary>
        /// Группа
        /// </summary>
        public string Group {
            get { return panel.Item_group; }
            set { RaisePropertyChanged(); }
        }
        public Brush GroupBackground {
            get { return panel.IsItemGroupOk ? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string GroupDesc {
            get { return panel.ItemGroupDesc; }
            set { RaisePropertyChanged(); }
        }

        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get { return panel.Length; }
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
            get { return panel.IsLengthOk ? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string LengthDesc {
            get { return panel.LengthDesc; }
            set { RaisePropertyChanged(); }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get { return panel.Height; }
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
            get { return panel.IsHeightOk ? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string HeightDesc {
            get { return panel.HeightDesc; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get { return panel.Thickness; }
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
            get { return panel.IsThicknessOk ? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string ThicknessDesc {
            get { return panel.ThicknessDesc; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork {
            get { return panel.Formwork; }
            set { RaisePropertyChanged(); }
        }

        public string MountElement {
            get { return panel.Mount_element; }
            set { RaisePropertyChanged(); }
        }

        public string Prong {
            get { return panel.Prong; }
            set { RaisePropertyChanged(); }
        }

        /// <summary>
        /// Балкон
        /// </summary>
        public string BalconyDoor {
            get { return panel.Balcony_door; }
            set { RaisePropertyChanged(); }
        }
        public string BalconyDoorDesc {
            get { return "Балкон " + panel.Balcony_door_modif?.Side; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Подрезка
        /// </summary>
        public string BalconyCut {
            get { return panel.Balcony_cut; }
            set { RaisePropertyChanged(); }
        }
        public string BalconyCutDesc {
            get { return "Подрезка " + panel.Balcony_cut_modif?.Measure + " " + panel.Balcony_cut_modif?.Side; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Электрика
        /// </summary>
        public string Electrics {
            get { return panel.Electrics; }
            set { RaisePropertyChanged(); }
        }
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get { return panel.Weight; }
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
            get { return panel.IsWeightOk ? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string WeightDesc {
            get { return panel.WeightDesc; }
            set { RaisePropertyChanged(); }
        }

        /// <summary>
        /// Проем
        /// </summary>
        public string Aperture {
            get { return panel.Aperture; }
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

        public int? StepHeightIndex { get { return panel.Step_height; } set { RaisePropertyChanged(); } }
        public string StepHeightDesc {
            get { return panel.Step_height_modif?.Measure?.ToString(); }
            set { RaisePropertyChanged(); }
        }
        public int? StepCount { get { return panel.Steps; } set { RaisePropertyChanged(); } }
        public int? StepFirst { get { return panel.First_step; } set { RaisePropertyChanged(); } }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Warning {
            get { return panel.Warning; }
            set { RaisePropertyChanged(); }
        }

        public string ErrorStatus {
            get { return panel.ErrorStatus.ToString("D"); }
            set { RaisePropertyChanged(); }
        }
        public Brush ErrorStatusBackground {
            get { return panel.ErrorStatus == ErrorStatusEnum.None? null : BadValueColor; }
            set { RaisePropertyChanged(); }
        }
        public string ErrorStatusDesc {
            get { return panel.GetErrorStatusDesc();            }
            set { RaisePropertyChanged(); }
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
