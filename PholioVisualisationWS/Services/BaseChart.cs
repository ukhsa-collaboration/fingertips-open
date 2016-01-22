
using System;
using System.Drawing;
using System.IO;
using Dundas.Charting.WebControl;

namespace PholioVisualisation.Services
{
    public abstract class BaseChart : IDisposable
    {
        protected static Font LabelFont = new Font("Verdana", 8F, FontStyle.Regular, GraphicsUnit.Point);
        protected static Color GridColor = Color.FromArgb(64, 64, 64, 64);
        protected static Color NonRagDarkBlue = Color.FromArgb(85, 85, 230);
        protected static Color NonRagLightBlue = Color.FromArgb(119, 119, 183);
        protected static Color ErrorBarColor = Color.FromArgb(55, 55, 55);
        protected static Color InequalityRangeColor = Color.FromArgb(160, 160, 160);

        protected Chart Chart = new Chart();
        protected ChartArea ChartArea = new ChartArea();
        protected bool WasBuiltOk = true;

        public MemoryStream GetChart()
        {
            MemoryStream stream = new MemoryStream();
            if (AreParametersValid)
            {
                Chart.SuppressExceptions = true;
                BuildChart();
                if (WasBuiltOk)
                {
                    Chart.RenderType = RenderType.BinaryStreaming;
                    Chart.Save(stream);
                }
                else
                {
                    WriteNoChartMessage(stream);
                }
            }
            return stream;
        }

        protected abstract void BuildChart();

        protected abstract bool AreParametersValid { get; }

        protected virtual void WriteNoChartMessage(MemoryStream stream)
        {
        }

        public void Dispose()
        {
            Chart.Dispose();
        }
    }

}