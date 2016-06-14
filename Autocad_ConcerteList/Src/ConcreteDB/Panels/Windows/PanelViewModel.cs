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
    public class PanelViewModel : ObservableObject
    {
        private static Brush BadValue = new SolidColorBrush(Colors.Red);

        private Panel panel;        
        /// <summary>
        /// Панели с такими паракметрами на чертеже
        /// </summary>
        public ObservableCollection<PanelViewModel> PanelsInModel { get; set; }        

        public PanelViewModel (Panel item)
        {
            panel = item;
            PanelsInModel = new ObservableCollection<PanelViewModel>();
            PanelsInModel.Add(this);
        }

        public PanelViewModel (IGrouping<Panel, Panel> items)
        {
            panel = items.Key;
            PanelsInModel = new ObservableCollection<PanelViewModel>();
            foreach (var item in items)
            {
                PanelsInModel.Add(new PanelViewModel(item));
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
        }
        /// <summary>
        /// Наличие в базе
        /// </summary>
        public string ExistsInBase {
            get {
                return panel.IsNew ? "Новая" : "Есть";
            }
        }
        /// <summary>
        /// Имя блока
        /// </summary>
        public string IsCorrectBlockName {
            get {
                return panel.IsCorrectBlockName ? "Ок" : "Ошибка";
            }
        }
        /// <summary>
        /// Группа
        /// </summary>
        public string Group {
            get { return panel.ItemGroup; }
        }
        /// <summary>
        /// Длина
        /// </summary>
        public short? Length {
            get { return panel.Lenght; }
        }
        public Brush LengthBackground {
            get { return panel.IsLengthOk ? null : BadValue; }
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
            get { return panel.IsHeightOk ? null : BadValue; }
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
            get { return panel.IsThicknessOk ? null : BadValue; }
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
            get { return panel.IsWeightOk ? null : BadValue; }
        }
        public string WeightDesc {
            get { return panel.WeightDesc; }
        }
        /// <summary>
        /// Объем
        /// </summary>
        public float? Volume {
            get { return panel.Volume; }
        }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Warning {
            get { return panel.Warning; }
        }

        /// <summary>
        /// Позиция
        /// </summary>               
        public string Position {
            get { return panel.Position.ToString(); }
        }        

        /// <summary>
        /// Команда - показать панель на чертеже
        /// </summary>
        public ICommand Show { get { return new RelayCommand(() => panel.Show(), () => panel.CanShow()); } }
    }
}
