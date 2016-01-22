namespace PholioVisualisation.PholioObjects
{
    public interface IArea
    {
        string Code { get; }
        int? Sequence { get; }
        string Name { get; }
        string ShortName { get; }
        int AreaTypeId { get; }
        bool IsCcg { get; }
        bool IsGpDeprivationDecile { get; }
        bool IsShape { get; }
        bool IsCountry { get;}
        bool IsCountyAndUnitaryAuthorityDeprivationDecile { get; }
        bool IsGpPractice { get; }
    }
}