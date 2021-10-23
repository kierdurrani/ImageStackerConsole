using ImageStackerConsole.Alignmnet;
using System.Runtime.InteropServices;

namespace ImageStackerConsole
{
    class Program2
    {

        [DllImport(@"C:\Users\Kier\Developing\ImageStackerConsole\Release\StackerDLLs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int sum_values(int[] array1, int size);


        static void Main(string[] args)
        {
            System.Console.WriteLine("Started in entrypoint: Program2.cs ");
            int[] test = { 2, 3, 5, 7, 11 };

            //            CPPCrossCorrelation instance = new CPPCrossCorrelation();

            // instance.CalculateOffsetParameters(test, null, null);

            int sum = sum_values(test, test.Length);
            System.Console.WriteLine(sum);

            System.Console.ReadKey();

            
        }
    }
}
