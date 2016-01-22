
namespace PholioVisualisation.PholioObjects
{
    public class NearByAreas
    {
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Postcode { get; set; }
        public string AreaTypeID { get; set; }
        public double Distance { get; set; }
        public string DistanceValF { get; set; }
        public double Easting { get; set; }
        public double Northing { get; set; }
        public LatitudeLongitude LatLng { get; set; }
    }
}
