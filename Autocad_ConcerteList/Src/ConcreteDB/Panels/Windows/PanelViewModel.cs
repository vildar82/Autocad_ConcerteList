using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        
        private Panel panel;
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

        public PanelViewModel (Panel item)
        {
            panel = item;
            PanelsInModel = new ObservableCollection<PanelDetailViewModel>();
            PanelsInModel.Add(new PanelDetailViewModel(item, item));            
        }

        public PanelViewModel (Panel panel, List<Panel> blocks)
        {
            this.panel = panel;            
            PanelsInModel = new ObservableCollection<PanelDetailViewModel>();
            foreach (var item in blocks)
            {
                PanelsInModel.Add(new PanelDetailViewModel(item, panel));
            }            
        }

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>               
        public string MarkAtr {
            get { return panel.Mark; }
        }
        /// <summary>
        /// Марка по формуле
        /// </summary>
        public string MarkByFormula {
            get { return panel.MarkByFormula; }
            set { RaisePropertyChanged(); }
        }
        public Brush MarkByFormulaBackground {
            get {
                return MarkAtr.Replace(" ", "")== MarkByFormula.Replace(" ", "") ? null : BadValueColor;
            }
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
        }
        public Brush ExistsInBaseBackground {
            get { return (panel.IsNew== null || !panel.IsNew.Value) ? null : NewPanelColor; }
        }
        
        /// <summary>
        /// Группа
        /// </summary>
        public string Group {
            get { return panel.ItemGroup; }
        }
        public Brush GroupBackground {
            get { return panel.IsItemGroupOk ? null : BadValueColor; }
        }
        public string GroupDesc {
            get { return panel.ItemGroupDesc; }
        }

        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get { return panel.Lenght; }
            set {                
                var len = panel.UpdateLength(value, this.PanelsInModel.Select(s=>s.PanelDetail).ToList());
                if (len != null)
                {
                    // обновление поля в деталях
                    foreach (var item in PanelsInModel)
                    {
                        item.Length = len;
                    }
                    // Обновление марки по формуле
                    panel.UpdateMarkByFormula();
                    MarkByFormula = panel.MarkByFormula;
                }
                RaisePropertyChanged();
            }
        }
        public Brush LengthBackground {
            get { return panel.IsLengthOk ? null : BadValueColor; }
        }
        public string LengthDesc {
            get { return panel.LengthDesc; }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public short? Height {
            get { return panel.Height; }
        }
        public Brush HeightBackground {
            get { return panel.IsHeightOk ? null : BadValueColor; }
        }
        public string HeightDesc {
            get { return panel.HeightDesc; }
        }
        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness {
            get { return panel.Thickness; }
        }
        public Brush ThicknessBackground {
            get { return panel.IsThicknessOk ? null : BadValueColor; }
        }
        public string ThicknessDesc {
            get { return panel.ThicknessDesc; }
        }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork {
            get { return panel.Formwork; }
        }
        /// <summary>
        /// Балкон
        /// </summary>
        public string BalconyDoor {
            get { return panel.BalconyDoor; }
        }
        /// <summary>
        /// Подрезка
        /// </summary>
        public string BalconyCut {
            get { return panel.BalconyCut; }
        }
        /// <summary>
        /// Электрика
        /// </summary>
        public string Electrics {
            get { return panel.Electrics; }
        }
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight {
            get { return panel.Weight; }
        }
        public Brush WeightBackground {
            get { return panel.IsWeightOk ? null : BadValueColor; }
        }
        public string WeightDesc {
            get { return panel.WeightDesc; }
        }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Warning {
            get { return panel.Warning; }
        }

        public string ErrorStatus {
            get { return panel.ErrorStatus.ToString("D"); }
        }
        public Brush ErrorStatusBackground {
            get { return panel.ErrorStatus == ErrorStatusEnum.None? null : BadValueColor; }
        }
        public string ErrorStatusDesc {
            get {
                 return panel.GetErrorStatusDesc();
            }
        }        

        /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public ICommand Show { get { return new RelayCommand(() => panel.Show(), () => panel.CanShow()); } }
    }
}
