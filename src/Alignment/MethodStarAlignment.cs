using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImageStackerConsole.Alignment
{
    public class MethodStarAlignment : IAlignmentMethod
    {

        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {

            List<StarCoordinates> starCoordinatesImg1 = findStarCoordinates(rgbImg1);
            List<StarCoordinates> starCoordinatesImg2 = findStarCoordinates(rgbImg2);

            throw new NotImplementedException();
        }


        public static List<StarCoordinates> findStarCoordinates(RGBImage rgbImage) {

            List<StarCoordinates> starCandidates = new List<StarCoordinates>();
            byte[,] greyArray = rgbImage.GetGreyscaleArray();

            // Find all possible stars by iterating over the image pixes with a filter:
            for (int y = 10; y < greyArray.GetLength(0) - 10; y++)
            {
                for (int x = 10; x < greyArray.GetLength(1) - 10; x++ )
                {
                    // Assert the centre of the star must be the brightest pixel of its neighbours and at least 50 bright.
                    if (greyArray[y, x] < 50 
                    || greyArray[y, x] < greyArray[y + 1, x] || greyArray[y, x] < greyArray[y, x - 1]
                    || greyArray[y, x] < greyArray[y - 1, x] || greyArray[y, x] < greyArray[y, x + 1]
                    || greyArray[y, x] < greyArray[y + 1, x + 1] || greyArray[y, x] < greyArray[y + 1, x - 1]
                    || greyArray[y, x] < greyArray[y - 1, x + 1] || greyArray[y, x] < greyArray[y - 1, x - 1])
                    {
                        continue;
                    }

                    // We say we have found the centre of a star it the adjacent few pixels are much brighter than a ring of pixels much further out.
                    // TODO; this doens't work well for the case of a single outlier pixel in the 'dark' region happens to be super light
                    int bright = greyArray[y + 1, x] + greyArray[y - 1, x] + greyArray[y, x + 1] + greyArray[y, x - 1];
                    bright += (greyArray[y + 2, x + 2] + greyArray[y + 2, x - 2] + greyArray[y - 2, x + 2] + greyArray[y - 2, x + 2]);

                    int dark = greyArray[y, x - 10] + greyArray[y, x + 10] + greyArray[y + 10, x] + greyArray[y - 10, x];
                    dark += greyArray[y + 7, x + 7] + greyArray[y + 7, x - 7] + greyArray[y - 7, x + 7] + greyArray[y - 7, x - 7];

                    // float darkRatio = ((float) bright + dark )/ (bright); threshold 1.3
                    int darkRatio = bright - dark;
                    if (darkRatio > 40 * 8)
                    {
                        starCandidates.Add(new StarCoordinates(x, y));
                    }
                }
            }

            // CULLING THE STARS: prevent multiple counts of the same star - i.e. remove close together star candidates
            // Group together stars which are close together.
            List<List<StarCoordinates>> listsOfEquivalentStars = new List<List<StarCoordinates>>();

            while ( starCandidates.Count > 0)
            {
                Console.WriteLine(starCandidates.Count);
                // Create a new equivalence class for the first star in the list of star candidates.
                List<StarCoordinates> newSetOfEquivalentStars = new List<StarCoordinates>();

                newSetOfEquivalentStars.Add(starCandidates.First());
                starCandidates.RemoveAt(0);

                listsOfEquivalentStars.Add(newSetOfEquivalentStars);

                // Repeatedly check if any remaining stars are close to any of the stars in the equivalence class. 
                // If close, then add the star to the equivalent set and check all remaining stars again.
                IEnumerator StarCandidatesEnumerator = starCandidates.GetEnumerator();
                while( StarCandidatesEnumerator.MoveNext() )
                {
                    StarCoordinates uncategorisedStar = (StarCoordinates) StarCandidatesEnumerator.Current;

                    IEnumerator newSetOfEquivalentStarsEnum = newSetOfEquivalentStars.GetEnumerator();
                    while( newSetOfEquivalentStarsEnum.MoveNext() )
                    {

                        StarCoordinates starInNewSet = (StarCoordinates) newSetOfEquivalentStarsEnum.Current;

                        if( StarCoordinates.Distance(uncategorisedStar, starInNewSet) < 5) 
                        {
                            // Indicates the star is sufficiently close to a star in this new set that it should be included in this new set

                            newSetOfEquivalentStars.Add(uncategorisedStar);
                            starCandidates.Remove(uncategorisedStar);

                            StarCandidatesEnumerator = starCandidates.GetEnumerator();
                            //newSetOfEquivalentStarsEnum = newSetOfEquivalentStars.GetEnumerator();
                            break;
                        }
                    }
                    continue;

                }              
            }

            // For each group of coordinates which represent the same star, calculate the average position of the brightest pts.
            List<StarCoordinates> starCoordinates = new List<StarCoordinates>(); 
            foreach (List<StarCoordinates> equivalentStars in listsOfEquivalentStars)
            {
                // Create list of the coordintaes of the brightest points
                int maxBrightness = 0; 
                List<StarCoordinates> brightestCoords = new List<StarCoordinates>();
                foreach (StarCoordinates coord in equivalentStars)
                {
                    if (greyArray[coord.yCoord, coord.xCoord] > maxBrightness)
                    {
                        maxBrightness = greyArray[coord.yCoord, coord.xCoord];
                        brightestCoords = new List<StarCoordinates>();
                        
                    }
                    if (greyArray[coord.yCoord, coord.xCoord] >= maxBrightness)
                    {
                        brightestCoords.Add(coord);
                    }
                }

                // Average the position of all the brightest points.
                int x = 0;
                int y = 0;
                foreach (StarCoordinates coord in brightestCoords)
                {
                    x += coord.xCoord;
                    y += coord.yCoord;
                }
                x = (int) (x / brightestCoords.Count);
                y = (int) (y / brightestCoords.Count);
                starCoordinates.Add(new StarCoordinates(x, y));
            }

            return starCoordinates;
        }
    }

  
    // Helper class to represent the coordinates of a given star in the image.
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

        public static double Distance(StarCoordinates coordinates1, StarCoordinates coordinates2)
        {
            int dx = coordinates1.xCoord - coordinates2.xCoord;
            int dy = coordinates1.yCoord - coordinates2.yCoord;

            return Math.Sqrt(dx * dx + dy * dy);
        }

    }

}
