using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CoreDataSetExportFormatter
    {
        private LookUpManager _lookUpManager;
        private CoreDataSet _coreDataSet;
        public string LowerCI95;
        public string UpperCI95;
        public string LowerCI99_8;
        public string UpperCI99_8;

        public CoreDataSetExportFormatter(LookUpManager lookUpManager, CoreDataSet coreDataSet)
        {
            _lookUpManager = lookUpManager;
            _coreDataSet = coreDataSet;

            if (_coreDataSet.Are95CIsValid)
            {
                LowerCI95 = _coreDataSet.LowerCI95.ToString();
                UpperCI95 = _coreDataSet.UpperCI95.ToString();
            }
            else
            {
                LowerCI95 = string.Empty;
                UpperCI95 = string.Empty;
            }

            if (_coreDataSet.Are99_8CIsValid)
            {
                LowerCI99_8 = _coreDataSet.LowerCI99_8.ToString();
                UpperCI99_8 = _coreDataSet.UpperCI99_8.ToString();
            }
            else
            {
                LowerCI99_8 = string.Empty;
                UpperCI99_8 = string.Empty;
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