namespace Autocad_ConcerteList.ConcreteDB.Panels
{
    public enum PanelTypeEnum
    {
        Unknown,
        /// <summary>
        /// Внутрянки
        /// </summary>
        WallInner,
        /// <summary>
        /// Наружки
        /// </summary>
        WallOuter,
        /// <summary>
        /// НФ - фризовые (парапет)
        /// </summary>
        WallOuterFreeze,
        /// <summary>
        /// Плиты
        /// </summary>
        Slab,
        /// <summary>
        /// Плиты лоджий
        /// </summary>
        SlabLoggia,
        /// <summary>
        /// Вентблоки, дымоудаление
        /// </summary>
        VentBlock,
        /// <summary>
        /// ЛМ
        /// </summary>
        Stair,
        /// <summary>
        /// ОЛ
        /// </summary>
        OL,
        /// <summary>
        /// ЛП
        /// </summary>
        LP,
        /// <summary>
        /// ПЛ
        /// </summary>
        PL,
        /// <summary>
        /// Электроблоки
        /// </summary>
        EB
    }
}
