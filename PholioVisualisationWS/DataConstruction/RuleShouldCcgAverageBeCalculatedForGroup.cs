using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
   public class RuleShouldCcgAverageBeCalculatedForGroup
   {
       public static int InvalidId = GroupIds.PracticeProfiles_IndicatorsForNeedsAssessment;

       public static bool Validates(Grouping grouping)
       {
           return grouping.GroupId != InvalidId;
       }
    }
}
