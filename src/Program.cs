using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ImageStackerConsole.Alignment;

namespace ImageStackerConsole
{
    class Program
    {
        static Stopwatch watch = new System.Diagnostics.Stopwatch();

        static void Main(string[] args)
        {           
            // Testing import and greyscale methods
            watch.Start();
            RGBImage img = new RGBImage(@"C:\Users\Kier\Pictures\Andromeda galaxy.jpg");
            img.GetGreyscaleArray();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");


            // IMAGE SELECTION
            // string imgDir = @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\";
            // string[] ImagePaths = new string[] { imgDir + "IMG_1311.JPG", imgDir + "IMG_1320.JPG", imgDir + "IMG_1326.JPG" };
            string imgDir = @"C:\Users\Kier\Developing\ImageStackerConsole\testImages\";
            string[] ImagePaths = new string[] { imgDir + "b.JPG", imgDir + "b2.JPG"};


            // TESTING THE 
            // MethodStarAlignment
            RGBImage img1 = new RGBImage(imgDir + "IMG_1311.JPG");
            List<StarCoordinates> coords = StarCoordinates.findStarCoordinates(img1);
            foreach(StarCoordinates star in coords)
            {
                img1.MakeGreenCross((int) star.xCoord, (int) star.yCoord);
            }
            img1.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\outtest.png");
            Console.WriteLine("Completed the star detection test");


            // img1.DrawLine(0, 0, 1000, 1000);
            // img1.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\LineDrawTest.png");
            
            // ALIGNMENT METHOD
            // var method = new MethodCrossCorrelation();
            var method = new MethodTriangles();
            AlignmentOrchestrator orchestrator = new AlignmentOrchestrator(ImagePaths, method);
            orchestrator.allPairs = true;

            AlignedImages alignedImages = orchestrator.CalculateOffsetParameterTable();


            // PROCESS ALIGMENT RESULT
            WriteStringArrayToFile(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\AlignmentsFound_TEST.txt", alignedImages.GetStringRepresentation());

            alignedImages.IsConsistent();

            Console.WriteLine("Program Ended. Press any key to close.");
            Console.ReadKey();
        }

        public static void WriteStringArrayToFile(String filePath, String[] data)
        {
            File.WriteAllLines(filePath, data);
        }

    }
}
