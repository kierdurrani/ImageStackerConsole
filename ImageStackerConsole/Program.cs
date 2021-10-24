using System;
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


            OffsetParameters Param1 = new OffsetParameters(107500, 0, 0.01, 1.02);
            OffsetParameters Param2 = new OffsetParameters(114, 76, 0.025, 3.66);
            OffsetParameters Param3 = new OffsetParameters(-556,50, -0.03, 0.17);


            Console.WriteLine("Confirm the following identities hold: ");

            Console.WriteLine("Associativity of composition: ");
            OffsetParameters Method1 = OffsetParameters.Compose(Param1, OffsetParameters.Compose(Param2, Param3));
            OffsetParameters Method2 = OffsetParameters.Compose(OffsetParameters.Compose(Param1, Param2), Param3);
           
            Console.WriteLine(Method1.GetStringRepresentation());
            Console.WriteLine(Method2.GetStringRepresentation());
            Console.WriteLine("----------------------------------------------");


            Console.WriteLine("Inverse of product of 2: ");
            Method1 = OffsetParameters.Compose(Param1, Param2).CalculateInverse();
            Method2 = OffsetParameters.Compose(Param2.CalculateInverse(), Param1.CalculateInverse());

            Console.WriteLine(Method1.GetStringRepresentation());
            Console.WriteLine(Method2.GetStringRepresentation());
            Console.WriteLine("----------------------------------------------");


            Console.WriteLine("Inverse of product of 2: ");
            Method1 = OffsetParameters.Compose(Param1, OffsetParameters.Compose(Param2, Param3)).CalculateInverse();
            Method2 = OffsetParameters.Compose(OffsetParameters.Compose( Param3.CalculateInverse(), Param2.CalculateInverse()), Param1.CalculateInverse());

            Console.WriteLine(Method1.GetStringRepresentation());
            Console.WriteLine(Method2.GetStringRepresentation());
            Console.WriteLine("----------------------------------------------");

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
