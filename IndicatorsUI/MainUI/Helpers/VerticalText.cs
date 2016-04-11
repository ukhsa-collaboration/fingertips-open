using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Profiles.MainUI.Helpers
{
    public class VerticalText
    {
        private const string Colour = "#333333";
        private const string BackgroundColour = "#ffffff";
        private static readonly Font Font = new Font("Arial", 9.0f, FontStyle.Bold, GraphicsUnit.Point);

        private SizeF GetTextDimension(StringFormat format, string text)
        {
            const int width = 600;
            const int height = 300;
            
            // create initial image and graphics objects so that we have an instance of Graphics to determine text spacing
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            return graphics.MeasureString(text, Font, width, format);
        }

        public MemoryStream GetImage(string text)
        {
            Graphics bitmapGraphics = null;
            MemoryStream stream = null;

            // Define format
            StringFormat format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Near;
            format.FormatFlags = StringFormatFlags.DirectionVertical;

            try
            {
                Bitmap bitmap = null;

                SizeF dimensions = GetTextDimension(format, text);
                int width = Convert.ToInt32(Math.Ceiling(dimensions.Width));
                int height = Convert.ToInt32(Math.Ceiling(dimensions.Height));

                // New bitmap
                bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                bitmapGraphics = Graphics.FromImage(bitmap);

                // Background 
                Color background = ColorTranslator.FromHtml(BackgroundColour);
                bitmapGraphics.FillRectangle(new SolidBrush(background), 0, 0, width, height);

                // Transform
                bitmapGraphics.TranslateTransform(width, height);
                bitmapGraphics.RotateTransform(180.0F);

                // Text
                bitmapGraphics.DrawString(text, Font,
                    new SolidBrush(ColorTranslator.FromHtml(Colour)), 0, 0, format);

                // Save image
                stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return stream;
            }
            finally
            {
                // Tidy up
                if (bitmapGraphics != null)
                {
                    bitmapGraphics.Dispose();
                }
            }
        }
    }
}