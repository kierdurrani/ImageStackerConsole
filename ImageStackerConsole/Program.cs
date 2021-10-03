using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageStackerConsole.AlignmnetMethods;

namespace ImageStackerConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //  Console.WriteLine("Path to file:");

            //  var ReadLine =  Console.ReadLine();
            //  Console.WriteLine("File Exists? " + System.IO.File.Exists(ReadLine));

            // "C:\Users\Kier\Pictures\ARSE.png"

            // try
            // {
                 var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                Image newImage = Image.FromFile("C:\\Users\\Kier\\Pictures\\ARSE.png");


               // Bitmap bitmap =  new Bitmap("C:\\Users\\Kier\\Pictures\\ARSE.png", true);
               // Console.WriteLine(bitmap.GetPixel(0, 0));

                int x = 100;
                int y = 100;


                //bitmap.SetPixel(x, y, Color.Green);
                //bitmap.SetPixel(x, y, Color.Green);
                //bitmap.SetPixel(x, y, Color.Green);
                //bitmap.SetPixel(x, y, Color.Green);

                // Graphics graphics = Graphics.FromImage(newImage);
                // graphics.DrawLine(new Pen(Color.Green), x - 5, y, x + 5, y);
                // graphics.DrawLine(new Pen(Color.Green), x, y - 5, x, y + 5);
                // newImage.Save("C:\\Users\\Kier\\Pictures\\ARSE2.png");

                // graphics.Save();

                RGBImage img = new RGBImage("C:\\Users\\Kier\\Pictures\\Andromeda galaxy.jpg");

                Console.WriteLine(img.RGBArray[1, 0, 0]);
                Console.WriteLine(img.RGBArray[1, 0, 1]);
                Console.WriteLine(img.RGBArray[1, 0, 2]);
                
                img.SaveToDisk("C:\\Users\\Kier\\Pictures\\output.png");

                img.GetGreyscaleArray();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            // }
            // catch 
            // {
            //     Console.WriteLine("ERROR");
            //     Console.ReadKey();
            // }

            // Create Point for upper-left corner of image.
            Point ulCorner = new Point(100, 100);

            Console.WriteLine("Program Ended. Press any key to close.");
            Console.ReadKey();
            // Draw image to screen.
            //  e.Graphics.DrawImage(newImage, ulCorner);


        }
    }
}
