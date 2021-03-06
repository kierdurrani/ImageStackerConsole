using System;

namespace ImageStackerConsole.Alignment
{
    class MethodCrossCorrelation : IAlignmentMethod
    {
        // Extremal 
        int xMax = 160;
        int yMax = 160;
        
        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {
            byte[,] img1 = rgbImg1.GetGreyscaleArray();
            byte[,] img2 = rgbImg2.GetGreyscaleArray();

            byte[,] smallImg1 = MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(img1))))))));
            byte[,] smallImg2 = MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(img2))))))));
            int scaleFactor = 16;

            ulong bestValue = 0;
            // int yMax = (int) (0.75 * Math.Min(smallImg1.GetLength(0), smallImg2.GetLength(1)) );
            // int xMax = (int) (0.75 * Math.Min(smallImg1.GetLength(1), smallImg2.GetLength(1)) );

            int xBest = 0;
            int yBest = 0;
            for (int yOffset = -yMax; yOffset < yMax; yOffset++)
            {
                if (yOffset % 10 == 0) { Console.WriteLine($"{yOffset} out of {yMax}"); }

                for (int xOffset = -xMax; xOffset < xMax; xOffset++)
                {
                    // TODO add ROTATION
                    ulong correlation = CalculateCrossCorrelation(smallImg1, smallImg2, xOffset, yOffset);
                    if (correlation > bestValue)
                    {
                        xBest = scaleFactor * xOffset;
                        yBest = scaleFactor * yOffset;
                        bestValue = correlation;
                    }
                }
            }


            byte[,] medImg1 = MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(img1))));
            byte[,] medImg2 = MakeSmaller(GaussianBlur(MakeSmaller(GaussianBlur(img2))));
            scaleFactor = 4;
            for (int yOffset = yBest - scaleFactor; yOffset < yBest + scaleFactor; yOffset++)
            {
                for (int xOffset = xBest - scaleFactor; xOffset < xBest + scaleFactor; xOffset++)
                {
                    ulong correlation = CalculateCrossCorrelation(medImg1, medImg2, xOffset, yOffset);
                    if (correlation > bestValue)
                    {
                        xBest = scaleFactor * xOffset;
                        yBest = scaleFactor * yOffset;
                        bestValue = correlation;
                    }
                }
            }


            // Precise Alignment:
            bestValue = 0; // Reset best value. Blurring may cause worse alignment to look better?
            for (int yOffset = yBest - scaleFactor; yOffset < yBest + scaleFactor; yOffset++)
            {
                for (int xOffset = xBest - scaleFactor; xOffset < xBest + scaleFactor; xOffset++)
                {
                    ulong correlation = CalculateCrossCorrelation(img1, img2, xOffset, yOffset);
                    if (correlation > bestValue)
                    {
                        xBest = xOffset;
                        yBest = yOffset;
                        Console.WriteLine($"DEBUG: ({xOffset},{yOffset},{bestValue})");
                        bestValue = correlation;
                    }
                }
            }

            return new OffsetParameters(xBest, yBest, 0.0, 1.0);
        }


        private static byte[,] GaussianBlur(byte[,] greyArray)
        {
            // The first dimension of getlength gets the height
            byte[,] output = new byte[greyArray.GetLength(0), greyArray.GetLength(1)];
            for (int y = 2; y < greyArray.GetLength(0) - 2; y++)
            {
                for (int x = 2; x < greyArray.GetLength(1) - 2; x++)
                {
                    // Gaussian of each point is weighted sum of nearby points in the original image
                    int cross1 = greyArray[y + 1, x] + greyArray[y - 1, x] + greyArray[y, x + 1] + greyArray[y, x - 1];
                    int cross2 = greyArray[y + 2, x] + greyArray[y - 2, x] + greyArray[y, x + 2] + greyArray[y, x - 2];

                    int xshape1 = greyArray[y - 1, x - 1] + greyArray[y - 1, x + 1] + greyArray[y + 1, x - 1] + greyArray[y + 1, x + 1];
                    int xshape2 = greyArray[y + 2, x + 2] + greyArray[y + 2, x - 2] + greyArray[y - 2, x + 2] + greyArray[y - 2, x - 2];

                    int chessknight = greyArray[y + 2, x + 1] + greyArray[y + 2, x - 1] + greyArray[y + 1, x + 2] + greyArray[y + 1, x - 2]
                            + greyArray[y - 1, x + 2] + greyArray[y - 1, x - 2] + greyArray[y - 2, x + 1] + greyArray[y - 2, x + 1];

                    int total = (26 * cross1 + 7 * cross2 + 16 * xshape1 + xshape2 + 4 * chessknight + 41 * greyArray[y, x]) / 273;
                    output[y, x] = (byte) total;
                }
            }
            return output;

        }

        private static byte[,] MakeSmaller(byte[,] input)
        {
            byte[,] output = new byte[input.GetLength(0) / 2, input.GetLength(1) / 2];
            for (int y = 0; y < output.GetLength(0); y++)
            {
                for (int x = 0; x < output.GetLength(1); x++)
                {
                    output[y,x] = input[2 * y, 2 * x];
                }
            }
            return output;
        }


        private ulong CalculateCrossCorrelation(byte[,] img1, byte[,] img2, int xOffset, int yOffset)
        {
            // Assert offsets are bounded between 0-75% of dimension.

            ulong correlation = 0;

            // Having a -ve offset is the same as offsetting the OTHER picture by a positive amount.
            // Do this by inverting the bounds of the for loop

            // TODO: FIX THE BOUNDS ON THE FOR LOOPS. - 0.75 factor is arbitrary
            int yLimit = (int) (0.75 * Math.Min(img2.GetLength(0), img1.GetLength(0)) );
            int xLimit = (int) (0.75 * Math.Min(img2.GetLength(1), img1.GetLength(1)) );

 
            if (xOffset >= 0)
            {
                if (yOffset >= 0)
                {
                    //  +x +y
                    for (int y = yOffset; y < (yLimit - yOffset); y++)
                    {
                        int buffer = 0; // prevent overflow
                        for (int x = xOffset; x < (xLimit - xOffset); x++)
                        {
                            buffer += img1[y + yOffset, x + xOffset] * img2[y, x];
                        }
                        correlation += (ulong) buffer;
                    }
                }
                else
                {
                    //  +x -y
                    yOffset = -yOffset;
                    for (int y = yOffset; y < (yLimit - yOffset); y++)
                    {
                        int buffer = 0; // prevent overflow
                        for (int x = xOffset; x < (xLimit - xOffset); x++)
                        {
                            buffer += img1[y, x + xOffset] * img2[y + yOffset, x];
                        }
                        correlation += (ulong) buffer;
                    }
                }
            }
            else
            {
                // then -ve x
                xOffset = -xOffset;
                if (yOffset >= 0)
                {
                    //  -x +y
                    for (int y = yOffset; y < (yLimit - yOffset); y++)
                    {
                        int buffer = 0; // prevent overflow
                        for (int x = xOffset; x < (xLimit - xOffset); x++)
                        {
                            buffer += img1[y + yOffset, x] * img2[y, x + xOffset];
                        }
                        correlation += (ulong) buffer;
                    }
                }
                else
                {
                    // -x -y
                    yOffset = -yOffset;
                    for (int y = yOffset; y < (yLimit - yOffset); y++)
                    {
                        int buffer = 0; // prevent overflow
                        for (int x = xOffset; x < (xLimit - xOffset); x++)
                        {
                            buffer += img1[y, x] * img2[y + yOffset, x + xOffset];
                        }
                        correlation += (ulong) buffer;
                    }
                }
            }
            return (correlation);
        }

    }
}
