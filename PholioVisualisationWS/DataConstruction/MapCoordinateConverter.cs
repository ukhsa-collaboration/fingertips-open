using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public static class MapCoordinateConverter
    {
        public static LatitudeLongitude GetLatitudeLongitude(EastingNorthing e)
        {
            return e != null ?
                ConvertEastingNorthingToLatitudeLongitude(e.Easting, e.Northing) :
                null;
        }

        public static LatitudeLongitude ConvertEastingNorthingToLatitudeLongitude(double east, double north)
        {
            //metres in, degrees out

            double K0 = 0.9996012717; // grid scale factor on central meridean
            double OriginLat = 49.0;
            double OriginLong = -2.0;
            double OriginX = 400000; // 400 kM
            double OriginY = -100000; // 100 kM
            double a = 6377563.396; // Airy Spheroid
            double b = 6356256.910;

            double e2;
            double n1;
            double n2;
            double n3;
            double OriginNorthings;

            // compute interim values
            a = a * K0;
            b = b * K0;

            n1 = (a - b) / (a + b);
            n2 = n1 * n1;
            n3 = n2 * n1;

            double lat = OriginLat * Math.PI / 180.0; // to radians                                                    


            e2 = (a * a - b * b) / (a * a);  // first eccentricity

            OriginNorthings = b * lat + b * (n1 * (1.0 + 5.0 * n1 * (1.0 + n1) / 4.0) * lat
                    - 3.0 * n1 * (1.0 + n1 * (1.0 + 7.0 * n1 / 8.0)) * Math.Sin(lat) * Math.Cos(lat)
                    + (15.0 * n1 * (n1 + n2) / 8.0) * Math.Sin(2.0 * lat) * Math.Cos(2.0 * lat)
                    - (35.0 * n3 / 24.0) * Math.Sin(3.0 * lat) * Math.Cos(3.0 * lat));

            double northing = north - OriginY;
            double easting = east - OriginX;

            double nu, phid, phid2, t2, t, q2, c, s, nphid, dnphid; // temps
            double nu2, nudivrho, invnurho, rho, eta2;


            /* Evaluate M term: latitude of the northing on the centre meridian */

            northing += OriginNorthings;

            phid = northing / (b * (1.0 + n1 + 5.0 * (n2 + n3) / 4.0)) - 1.0;
            phid2 = phid + 1.0;

            while (Math.Abs(phid2 - phid) > 1E-6)
            {
                phid = phid2;
                nphid = b * phid + b * (n1 * (1.0 + 5.0 * n1 * (1.0 + n1) / 4.0) * phid
                        - 3.0 * n1 * (1.0 + n1 * (1.0 + 7.0 * n1 / 8.0)) * Math.Sin(phid) * Math.Cos(phid)
                        + (15.0 * n1 * (n1 + n2) / 8.0) * Math.Sin(2.0 * phid) * Math.Cos(2.0 * phid)
                        - (35.0 * n3 / 24.0) * Math.Sin(3.0 * phid) * Math.Cos(3.0 * phid));

                dnphid = b * ((1.0 + n1 + 5.0 * (n2 + n3) / 4.0) - 3.0 * (n1 + n2 + 7.0 * n3 / 8.0) * Math.Cos(2.0 * phid)
                        + (15.0 * (n2 + n3) / 4.0) * Math.Cos(4 * phid) - (35.0 * n3 / 8.0) * Math.Cos(6.0 * phid));

                phid2 = phid - (nphid - northing) / dnphid;
            }

            c = Math.Cos(phid);
            s = Math.Sin(phid);
            t = Math.Tan(phid);
            t2 = t * t;
            q2 = easting * easting;


            nu2 = (a * a) / (1.0 - e2 * s * s);
            nu = Math.Sqrt(nu2);

            nudivrho = a * a * c * c / (b * b) - c * c + 1.0;

            eta2 = nudivrho - 1;

            rho = nu / nudivrho;

            invnurho = ((1.0 - e2 * s * s) * (1.0 - e2 * s * s)) / (a * a * (1.0 - e2));

            lat = phid - t * q2 * invnurho / 2.0 + (q2 * q2 * (t / (24 * rho * nu2 * nu) * (5 + (3 * t2) + eta2 - (9 * t2 * eta2))));

            double lon = (easting / (c * nu))
                - (easting * q2 * ((nudivrho + 2.0 * t2) / (6.0 * nu2)) / (c * nu))
                + (q2 * q2 * easting * (5 + (28 * t2) + (24 * t2 * t2)) / (120 * nu2 * nu2 * nu * c));

            return new LatitudeLongitude
            {
                Latitude = Math.Round(lat * 180.0 / Math.PI, 5, MidpointRounding.AwayFromZero),
                Longitude = Math.Round((lon * 180.0 / Math.PI) + OriginLong, 5, MidpointRounding.AwayFromZero),
            };
        }
    }
}
