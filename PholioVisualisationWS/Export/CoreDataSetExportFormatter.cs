using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CoreDataSetExportFormatter
    {
        private LookUpManager _lookUpManager;
        private CoreDataSet _coreDataSet;
        public string LowerCI;
        public string UpperCI;

        public CoreDataSetExportFormatter(LookUpManager lookUpManager, CoreDataSet coreDataSet)
        {
            _lookUpManager = lookUpManager;
            _coreDataSet = coreDataSet;

            if (_coreDataSet.AreCIsValid)
            {
                LowerCI = _coreDataSet.LowerCI.ToString();
                UpperCI = _coreDataSet.UpperCI.ToString();
            }
            else
            {
                LowerCI = string.Empty;
                UpperCI = string.Empty;
            }
        }

        public string CategoryType
        {
            get
            {
                return _coreDataSet.CategoryTypeId != CategoryTypeIds.Undefined
                    ? _lookUpManager.GetCategoryTypeName(_coreDataSet.CategoryTypeId)
                    : "";
            }
        }

        public string Category
        {
            get
            {
                return _coreDataSet.CategoryId != CategoryTypeIds.Undefined
                        ? _lookUpManager.GetCategoryName(_coreDataSet.CategoryTypeId, _coreDataSet.CategoryId)
                        : "";
            }
        }

        public string Value
        {
            get
            {
                return _coreDataSet.IsValueValid
                    ? _coreDataSet.Value.ToString()
                    : "";
            }
        }

        public string Count
        {
            get
            {
                return _coreDataSet.IsCountValid
                    ? _coreDataSet.Count.ToString()
                    : "";
            }
        }

        public string Denominator
        {
            get
            {
                return _coreDataSet.IsDenominatorValid
                    ? _coreDataSet.Denominator.ToString()
                    : "";
            }
        }

        public string ValueNote
        {
            get
            {
                return _coreDataSet.ValueNoteId > 0
                    ? _lookUpManager.GetValueNoteText(_coreDataSet.ValueNoteId)
                    : "";
            }
        }
    }
}