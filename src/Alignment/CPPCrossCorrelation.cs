using System;
using System.Runtime.InteropServices;

namespace ImageStackerConsole.Alignment
{
    internal class CPPCrossCorrelation //: IAlignmentMethod
    {
        // https://mark-borg.github.io/blog/2017/interop/

      
        // public static extern double[] CalculateCrossCorrelationOffsetParameters(byte[,] rgbImg1, byte[,] rgbImg2);

       // [DllImport(@"C:\Users\Kier\Developing\ImageStackerConsole\ImageStackerConsole\StackerDLLs.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
       // public static extern int doubleValues(int[] array1, int[] array2);

        public void CalculateOffsetParameters(int[] rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {
            // byte[,] array1 = rgbImg1.GetGreyscaleArray();
            // byte[,] array2 = rgbImg2.GetGreyscaleArray();

            // double[] rawOutput = CPPCrossCorrelation.CalculateCrossCorrelationOffsetParameters(array1, array2);


      //      doubleValues(rgbImg1, rgbImg1);




            Console.WriteLine("CONTENT");
            foreach (int x in rgbImg1) {
               Console.WriteLine(x);
            }
            
        }

    }
}
