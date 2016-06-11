using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.RegystryPanel.Windows
{
    public class PanelViewModel : ObservableObject
    {
        private Panel panel;

        public PanelViewModel (Panel panel)
        {
            
        }

        /// <summary>
        /// Марка панели из атрибута
        /// </summary>
        public string MarkAtr
        {
            get { return panel.Mark; }            
        }
        /// <summary>
        /// Марка по формуле
        /// </summary>
        public string MarkByFormula
        {
            get { return panel.MarkDb; }            
        }
        /// <summary>
        /// Наличие в базе
        /// </summary>
        public string ExistsInBase
        {
            get
            {
                return panel.IsNew ? "Нет" : "Да";
            }
        }
        /// <summary>
        /// Имя блока
        /// </summary>
        public string IsCorrectBlockName
        {
            get
            {
                return panel.IsCorrectBlockName ? "Ок" : "Ошибка";
            }
        }
        /// <summary>
        /// Группа
        /// </summary>
        public string Group
        {
            get { return panel.ItemGroup; }            
        }
        /// <summary>
        /// Длина
        /// </summary>
        public short? Length
        {
            get { return panel.Lenght; }            
        }
        /// <summary>
        /// Высота
        /// </summary>
        public short? Height
        {
            get { return panel.Height; }
        }
        /// <summary>
        /// Ширина
        /// </summary>
        public short? Thickness
        {
            get { return panel.Thickness; }
        }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork
        {
            get { return panel.Formwork; }
        }
        /// <summary>
        /// Балкон
        /// </summary>
        public string BalconyDoor
        {
            get { return panel.BalconyDoor; }
        }
        /// <summary>
        /// Подрезка
        /// </summary>
        public string BalconyCut
        {
            get { return panel.BalconyCut; }
        }
        /// <summary>
        /// Электрика
        /// </summary>
        public string Electrics
        {
            get { return panel.Electrics; }
        }
        /// <summary>
        /// Вес
        /// </summary>
        public float? Weight
        {
            get { return panel.Weight; }
        }
        /// <summary>
        /// Объем
        /// </summary>
        public float? Volume
        {
            get { return panel.Volume; }
        }
    }
}
