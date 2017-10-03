using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB.Formula;

namespace Autocad_ConcerteList.Src.ConcreteDB
{
    /// <summary>
    /// Изделие
    /// </summary>
    public interface IIPanel : IEquatable<IIPanel>, IComparable<IIPanel>
    {
        FormulaItem Formula { get; set; }
        short? LengthMark { get; set; }
        short? HeightMark { get; set; }
        short? ThicknessMark { get; set; }
        PanelSeria PanelSeria { get; set; }
        PanelTypeEnum PanelType { get; set; }

        string Mark { get; }      
        // Имя группы полное
        string Item_group { get; }
        /// <summary>
        /// Имя группы для поиска в базе (без индекса -Б1.2-)
        /// </summary>
        string Item_groupForSearchInBD { get; }
        // имя группы без нового класса - по которому ищется группа в базе
        string ItemGroupWoClassNew { get; }
        short? Length { get; set; }
        short? Height { get; set; }
        short? Thickness { get; set; }        
        short? Formwork { get; }
        string Balcony_door { get; }
        string Balcony_cut { get; }
        //short? FormworkMirror { get; }
        string Electrics { get; }
        float? Weight { get; set; }
        float? Volume { get; set; }
        /// <summary>
        /// Закладная. Например: Д или пусто
        /// </summary>
        string Mount_element { get; }
        /// <summary>
        /// Зубец. Например: Г или пусто
        /// </summary>
        string Prong { get; }       

        /// <summary>
        /// Высота ступени - индекс - 1,2,3 (ЛМ-1.11-15)
        /// </summary>
        int? Step_height { get; }
        /// <summary>
        /// Кол. ступеней. - 11,12
        /// </summary>
        int? Steps { get; }
        /// <summary>
        /// Высота первой ступени
        /// </summary>
        int? First_step { get; }
        /// <summary>
        /// Что это наружная стеновая панель
        /// </summary>
        bool IsExteriorWall { get; }
        bool IsInnerWall { get; }
        bool HasErrors { get;  }
        bool IsWeightOk { get; set; }
        string BlName { get; set; }
        string Aperture { get; set; }
        Workspace WS { get; set; }
        string MarkByFormula { get; set; }
        bool? IsNew { get; set; }
        bool IsItemGroupOk { get; set; }
        string ItemGroupDesc { get; set; }
        bool IsLengthOk { get; set; }
        string LengthDesc { get; set; }
        bool IsHeightOk { get; set; }
        string HeightDesc { get; set; }
        bool IsThicknessOk { get; set; }
        string ThicknessDesc { get; set; }
        ModificatorDbo Balcony_door_modif { get; set; }
        ModificatorDbo Balcony_cut_modif { get; set; }
        string WeightDesc { get; set; }
        ModificatorDbo Step_height_modif { get; set; }
        string Warning { get; set; }
        ErrorStatusEnum ErrorStatus { get; set; }
        ObjectId IdBlRef { get; set; }
        ItemConstructionDbo DbItem { get; set; }
        ItemGroupDbo DbGroup { get; set; }
        string ColorMark { get; set; }
        int ItemConstructionId { get; set; }
        string MarkWoSpace { get; set; }
        bool IsIgnoreGab { get; set; }
        Point3d Position { get; set; }

        bool LengthHasProperty { get; set; }
        bool HeightHasProperty { get; set; }
        bool ThicknessHasProperty { get; set; }
        bool WeightHasProperty { get; set; }
        bool ApertureHasProperty { get; set; }

	    void Init();
        void Checks();
        void Show();
        short? UpdateLength(short? value, List<IIPanel> list);
        short? UpdateHeight(short? value, List<IIPanel> list);
        short? UpdateThickness(short? value, List<IIPanel> list);
        float? UpdateWeight(float? value, List<IIPanel> list);
        string UpdateAperture(string value, List<IIPanel> list);
        string GetErrorStatusDesc();
        void SetAtr(string tag, string value);
        void DefineItemGroup();
        string GetMarkWithColor();
        string GetHandMarkNoColor();
        string ParamsToString();     
        /// <summary>
        /// Определение габарита длины в марке по параметру длины и фактору длины.
        /// Запись в LengthMark
        /// </summary>        
        string GetLengthMarkPart(short lengthFactor);
        string GetHeightMarkPart(short heightFactor);
        string GetThicknessMarkPart(short thicknessFactor);
        //string CorrectLentghParseValue(short? valueString);
        //string CorrectHeightParseValue(short? valueString);
        //string CorrectThicknessParseValue(short? valueString);
    }
}
