using System;
using System.Collections.Generic;
using Autocad_ConcerteList.ConcreteDB.Panels;

namespace Autocad_ConcerteList.ConcreteDB.Formula
{
    public static class FormulaFactory
    {
        // Внутрянки
	    private static readonly Func<FormulaParameters, IIPanel, string> FormulaLHT_FE = (f, p) =>
            $"{p.Item_group} {p.GetLengthMarkPart(f.LengthFactor)}.{p.GetHeightMarkPart(f.HeightFactor)}.{p.GetThicknessMarkPart(f.ThicknessFactor)}-{p.Formwork}-{p.Electrics}";
        // Наружки
	    private static readonly Func<FormulaParameters, IIPanel, string> FormulaLHT_FBPE = (f, p) =>
            $"{p.Item_group} {p.GetLengthMarkPart(f.LengthFactor)}.{p.GetHeightMarkPart(f.HeightFactor)}.{p.GetThicknessMarkPart(f.ThicknessFactor)}-{p.Formwork}{p.Balcony_door}{p.Balcony_cut}-{p.Electrics}";        
        // Плиты
	    private static readonly Func<FormulaParameters, IIPanel, string> FormulaLH_FE = (f, p) =>
            $"{p.Item_group} {p.GetLengthMarkPart(f.LengthFactor)}.{p.GetHeightMarkPart(f.HeightFactor)}-{p.Formwork}-{p.Electrics}";
        // ОЛ
	    private static readonly Func<FormulaParameters, IIPanel, string> FormulaHL_FE = (f, p) =>
            $"{p.Item_group} {p.GetHeightMarkPart(f.HeightFactor)}.{p.GetLengthMarkPart(f.LengthFactor)}-{p.Formwork}-{p.Electrics}";

        /// <summary>
        /// Key - Срия_Тип панели. 
        /// </summary>
        private static readonly Dictionary<string, FormulaItem> dictFormules = new Dictionary<string, FormulaItem>
        {            
            // внутрянки            
            { $"{PanelSeria.PIK1}_{PanelTypeEnum.WallInner}", new FormulaItem(FormulaLHT_FE, new FormulaParameters(10, 100, 10),"LHT") },            
	        // внутрянки (3-значная высота (см))           
	        { $"{PanelSeria.PIK2}_{PanelTypeEnum.WallInner}", new FormulaItem(FormulaLHT_FE, new FormulaParameters(10, 10, 10),"LHT") },            
            // наружки
            { $"{PanelSeria.PIK1}_{PanelTypeEnum.WallOuter}", new FormulaItem(FormulaLHT_FBPE, new FormulaParameters(100, 100, 10),"LHT") },
            { $"{PanelSeria.PIK2}_{PanelTypeEnum.WallOuter}", new FormulaItem(FormulaLHT_FBPE, new FormulaParameters(10, 10, 10),"LHT") },
	        { $"{PanelSeria.PIK1}_{PanelTypeEnum.WallOuterFreeze}", new FormulaItem(FormulaLHT_FBPE, new FormulaParameters(100, 100, 10),"LHT") },
			{ $"{PanelSeria.PIK2}_{PanelTypeEnum.WallOuterFreeze}", new FormulaItem(FormulaLHT_FBPE, new FormulaParameters(10, 100, 10),"LHT") },            
            // плиты
            { $"{PanelSeria.PIK1}_{PanelTypeEnum.Slab}", new FormulaItem(FormulaLH_FE, new FormulaParameters(10, 10, 0),"LH") },            
            // Плиты лоджий
            { $"{PanelSeria.PIK1}_{PanelTypeEnum.SlabLoggia}", new FormulaItem(FormulaLH_FE, new FormulaParameters(100, 100, 0),"LH") },            
            // ОЛ
            { $"{PanelSeria.PIK1}_{PanelTypeEnum.OL}", new FormulaItem(FormulaHL_FE, new FormulaParameters(100, 100, 0),"HL") },
        };

        public static FormulaItem GetFormula(PanelSeria panelSeria, PanelTypeEnum panelType)
        {            
            var key = $"{panelSeria}_{panelType}";
            dictFormules.TryGetValue(key, out FormulaItem formulaResult);
            return formulaResult;
        }
    }
}
