using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class XyPointListBuilder
    {
        public List<XyPoint> XyPoints = new List<XyPoint>();

        public XyPointListBuilder(IList<CoreDataSet> xData, IList<CoreDataSet> yData)
        {
            if (yData.Count == xData.Count)
            {
                for (int i = 0; i < yData.Count; i++)
                {
                    AddPoint(xData[i].Value, yData[i].Value);
                }
            }
        }

        public XyPointListBuilder(IList<int> xData, IList<CoreDataSet> yData)
        {
            if (yData.Count == xData.Count)
            {
                for (int i = 0; i < yData.Count; i++)
                {
                    AddPoint(xData[i], yData[i].Value);
                }
            }
        }

        private void AddPoint(double x, double y)
        {
            XyPoints.Add(new XyPoint
            {
                X = x,
                Y = y
            });
        }
    }
}