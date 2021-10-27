﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ImageStackerConsole.Alignmnet;

namespace ImageStackerConsole
{
    class Program
    {
        static Stopwatch watch = new System.Diagnostics.Stopwatch();

        static void Main(string[] args)
        {           
            watch.Start();


            RGBImage img = new RGBImage(@"C:\Users\Kier\Pictures\Andromeda galaxy.jpg");


            img.GetGreyscaleArray();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

            // OffsetParameters Method1 = OffsetParameters.Compose(Param1, Param1.CalculateInverse());
            // OffsetParameters Method2 = OffsetParameters.Compose(Param1.CalculateInverse(), Param1);

            if (false)
            {
                string[] ImagePaths = new string[] {
                                @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\IMG_1311.JPG",
                                @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\IMG_1320.JPG",
                                @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\IMG_1326.JPG"
                            };

                AlignmentOrchestrator orchestrator = new AlignmentOrchestrator(ImagePaths, new MethodCrossCorrelation());

                orchestrator.allPairs = true;

                AlignedImages alignedImages = orchestrator.CalculateOffsetParameterTable();

                WriteStringArrayToFile(@"C:\Users\Kier\Pictures\OUTPUT_TEST.txt", alignedImages.GetStringRepresentation());

                alignedImages.IsConsistent();
            }

            Console.WriteLine("Program Ended. Press any key to close.");
            Console.ReadKey();
        }

        public static void WriteStringArrayToFile(String filePath, String[] data)
        {
            File.WriteAllLines(filePath, data);
        }

    }
}
