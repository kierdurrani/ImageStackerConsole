using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageStackerConsole.Alignment;
using ImageStackerConsole.Stacking;

namespace ImageStackerConsole
{
    class Program
    {
        static Stopwatch watch = new System.Diagnostics.Stopwatch();
        static void Main(string[] args)
        {
            // Testing import and greyscale methods
            watch.Start();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");


            // IMAGE SELECTION
            // string imgDir = @"C:\Users\Kier\Developing\Space Image Stack Project\PICTURE LIBRARY\282CANON\";
            // string[] ImagePaths = new string[] { imgDir + "IMG_1311.JPG", imgDir + "IMG_1320.JPG", imgDir + "IMG_1326.JPG" };
            string imgDir = @"C:\Users\Kier\Developing\ImageStackerConsole\testImages\";
            imgDir = @"C:\Users\Kier\Developing\ImageStackerConsole\testImages\in\";
            // string[] ImagePaths = new string[] { imgDir + "in1.JPG", imgDir + "in1b-comp2.png" };
            string[] ImagePaths = new string[] {imgDir + "IMG_1317.JPG",
                                    imgDir + "IMG_1318.JPG",
                                    imgDir + "IMG_1319.JPG",
                                    imgDir + "IMG_1320.JPG",
                                    imgDir + "IMG_1321.JPG",
                                    imgDir + "IMG_1322.JPG",
                                    imgDir + "IMG_1323.JPG",
                                    imgDir + "IMG_1324.JPG",
                                    imgDir + "IMG_1325.JPG",
                                    imgDir + "IMG_1326.JPG",
                                    imgDir + "IMG_1327.JPG",
                                    imgDir + "IMG_1328.JPG",
                                    imgDir + "IMG_1329.JPG",
                                    imgDir + "IMG_1330.JPG"};



            // TESTING THE STAR DETECTION METHOD
            // StarDetectionTest(new RGBImage(imgDir + "IMG_1311.JPG"), @"C:\Users\Kier\Developing\ImageStackerConsole\outtest.png");
            // StarDetectionTest(new RGBImage(ImagePaths[0]), @"C:\Users\Kier\Developing\ImageStackerConsole\testImages\OUT-StarDetectionTest0.png");
            // StarDetectionTest(new RGBImage(ImagePaths[1]), @"C:\Users\Kier\Developing\ImageStackerConsole\testImages\OUT-StarDetectionTest1.png");


            // img1.DrawLine(0, 0, 1000, 1000);
            // img1.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\LineDrawTest.png");

            // ALIGNMENT METHOD
            // var method = new MethodCrossCorrelation();
            var method = new MethodTriangles();
            AlignmentOrchestrator orchestrator = new AlignmentOrchestrator(ImagePaths, method);
            orchestrator.AllPairs = true;

            // AlignedImages alignedImages = orchestrator.CalculateOffsetParameterTable();
            AlignedImages alignedImages = AlignedImages.ImportAlignmentParameters(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\AlignmentsFound_TEST.txt");

            // PROCESS ALIGMENT RESULT
            // WriteStringArrayToFile(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\AlignmentsFound_TEST.txt", alignedImages.GetStringRepresentation());

            alignedImages.IsConsistent();


            // MEMORY USAGE ESTIMATION:
            GC.Collect();
            long v = GC.GetTotalMemory(true);
            byte[,,,] a = new byte[3888,2592,20,3];
            
            long v2 = GC.GetTotalMemory(false);
            Console.WriteLine(v2 - v);
            // int b = a.Length;

            // Stacking
            IStackingMethod s = new StandardStack();
            RGBImage output = s.StackImages(alignedImages, null);
            output.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\FinalOut.png");

            Bitmap inMap = new Bitmap(ImagePaths[1]);
            Bitmap outmap = StandardStack.TransformImage( inMap, alignedImages.OffsetParameterTable[0, 1]);
            outmap.Save(imgDir + "transformedOUT.png", ImageFormat.Png);

            Console.WriteLine("Program Ended. Press any key to close.");
            Console.ReadKey();
        }

        private static void StarDetectionTest(RGBImage img1, String OutPath)
        {
            List<StarCoordinates> coords = StarCoordinates.findStarCoordinates(img1);
            foreach (StarCoordinates star in coords)
            {
                img1.MakeGreenCross((int)star.xCoord, (int)star.yCoord);
            }
            img1.SaveToDisk(OutPath);
            Console.WriteLine("Completed the star detection test");
        }

        public static void WriteStringArrayToFile(String filePath, String[] data)
        {
            File.WriteAllLines(filePath, data);
        }

    }
}
