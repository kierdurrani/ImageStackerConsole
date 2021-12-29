﻿using System;
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
       

            AlignmentOrchestrator orchestrator = new AlignmentOrchestrator(ImagePaths, new MethodCrossCorrelation());
            orchestrator.allPairs = false;

            AlignedImages alignedImages = orchestrator.CalculateOffsetParameterTable();

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
