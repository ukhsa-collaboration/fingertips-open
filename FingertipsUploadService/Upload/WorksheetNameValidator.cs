/* This class contains the functionality to validate the worksheet names for simple upload*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class WorksheetNameValidator : IWorksheetNameValidator
    {
        public bool ValidateBatch(List<string> worksheets)
        {
            return worksheets.Any(x => x.Equals(WorksheetNames.BatchPholio, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}