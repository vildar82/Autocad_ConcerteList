using System;
using System.ComponentModel;

namespace Autocad_ConcerteList.ConcreteDB.Options
{
   [Serializable]
   public class Options
   {
      private static Options _instance;
      public static Options Instance
      {
         get
         {
            if (_instance == null)
            {
               _instance = Load();
            }
            return _instance;
         }
      }

      private Options() { }     
      
      /// <summary>
      /// Имя блока рабочей области.
      /// </summary>
      [Category("Рабочая область")]
      [Description("Имя блока рабочей области.")]
      [DefaultValue("rab_obl")]
      public string WorkspaceBlockName { get; set; } = "rab_obl";

      /// <summary>
      /// Атрибут секции
      /// </summary>
      [Category("Рабочая область")]
      [Description("Атрибут секции.")]
      [DefaultValue("СЕКЦИЯ")]
      public string WorkspaceAttrSection { get; set; } = "СЕКЦИЯ";

      /// <summary>
      /// Атрибут этажа
      /// </summary>
      [Category("Рабочая область")]
      [Description("Атрибут этажа.")]
      [DefaultValue("ЭТАЖ")]
      public string WorkspaceAttrFloor { get; set; } = "ЭТАЖ";

      /// <summary>
      /// Атрибут марки
      /// </summary>
      [Category("Панели")]
      [Description("Атрибут марки.")]
      [DefaultValue("МАРКА")]
      public string PanelAttrMark { get; set; } = "МАРКА";

      /// <summary>
      /// Атрибут покраски
      /// </summary>
      [Category("Панели")]
      [Description("Атрибут покраски.")]
      [DefaultValue("ПОКРАСКА")]
      public string PanelAttrColorIndex { get; set; } = "ПОКРАСКА";

      public static Options Load()
      {
         return new Options();
      }     
   }
}