using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageStackerConsole
{
    public class RGBImage : ICloneable
    {

        public byte[,,] RGBArray { get; } // [y,x,c] - color is B,G,R
        public int ImageHeight { get { return RGBArray.GetLength(0); } }
        public int ImageWidth { get { return RGBArray.GetLength(1); } }

        public RGBImage(string path)
        {

            // Uses the method: https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap.lockbits
            // Locking creates a buffer in unmanaged memory which is faster to read and manipulate.
            // Lock bitmap to BitmapData class, copy into the bitmapByteArray, then copy into correct shape array

            Bitmap imgBitmap = new Bitmap(path); // TODO - try/catch

            Rectangle rect = new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height);
            BitmapData bmpData = imgBitmap.LockBits(rect, ImageLockMode.ReadOnly, imgBitmap.PixelFormat);
                        
            RGBArray = new byte[imgBitmap.Height, imgBitmap.Width, 3];

            byte[] bitmapByteArray = new byte[Math.Abs(bmpData.Stride) * imgBitmap.Height];
            Marshal.Copy(bmpData.Scan0, bitmapByteArray, 0, Math.Abs(bmpData.Stride) * imgBitmap.Height);
            
            
            for (int y = 0; y < imgBitmap.Height ; y++)
            {                
                for (int x = 0; x < imgBitmap.Width; x++)
                {
                    int byteIndex = y * bmpData.Stride + 3 * x;
                    RGBArray[y, x, 0] = bitmapByteArray[byteIndex    ];  // B
                    RGBArray[y, x, 1] = bitmapByteArray[byteIndex + 1];  // G
                    RGBArray[y, x, 2] = bitmapByteArray[byteIndex + 2];  // R
                }
            }

            imgBitmap.UnlockBits(bmpData);

        }

        public RGBImage(byte[,,] array)
        {
            RGBArray = array;
        }

        public object Clone()
        {
            byte[,,] arrayClone = (byte[,,]) this.RGBArray.Clone();
            return new RGBImage(arrayClone);
        }

        public void SaveToDisk(String path)
        {

            try
            {
                int ByteArrayLength = ImageHeight * ImageWidth * 4;

                Byte[] ImageData = new Byte[ByteArrayLength];

                int BytePosition = 0;
                for (int y = 0; y < ImageHeight; y++)
                {
                    for (int x = 0; x < ImageWidth; x++)
                    {
                       
                        ImageData[BytePosition++] = RGBArray[y, x, 0];
                        ImageData[BytePosition++] = RGBArray[y, x, 1];
                        ImageData[BytePosition++] = RGBArray[y, x, 2];
                        ImageData[BytePosition++] = 255; // alpha

                    }
                }

                Bitmap Img = new Bitmap(ImageWidth, ImageHeight, ImageWidth*4,
                                     System.Drawing.Imaging.PixelFormat.Format32bppRgb,
                                     Marshal.UnsafeAddrOfPinnedArrayElement(ImageData, 0));

                Img.Save(path);

            }
            catch (ArgumentException e)
            {
                Console.WriteLine("EXCEPTION TRYING GET BITMAP");
                Console.WriteLine(e.GetBaseException());
                
            }            
        }

        public byte[,] GetGreyscaleArray()
        {
            byte[,] greyscale = new byte[ImageHeight, ImageWidth];

            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    greyscale[y,x] = (byte) ((RGBArray[y, x, 0] + RGBArray[y, x, 1] + RGBArray[y, x, 2]) / 3); // Rounds towards zero as an integer
                }
            }
            return greyscale;
        }

        public void MakeGreenCross(int x, int y)
        {
            // Prevent out of bounds exception
            if (x < 10 || x > ImageWidth - 10 || y < 10 || y > ImageHeight - 10)
            {
                return;
            }

            // Modify the underlying color array to have a green cross.
            for (int i = -7; i <= 7; i++)
            {

                // On y axis
                RGBArray[y, x + i, 0] = 0;
                RGBArray[y, x + i, 1] = 255;
                RGBArray[y, x + i, 2] = 0;

                // One off y axis to thicken
                RGBArray[y + 1, x + i, 0] = 0;
                RGBArray[y + 1, x + i, 1] = 255;
                RGBArray[y + 1, x + i, 2] = 0;

                // x-axis
                RGBArray[y + i, x + 1, 0] = 0;
                RGBArray[y + i, x + 1, 1] = 255;
                RGBArray[y + i, x + 1, 2] = 0;
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            try
            {
                double dx = (x2 - x1);
                double dy = (y2 - y1);
                double distance = Math.Sqrt( dx * dx + dy * dy);

                dx = dx/distance;
                dy = dy/distance;

                int t = 0;
                int x;
                int y;
                while (t < distance)
                {
                    x = (int) (x1 + t * dx);
                    y = (int) (y1 + t * dy);
                    
                    RGBArray[y, x, 0] = (byte) (color       % 255);
                    RGBArray[y, x, 1] = (byte) (color/256   % 255);
                    RGBArray[y, x, 2] = (byte) (color/65536 % 255);

                    t++;
                }                
            }
            catch {
                Console.WriteLine("Bounds Check");
            }
        
        }


    }
}