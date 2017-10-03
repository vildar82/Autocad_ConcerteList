using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class MarkPart
    {
        public string Mark { get; set; }
        /// <summary>
        /// часть марки после группы
        /// </summary>
        public string MarkInputAfterGroup { get; set; }
        public string PartGroup { get; set; }
        public string GroupIndexClassNew { get; set; }        
        //public ItemGroupDbo DBGroup { get; set; }
        public string PartGab { get; set; }
        public string PartDop { get; set; }
        public PanelSeria PanelSeria { get; set; }
        public PanelTypeEnum PanelType { get; set; }        
        public string ItemGroupWoClassNew { get; set; }
        /// <summary>
        /// Новый индекс -Б1.2-, 
        /// </summary>
        public bool IsNewBIndex { get; set; }
        /// <summary>
        /// Значение нового индекса Б1.2
        /// </summary>
        public string NewBIndex { get; set; }

        public MarkPart (string mark)
        {
            Mark = mark;            
        }
    }
}
