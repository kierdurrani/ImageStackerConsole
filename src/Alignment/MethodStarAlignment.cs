using System;
using System.Collections.Generic;

namespace ImageStackerConsole.Alignment
{
    public class MethodStarAlignment : IAlignmentMethod
    {

        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {

            List<StarCoordinates> starCoordinatesImg1 = StarCoordinates.findStarCoordinates(rgbImg1);
            List<StarCoordinates> starCoordinatesImg2 = StarCoordinates.findStarCoordinates(rgbImg2);
            starCoordinatesImg1.Sort();
            starCoordinatesImg2.Sort();

            Console.WriteLine("Stars found in each image were: " + starCoordinatesImg1.Count + ',' + starCoordinatesImg2.Count);

            // convert to arrays for performance.
            double[][] starArray1 = StarCoordinates.convertStarListToArray(starCoordinatesImg1);
            double[][] starArray2 = StarCoordinates.convertStarListToArray(starCoordinatesImg2);
            
            // Best Offset Parameters found
            OffsetParameters bestParameters = null;
            int mostAlignedStars = 0;

            int yMax = Math.Min(rgbImg1.ImageHeight/2, rgbImg2.ImageHeight/2) ;
            int xMax = Math.Min(rgbImg1.ImageWidth /2, rgbImg2.ImageWidth /2);

            for (int yOffset = -yMax; yOffset < yMax; yOffset += 4)
            {
                for (int xOffset = -xMax; xOffset < xMax; xOffset += 4)
                {
                    // Transform the coordinates of the 2nd list                    
                    OffsetParameters currentOffsetParams = new OffsetParameters(xOffset, yOffset, 0.0, 0.0);

                    double[][] transformedArray2 = currentOffsetParams.TransformCoordinatesBulk( starArray2 );

                    int allignedStars = countAlignedStars(starArray1, transformedArray2);
                    if (allignedStars > mostAlignedStars)
                    {
                        bestParameters = currentOffsetParams;
                        mostAlignedStars = allignedStars;
                        Console.WriteLine("Aligned: " + allignedStars + " with params: " + xOffset + "," + yOffset);
                    }
                }
            }

            if (bestParameters == null) 
            {
                throw new AlignmentFailedException();
            }

            return bestParameters;
        }

        private static int countAlignedStars(double[][] coordsList1, double[][] coordsList2)
        {
            int starAlignmentCount = 0;
            const int detectionRadius = 8;

            // Foreach star in the 2nd (transformed) array, see if there is a star in the 1st array which has very similar coords.
            for ( int index = 0; index < coordsList2.Length ; index ++)
            {
                // Since coordsList1 is sorted by xCoord, we can use interval bisection in coordsList1 to quickly find
                // the coordinate with smallest xCoord which is still within detection radius to coord2.xCoord.
                // After that, we work rightwards until we are outside of the detection radius and see if any stars match up.

                // After finding the furthest left possible match, work rightwards.
                int lowerBound = 0;
                int upperBound = coordsList1.Length;
                while ((upperBound - lowerBound) > 4)
                {
                    // Interval Bisection: If coords2 is to the left of the half way coordinate:
                    int bisectionPoint = (upperBound + lowerBound) / 2;
                    if (coordsList2[index][0] - coordsList1[bisectionPoint][0] < -detectionRadius)
                    {
                        // bisection point was too big, decrease upper bound
                        upperBound = bisectionPoint;
                    }
                    else
                    {
                        // bisection point was too small, increase lower bound
                        lowerBound = bisectionPoint;
                    }
                }

                for (int i = lowerBound; i < coordsList1.Length; i++)
                {
                    double xMisalign = coordsList1[i][0] - coordsList2[index][0];
                    double yMisalign = coordsList1[i][1] - coordsList2[index][1];
                    if (xMisalign > detectionRadius)
                    {
                        // Console.WriteLine(xMisalign);
                        break; // gone too far horizontally.
                    }
                    
                    if( xMisalign * xMisalign  + yMisalign * yMisalign < detectionRadius * detectionRadius )
                    {
                        starAlignmentCount++;
                        break;
                    }

                }
            }
            // Console.WriteLine(starAlignmentCount);
            return starAlignmentCount;
        }
    }
}
