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
            string imgDir = @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\";
            string[] ImagePaths = new string[] { imgDir + "IMG_1311.JPG", imgDir + "IMG_1320.JPG", imgDir + "IMG_1326.JPG" };


            // TESTING THE 
            // MethodStarAlignment
            RGBImage img1 = new RGBImage(imgDir + "IMG_1311.JPG");
            List<StarCoordinates> coords = MethodStarAlignment.findStarCoordinates(img1);
            foreach(StarCoordinates star in coords)
            {
                img1.MakeGreenCross(star.xCoord, star.yCoord);
            }
            img1.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\outtest.png");
            Console.WriteLine("Completed the star detection test");


            // ALIGNMENT METHOD
            // var method = new MethodCrossCorrelation();
            var method = new MethodStarAlignment();
            AlignmentOrchestrator orchestrator = new AlignmentOrchestrator(ImagePaths, method);
            orchestrator.allPairs = false;

            AlignedImages alignedImages = orchestrator.CalculateOffsetParameterTable();




            // PROCESS ALIGMENT RESULT
            WriteStringArrayToFile(@"C:\Users\Kier\Pictures\OUTPUT_TEST.txt", alignedImages.GetStringRepresentation());

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
