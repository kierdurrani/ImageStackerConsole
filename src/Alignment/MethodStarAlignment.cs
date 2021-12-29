using System;

namespace ImageStackerConsole.Alignment
{
    public class MethodStarAlignment : IAlignmentMethod
    {

        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {
            throw new NotImplementedException();
        }


        public static StarCoordinates[] findStarCoordinates(RGBImage rgbImage) {

            byte[,] greyArray = rgbImage.GetGreyscaleArray();

            for (int y = 10; y < greyArray.GetLength(0) - 10; y += 3)
            {
                for (int x = 10; x < greyArray.GetLength(1) - 10; x += 3)
                {
                    // This could be done quickly in python using numpy to multiply a filter array by the other

                    // TODO - instead use the premise that the centre of the star should be the brightest 
                    if (greyArray[y,x] > 50)
                    {
                        // TODO; this doens't work well for the case of a single outlier pixel in the 'dark' region happens to be super light
                        int bright = greyArray[y + 1, x] + greyArray[y - 1, x] + greyArray[y, x + 1] + greyArray[y, x - 1];
                        bright += (greyArray[y + 2, x + 2] + greyArray[y + 2, x - 2] + greyArray[y - 2, x + 2] + greyArray[y - 2, x + 2]);

                        int dark = greyArray[y, x - 10] + greyArray[y, x + 10] + greyArray[y + 10, x] + greyArray[y - 10, x];
                        dark += greyArray[y + 7, x + 7] + greyArray[y + 7, x - 7] + greyArray[y - 7, x + 7] + greyArray[y - 7, x - 7];

                        // float darkRatio = ((float) bright + dark )/ (bright); threshold 1.3
                        int darkRatio = bright - dark;
                        if (darkRatio > 40 * 8)
                        {
                           // starCandidates.add(new StarCoordinates(x, y));
                        }
                    }
                }
            }

            // TODO - complete implementation

            return null;
        }
        

        // Internal class used to represent the position of stars within the image
        
    }

    public class StarCoordinates : IComparable
    {
        // 
        public int xCoord { get; }
        public int yCoord { get; }

        public StarCoordinates(int xValue, int yValue)
        {
            xCoord = xValue;
            yCoord = yValue;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }


    }

}
