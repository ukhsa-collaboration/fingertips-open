using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface ICoreDataSetValidator
    {
        void Validate(IList<CoreDataSet> coreDataSets);
        string GetErrorMessage();
    }

    public class CoreDataSetValidator : ICoreDataSetValidator
    {
        private readonly IAreasReader _areasReader;
        private readonly IPholioReader _pholioReader;
        private IList<CoreDataSet> _coreDataSets;

        private string AgesNotInLive { get; set; }
        private string AreaCodesNotInLive { get; set; }

        public CoreDataSetValidator(IAreasReader areasReader, IPholioReader pholioReader)
        {
            _areasReader = areasReader;
            _pholioReader = pholioReader;
        }

        public void Validate(IList<CoreDataSet> coreDataSets)
        {
            _coreDataSets = coreDataSets;
            ValidateAges();
            ValidateAreaCodes();

            
        }

        private void ValidateAges()
        {
            IList<int> ageIds = _pholioReader.GetAllAgeIds();

            var sb = new StringBuilder();
            foreach (CoreDataSet coreDataSet in _coreDataSets)
            {
                if (!ageIds.Contains(coreDataSet.AgeId))
                {
                    sb.Append(coreDataSet.AgeId + " ");
                }
            }

            AgesNotInLive = sb.ToString();
        }

        private void ValidateAreaCodes()
        {
            var areaCodes = _coreDataSets.Select(x => x.AreaCode).ToList();

            IList<string> areaCodeThatDoNotExist = _areasReader.GetAreaCodesThatDoNotExist(areaCodes);

            var sb = new StringBuilder();
            foreach (var areaCode in areaCodeThatDoNotExist)
            {
                    sb.Append(areaCode + " ");
            }

            AreaCodesNotInLive = sb.ToString();
        }

        public string GetErrorMessage()
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrWhiteSpace(AgesNotInLive))
            {
                errorMessage = string.Format("The following age IDs do not exist: {0}", AgesNotInLive.Trim());
            }

            if (!string.IsNullOrWhiteSpace(AreaCodesNotInLive))
            {
                errorMessage = string.Format("The following area codes do not exist: {0}", AreaCodesNotInLive);
            }

            return errorMessage;
        }
    }
}
