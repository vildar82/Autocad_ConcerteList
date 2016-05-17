using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;
using Autocad_ConcerteList.Model.ConcreteDB.Formula;

namespace Autocad_ConcerteList.Model.ConcreteDB
{
    public class ItemEntryData : iItem
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
        private string actualFormulaValue;                

        public string Electrics
        {
            get
            {
                return ElectricsIdx == null ? null : ElectricsIdx + ElectricsPrefix;
            }
        }
    }
}