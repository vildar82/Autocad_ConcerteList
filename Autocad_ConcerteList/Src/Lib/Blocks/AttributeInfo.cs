﻿using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Lib.Blocks
{
    /// <summary>
    /// Описание атрибута
    /// Для AttributeDefinition или AttributeReference
    /// </summary>
    public class AttributeInfo
    {
        public string Tag { get; set; }
        public string Text { get; set; }
        public ObjectId IdAtr { get; set; }
        public bool IsAtrDefinition { get; set; }

        /// <summary>
        /// DBText - должен быть или AttributeDefinition или AttributeReference
        /// иначе исключение ArgumentException
        /// </summary>      
        public AttributeInfo(DBText attr)
        {
            var attdef = attr as AttributeDefinition;
            if (attdef != null)
            {
                Tag = attdef.Tag;
                IsAtrDefinition = true;
            }
            else
            {
                var attref = attr as AttributeReference;
                if (attref != null)
                {
                    Tag = attref.Tag;
                }
                else
                {
                    throw new ArgumentException("requires an AttributeDefintion or AttributeReference");
                }
            }
            Text = attr.TextString;
            IdAtr = attr.Id;
        }

        public AttributeInfo(AttributeReference atrRef)
        {
            Tag = atrRef.Tag;
            Text = atrRef.TextString;
            IdAtr = atrRef.Id;
        }

        public static List<AttributeInfo> GetAttrDefs(ObjectId idBtr)
        {
            var resVal = new List<AttributeInfo>();

            if (!idBtr.IsNull)
            {
                using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
                {
                    foreach (var idEnt in btr)
                    {
                        using (var attrDef = idEnt.Open(OpenMode.ForRead, false, true) as AttributeDefinition)
                        {                            
                            if (attrDef != null && attrDef.Visible)
                            {
                                var attrDefInfo = new AttributeInfo(attrDef);
                                resVal.Add(attrDefInfo);
                            }
                        }
                    }
                }
            }
            return resVal;
        }

        public static List<AttributeInfo> GetAttrRefs(BlockReference blRef)
        {
            var resVal = new List<AttributeInfo>();
            if (blRef?.AttributeCollection != null)
            {
                foreach (ObjectId idAttrRef in blRef.AttributeCollection)
                {
                    if (!idAttrRef.IsValidEx()) continue;
                    using (var atrRef = idAttrRef.Open(OpenMode.ForRead, false, true) as AttributeReference)
                    {
                        if (atrRef.Visible)
                        {
                            var ai = new AttributeInfo(atrRef);
                            resVal.Add(ai);
                        }
                    }
                }
            }
            return resVal;
        }        
    }
}
