using ImageStackerConsole.Alignment;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageStackerConsole.Stacking
{
    class StandardStack : IStackingMethod
    {
        public RGBImage StackImages(AlignedImages alignedImages, LoadingBar LoadingBar)
        {


           // Bitmap b = TransformImage(Bitmap bmp, float angle);

                throw new NotImplementedException();
        }



        public static Bitmap TransformImage(Bitmap transformMe, OffsetParameters offset)
        {

            int canvasWidth  = (int) (offset.Zoom * transformMe.Width  * 2.0 );
            int canvasHeight = (int) (offset.Zoom * transformMe.Height * 2.0 );

            var targetCanvas = new Bitmap( canvasWidth, canvasHeight );

            using (Graphics g = Graphics.FromImage(targetCanvas))
            {              
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Console.WriteLine($"Interpolation mode: {g.InterpolationMode}");


                SolidBrush greenBrush = new SolidBrush(Color.Green);
                g.FillRectangle(greenBrush, new Rectangle(0, 0, canvasWidth, canvasHeight));

                g.ScaleTransform((float) offset.Zoom, (float) offset.Zoom);
                g.RotateTransform((float) ((0.01) * offset.Theta * 180.0 / Math.PI) );
                //g.TranslateClip(-2000, -1000);
                g.TranslateClip(200, 100);
                g.DrawImage(transformMe, 5, 5, (int) (transformMe.Width * offset.Zoom), (int) (transformMe.Height * offset.Zoom));

                g.Save();
            }


            return targetCanvas;
        }



    }
}
