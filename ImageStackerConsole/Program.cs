using System;
using System.Drawing;
using System.IO;
using ImageStackerConsole.Alignmnet;

namespace ImageStackerConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            RGBImage img = new RGBImage(@"C:\Users\Kier\Pictures\Andromeda galaxy.jpg");

            Console.WriteLine(img.RGBArray[1, 0, 0]);
            Console.WriteLine(img.RGBArray[1, 0, 1]);
            Console.WriteLine(img.RGBArray[1, 0, 2]);
                
            img.SaveToDisk(@"C:\Users\Kier\Pictures\output.png");

            img.GetGreyscaleArray();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");


            OffsetParameters Param1 = new OffsetParameters(0, 0, 0, 1);
            OffsetParameters Param2 = new OffsetParameters(-24, 76, 0, 1);
            OffsetParameters Param3 = new OffsetParameters(134,-101,0,1);
            Param2 = Param2.CalculateInverse();

            Console.WriteLine( Param1.GetStringRepresentation());
            Console.WriteLine( Param2.GetStringRepresentation());

            Console.WriteLine(OffsetParameters.Compose(Param1, Param2).GetStringRepresentation());


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

            Console.WriteLine("Program Ended. Press any key to close.");
            Console.ReadKey();
        }

        public static void WriteStringArrayToFile(String filePath, String[] data)
        {
            File.WriteAllLines(filePath, data);
        }

    }
}
