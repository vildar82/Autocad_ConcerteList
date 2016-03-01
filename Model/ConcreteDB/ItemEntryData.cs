using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;

namespace Autocad_ConcerteList.ConcreteDB
{
   internal class ItemEntryData
   {
      public List<string> SeriesList;
      public string ItemGroup;
      public short? Length;
      public short? Height;
      public short? Thicknes;
      public short? Formwork;
      public string BalconyDoor;
      public string BalconyCut;
      public short? FormworkMirror;
      public short? ElectricsIdx;
      public string ElectricsPrefix;
      public float? Weight;
      public float? Volume;
      private readonly ItemForm _sourceForm;

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
         Length = short.TryParse(_sourceForm.lenghtTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         Height = short.TryParse(_sourceForm.heigthTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         Thicknes = short.TryParse(_sourceForm.thicknessTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         Formwork = short.TryParse(_sourceForm.formworkTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         BalconyDoor = string.IsNullOrWhiteSpace(_sourceForm.balconyDoorComboBox.Text) ? null : _sourceForm.balconyDoorComboBox.Text;
         BalconyCut = string.IsNullOrWhiteSpace(_sourceForm.balconycutComboBox.Text) ? null : _sourceForm.balconycutComboBox.Text;
         FormworkMirror = short.TryParse(_sourceForm.formworkmirrorTextBox.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         ElectricsIdx = short.TryParse(_sourceForm.electricsIdxTextBox1.Text, out tmpIntValue) ? (short?)tmpIntValue : null;
         ElectricsPrefix = _sourceForm.electricsIdxTextBox2.Text;
         Weight = float.TryParse(_sourceForm.weigthTextBox.Text, out tmpFloatValue) ? (float?)tmpFloatValue : null;
         Volume = float.TryParse(_sourceForm.volumeTextBox.Text, out tmpFloatValue) ? (float?)tmpFloatValue : null;

         //refresh markLabel
         var formulaTableList = new I_C_FormulaTableAdapter();
         var actualFormula = formulaTableList.GetData().LastOrDefault();
         if (actualFormula != null)
         {
            var formulaValue = actualFormula["FormulaValue"].ToString();
            _sourceForm.markLabel.Text = string.Format("Марка: <{0}>", getMark(formulaValue));
         }
      }

      public string getMark(string formulaValue)
      {
         var result = string.Empty;

         return result;
      }

      public bool IsCheck
      {
         get
         {
            RefreshItemEntryData();

            //set error
            _sourceForm.errorValueProvider.Clear();
            if (SeriesList.Count == 0) _sourceForm.errorValueProvider.SetError(_sourceForm.seriesCheckedListBox, "Error!");
            if (string.IsNullOrEmpty(ItemGroup)) _sourceForm.errorValueProvider.SetError(_sourceForm.itemGroupComboBox, "Error!");
            if (Length == null) _sourceForm.errorValueProvider.SetError(_sourceForm.lenghtTextBox, "Error!");
            if (Height == null) _sourceForm.errorValueProvider.SetError(_sourceForm.heigthTextBox, "Error!");
            if (Thicknes == null) _sourceForm.errorValueProvider.SetError(_sourceForm.thicknessTextBox, "Error!");
            if (Weight == null) _sourceForm.errorValueProvider.SetError(_sourceForm.weigthTextBox, "Error!");
            if (Volume == null) _sourceForm.errorValueProvider.SetError(_sourceForm.volumeTextBox, "Error!");

            //return result
            return !string.IsNullOrEmpty(ItemGroup) &&
                    Length != null &&
                    Height != null &&
                    Thicknes != null &&
                    Weight != null &&
                    Volume != null;
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
