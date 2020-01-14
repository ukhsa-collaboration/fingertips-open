using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.ProfileData.Repositories
{
    public interface ILookUpsRepository
    {
        IEnumerable<CategoryType> GetCategoryTypes();
        IEnumerable<Skin> GetSkins();
        IEnumerable<KeyColour> GetKeyColours();
        IEnumerable<Sex> GetSexes();
        IEnumerable<Age> GetAges();
        IEnumerable<Comparator> GetComparators();
        IEnumerable<YearType> GetYearTypes();
        IEnumerable<IndicatorValueType> GetIndicatorValueTypes();
        IEnumerable<ConfidenceIntervalMethod> GetConfidenceIntervalMethods();
        IEnumerable<Unit> GetUnits();
        IEnumerable<DenominatorType> GetDenominatorTypes();
    }
}