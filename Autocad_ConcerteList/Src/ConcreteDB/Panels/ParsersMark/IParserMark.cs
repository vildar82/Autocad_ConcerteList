using AcadLib.Errors;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public interface IParserMark
    {
        string BalconyCut { get; set; }
        string BalconyDoor { get; set; }
        string Electrics { get; set; }
        Error Error { get; set; }
        short? Formwork { get; set; }
        int GroupIndexClass { get; set; }
        short? Height { get; set; }
        string ItemGroup { get; set; }
        short? Length { get; set; }
        string MarkInput { get; }
        string MarkWoGroupClassIndex { get; set; }
        string MountIndex { get; set; }
        string ProngIndex { get; set; }
        int? StepFirstHeight { get; set; }
        int? StepHeightIndex { get; set; }
        int? StepsCount { get; set; }
        short? Thickness { get; set; }

        void Parse();
        void UpdateGab(string gabKey);
    }
}