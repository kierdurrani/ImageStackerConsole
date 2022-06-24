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

            // Generate an array of alignedPixels
            byte[][,,] alignedPixels = new byte[alignedImages.ImagePaths.Length][,,];

            for (int i = 0; i < alignedImages.ImagePaths.Length; i++ )
            {
                OffsetParameters currentOffset = alignedImages.OffsetParameterTable[0, i];

                Bitmap transformedImage = TransformImage(new Bitmap(alignedImages.ImagePaths[i]), currentOffset);

                alignedPixels[i] = new RGBImage(transformedImage).RGBArray;
            }

            int final_height = alignedPixels[0].GetLength(0);
            int final_width  = alignedPixels[0].GetLength(1);
            int imgs = alignedImages.ImagePaths.Length;
            // Calculate final brightnesses
            Console.WriteLine("------------- STACKING ALL IMAGES --------------");
            byte[,,] finalBrightness = new byte[final_height, final_width, 3];

            int b = 0;
            int a = 0;
            int im = 0;
            //try
            //{

                for (int j = 0; j < final_height - 10; j++)
                {
                Console.WriteLine(j);
                    b = j;
                    for (int i = 0; i < final_width - 10; i++)
                    {
                        a = i;
                        int sum0 = 0;
                        int sum1 = 0;
                        int sum2 = 0;

                        for (int img = 0; img < alignedPixels.Length; img++)
                        {
                            im = img;
                            sum0 += alignedPixels[img][j, i, 0];
                            sum1 += alignedPixels[img][j, i, 1];
                            sum2 += alignedPixels[img][j, i, 2];
                        }
                        finalBrightness[j, i, 0] = (byte)(sum0 / imgs);
                        finalBrightness[j, i, 1] = (byte)(sum1 / imgs);
                        finalBrightness[j, i, 2] = (byte)(sum2 / imgs);
                    }
                }
            //}
            //catch { 
            //    Console.WriteLine($"a,b,im - {a},{b},{im}"); 
            //}
            return new RGBImage(finalBrightness);
        }

        

        public static Bitmap TransformImage(Bitmap transformMe, OffsetParameters offset)
        {

            int canvasWidth = (int)(offset.Zoom * transformMe.Width * 1.0);
            int canvasHeight = (int)(offset.Zoom * transformMe.Height * 1.0);
            Console.WriteLine($"Zoom Error: {offset.ZoomError}");

            var targetCanvas = new Bitmap(canvasWidth, canvasHeight);

            using (Graphics g = Graphics.FromImage(targetCanvas))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Give green background
                SolidBrush greenBrush = new SolidBrush(Color.Green);
                g.FillRectangle(greenBrush, new Rectangle(0, 0, canvasWidth, canvasHeight));

                // Translate first then rotate (order doesnt commute)
                g.TranslateTransform((float) offset.X, (float)offset.Y);
                g.ScaleTransform((float)offset.Zoom, (float)offset.Zoom);
                g.RotateTransform((float) (offset.Theta * 180.0 / Math.PI));
 
                Console.WriteLine(offset.GetStringRepresentation());
                g.DrawImage(transformMe, 0, 0, transformMe.Width, transformMe.Height);

                g.Save();
            }


            return targetCanvas;
        }


    }
}
