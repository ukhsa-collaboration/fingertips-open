using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using Dundas.Charting.WebControl;
using PholioVisualisation.Cache;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class PracticeChart : BaseChart
    {
        public PracticeChartParameters Parameters { get; set; }

        protected override bool AreParametersValid { get { return Parameters.AreValid; } }

        private Series seriesAll = new Series();
        private Series seriesPractice = new Series();
        private Series seriesChildPractices = new Series();
        private PracticeValueCollection collection;

        protected override void WriteNoChartMessage(MemoryStream stream)
        {
            Image image = new Bitmap(Parameters.Width, 140);

            Font font = new Font("Verdana", 20, FontStyle.Italic, GraphicsUnit.Pixel);

            Color color = Color.DarkGray;
            Point atpoint = new Point(image.Width / 2, image.Height - 24);
            SolidBrush brush = new SolidBrush(color);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            g.Clear(Color.White);
            g.DrawString("Earlier data is not available for both indicators", font, brush, atpoint, sf);
            g.Dispose();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        protected override void BuildChart()
        {
            Chart.Width = Parameters.Width;
            Chart.Height = Parameters.Height;

            PracticeAxisRepository repo = new PracticeAxisRepository();
            PracticeAxis axis1 = repo.GetAxis(Parameters.IndicatorId1,
               Parameters.GroupId1, Parameters.SexId1, Parameters.DataPointOffset);
            PracticeAxis axis2 = repo.GetAxis(Parameters.IndicatorId2,
              Parameters.GroupId2, Parameters.SexId2, Parameters.DataPointOffset);

            collection = new PracticeValueCollectionBuilder
            {
                AreaCode = Parameters.AreaCode,
                ParentAreaCode = Parameters.ParentAreaCode,
                DataPointOffset = Parameters.DataPointOffset,
                TopAreaCode = AreaCodes.England,
                IndicatorId1 = Parameters.IndicatorId1,
                IndicatorId2 = Parameters.IndicatorId2,
                GroupId1 = Parameters.GroupId1,
                GroupId2 = Parameters.GroupId2,
                SexId1 = Parameters.SexId1,
                SexId2 = Parameters.SexId2,
                AgeId1 = Parameters.AgeId1,
                AgeId2 = Parameters.AgeId2,
            }.Build(axis1, axis2);

            if (axis1 == null)
            {
                repo.AddAxis(Parameters.IndicatorId1,
                    Parameters.GroupId1, Parameters.SexId1, Parameters.DataPointOffset, collection.PracticeAxis1);
            }

            if (axis2 == null)
            {
                repo.AddAxis(Parameters.IndicatorId2,
                    Parameters.GroupId2, Parameters.SexId2, Parameters.DataPointOffset, collection.PracticeAxis2);
            }


            AddChartArea();

            if (collection.IsData)
            {
                AddLimitsAndTitles();

                AddPoints(seriesAll, "All", InequalityRangeColor);

                AddPoints(seriesChildPractices, "Child", NonRagDarkBlue);

                if (collection.IsPracticeData)
                {
                    AddPoints(seriesPractice, "Practice", Color.Black);
                    seriesPractice.MarkerStyle = MarkerStyle.Cross;
                    seriesPractice.MarkerSize = 10;
                }

                AddData();
            }
            else
            {
                WasBuiltOk = false;
            }
        }

        private void AddLimitsAndTitles()
        {
            SetLimitsAndTitle(ChartArea.AxisY, collection.PracticeAxis1);
            SetLimitsAndTitle(ChartArea.AxisX, collection.PracticeAxis2);
        }

        private static void SetLimitsAndTitle(Axis axis, PracticeAxis practiceAxis)
        {
            axis.Minimum = practiceAxis.Limits.Min;
            axis.Maximum = practiceAxis.Limits.Max;
            axis.Title = practiceAxis.Title;
        }

        protected void AddChartArea()
        {
            ChartArea.Name = "Default";
            Chart.ChartAreas.Add(ChartArea);

            // Y Axis
            ChartArea.AxisY.TitleFont = LabelFont;
            ChartArea.AxisY.MajorGrid.LineColor = GridColor;
            ChartArea.AxisY.MajorTickMark.Enabled = false;
            ChartArea.AxisY.LabelStyle.Font = LabelFont;
            ChartArea.AxisY.LineColor = GridColor;

            // X Axis
            ChartArea.AxisX.TitleFont = LabelFont;
            ChartArea.AxisX.MajorGrid.LineColor = GridColor;
            ChartArea.AxisX.MajorTickMark.Enabled = false;
            ChartArea.AxisX.LabelStyle.Font = LabelFont;
            ChartArea.AxisX.LineColor = GridColor;
            ChartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

            // Chart area
            ChartArea.Position.Auto = true;
        }

        protected void AddPoints(Series series, string name, Color color)
        {
            series.Name = name;
            series.Type = SeriesChartType.Point;
            series.ShowInLegend = false;
            series.MarkerStyle = MarkerStyle.Circle;
            series.XValueType = ChartValueTypes.Double;
            series.YValueType = ChartValueTypes.Double;
            series.Color = color;
            Chart.Series.Add(series);
        }

        private void AddData()
        {
            Dictionary<string, float> data1 = collection.PracticeAxis1.IndicatorData;

            Dictionary<string, float> data2 = collection.PracticeAxis2.IndicatorData;


            // England
            foreach (string areaCode in data1.Keys)
            {
                AddPoint(seriesAll, data1, areaCode, data2);
            }

            // PCT or CCG
            IEnumerable<string> childAreas = collection.ChildAreaCodes;
            if (childAreas != null)
            {
                foreach (var childArea in childAreas)
                {
                    AddPoint(seriesChildPractices, data1, childArea, data2);
                }
            }

            // Practice
            if (collection.IsPracticeData)
            {
                seriesPractice.Points.AddXY(collection.PracticeValue2, collection.PracticeValue1);
            }
        }

        private static void AddPoint(Series series, Dictionary<string, float> data1, string key, 
            Dictionary<string, float> data2)
        {
            if (data1.ContainsKey(key) && data2.ContainsKey(key))
            {
                series.Points.AddXY(data2[key], data1[key]);
            }
        }
    }
}