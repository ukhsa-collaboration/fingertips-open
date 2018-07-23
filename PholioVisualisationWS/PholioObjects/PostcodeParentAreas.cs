namespace PholioVisualisation.PholioObjects
{
    public class PostcodeParentAreas
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public int Easting { get; set; }
        public int Northing { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string AreaCode6 { get; set; }

        /// <summary>
        /// Subregion
        /// </summary>
        public string AreaCode46 { get; set; }

        /// <summary>
        /// District & UA
        /// </summary>
        public string AreaCode101 { get; set; }

        /// <summary>
        /// County & UA
        /// </summary>
        public string AreaCode102 { get; set; }

        /// <summary>
        /// PHE Centres 2013
        /// </summary>
        public string AreaCode103 { get; set; }

        /// <summary>
        /// PHE Centres 2015
        /// </summary>
        public string AreaCode104 { get; set; }

        /// <summary>
        /// CCG Pre Apr 2017
        /// </summary>
        public string AreaCode153 { get; set; }

        public int PlaceTypeWeighting
        {
            get { return 2; }
        }
    }
}