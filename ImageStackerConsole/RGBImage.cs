using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageStackerConsole
{
    public class RGBImage
    {

        public byte[,,] RGBArray { get; } // [y,x,c] - color is B,G,R
        public int ImageHeight { get { return RGBArray.GetLength(0); } }
        public int ImageWidth { get { return RGBArray.GetLength(1); } }


        public RGBImage(string path)
        {
            Bitmap img = new Bitmap(path); // TODO - try/catch

            // Uses the method: https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap.lockbits
            // Locking creates a buffer in unmanaged memory which is faster to read and manipulate.
            // Lock bitmap to BitmapData class, copy into the PixelByteArray, then copy into correct shape array

            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            System.Console.WriteLine(img.PixelFormat);
            BitmapData bmpData = img.LockBits(rect, ImageLockMode.ReadOnly, img.PixelFormat);

            
            RGBArray = new byte[img.Height, img.Width, 3];

            byte[] PixelByteArray = new byte[Math.Abs(bmpData.Stride) * img.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, PixelByteArray, 0, Math.Abs(bmpData.Stride) * img.Height);

            int BytePosition = 0;
            for (int y = 0; y < img.Height ; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    RGBArray[y, x, 0] = PixelByteArray[BytePosition++];  // B
                    RGBArray[y, x, 1] = PixelByteArray[BytePosition++];  // G
                    RGBArray[y, x, 2] = PixelByteArray[BytePosition++];  // R
                }
            }

        }

        public RGBImage(byte[,,] array)
        {
            RGBArray = array;
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


    }
}