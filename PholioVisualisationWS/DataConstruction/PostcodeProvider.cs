using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    ///     This is required because reading all the postcodes at once may cause
    ///     OutOfMemory exception to be thrown.
    /// </summary>
    public class PostcodeProvider
    {
        private IAreasReader areasReader;
        private char index = 'a';

        public PostcodeProvider(IAreasReader areasReader)
        {
            this.areasReader = areasReader;
        }

        public bool AreMorePostcodes
        {
            get { return index <= 'z'; }
        }

        public IList<PostcodeParentAreas> GetNextPostcodeBatch()
        {
            return areasReader.GetAllPostCodeParentAreasStartingWithLetter((index++).ToString());
        }
    }
}