using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.ConcreteDB.Formula;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;

namespace Autocad_ConcerteList.ConcreteDB
{
    public class ItemEntryData
    {
        public string Mark { get; set; }
        public List<string> SeriesList;
        public string ItemGroup { get; set; }
        public short? Lenght { get; set; }
        public short? Height { get; set; }
        public short? Thickness { get; set; }
        public short? Formwork { get; set; }
        public string BalconyDoor { get; set; }
        public string BalconyCut { get; set; }
        public short? FormworkMirror { get; set; }
        public short? ElectricsIdx { get; set; }
        public string ElectricsPrefix { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }
        private readonly ItemForm _sourceForm;
        private string actualFormulaValue;

        public ItemEntryData(ItemForm sourceForm)
        {
            _sourceForm = sourceForm;
        }

        public void RefreshItemEntryData()
        {
            short tmpIntValue;
            float tmpFloatValue;

            //get data
            SeriesList = (from DataRowView checkedItem in _sourceForm.seriesCheckedListBox.CheckedItems
                          select checkedItem["Series"].ToString()).ToList();
            ItemGroup = ((DataRowView)_sourceForm.itemGroupComboBox.SelectedItem).Row["ItemGroup"].ToString();
            Lenght = short.TryParse(_sourceForm.lenghtTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            Height = short.TryParse(_sourceForm.heigthTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            Thickness = short.TryParse(_sourceForm.thicknessTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            Formwork = short.TryParse(_sourceForm.formworkTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            BalconyDoor = string.IsNullOrWhiteSpace(_sourceForm.balconyDoorComboBox.Text) ? null : _sourceForm.balconyDoorComboBox.Text;
            BalconyCut = string.IsNullOrWhiteSpace(_sourceForm.balconycutComboBox.Text) ? null : _sourceForm.balconycutComboBox.Text;
            FormworkMirror = short.TryParse(_sourceForm.formworkmirrorTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            ElectricsIdx = short.TryParse(_sourceForm.electricsIdxTextBox1.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
            ElectricsPrefix = _sourceForm.electricsIdxTextBox2.Text;
            Weight = float.TryParse(_sourceForm.weigthTextBox.Text, out tmpFloatValue) ? (float?)tmpFloatValue : null;
            Volume = float.TryParse(_sourceForm.volumeTextBox.Text, out tmpFloatValue) ? (float?)tmpFloatValue : null;

            //refresh markLabel           

            if (actualFormulaValue == null)
            {
                var formulaTableList = new I_C_FormulaTableAdapter();
                ConcerteDataSet.I_C_FormulaRow actualFormulaRow = formulaTableList.GetData().Last();
                actualFormulaValue = actualFormulaRow["FormulaValue"].ToString();
            }
            if (actualFormulaValue != null)
            {
                Mark = getMark(actualFormulaValue);
                _sourceForm.markLabel.Text = $"Марка: {Mark}";
            }
        }

        public string getMark(string formulaValue)
        {
            var result = string.Empty;
            ParserFormula parserFormula = new ParserFormula(formulaValue, this);
            parserFormula.Parse();
            result = parserFormula.Result;
            return result;
        }

        public bool IsCheck
        {
            get
            {
                RefreshItemEntryData();
                bool res = true;                
                _sourceForm.errorValueProvider.Clear();

                // Проверка серии
                if (SeriesList.Count == 0)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.seriesCheckedListBox, "Error!");
                }

                // Проверка группы изделия
                if (string.IsNullOrEmpty(ItemGroup))
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.itemGroupComboBox, "Error!");
                }

                // Проверка длины
                if (Lenght == null)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.lenghtTextBox, "Error!");
                }

                // Проверка высоты
                if (Height == null)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.heigthTextBox, "Error!");
                }

                // Проверка толщины
                if (Thickness == null)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.thicknessTextBox, "Error!");
                }

                // Проверка массы
                if (Weight == null)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.weigthTextBox, "Error!");
                }

                // Проверка объема
                if (Volume == null)
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.volumeTextBox, "Error!");
                }

                // Проверка марки
                if (string.IsNullOrEmpty(Mark))
                {
                    res = false;
                    _sourceForm.errorValueProvider.SetError(_sourceForm.markLabel, "Error!");
                }
                                
                return res;
            }
        }

        public string Electrics
        {
            get
            {
                return ElectricsIdx == null ? null : string.Format("{0}{1}", ElectricsIdx, ElectricsPrefix);
            }
        }
    }
}
