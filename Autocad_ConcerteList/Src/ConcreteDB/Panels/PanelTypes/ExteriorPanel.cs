﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Наружная стеновая панель
    /// </summary>
    public class ExteriorPanel : Panel
    {
        public ExteriorPanel(MarkPart markPart, BlockReference blRef, string blName) : base(markPart, blRef, blName)
        {
        }

        public override short GetGab(string nameGabRu, short value, double factor)
        {
            if (value == 3018 && nameGabRu == LengthNameRu)
            {
                return 31;
            }
            return base.GetGab(nameGabRu, value, factor);
        }

        public override string CorrectHeightParseValue(string valueString)
        {
            if (Height == 3018)
            {
                return  "31";                
            }
            return valueString;
        }
    }
}
