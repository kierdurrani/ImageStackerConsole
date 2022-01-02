using System;
using System.Collections.Generic;

namespace ImageStackerConsole.Alignment
{
    class MethodTriangles : IAlignmentMethod
    {

        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {
            Console.WriteLine("--------------- CalculateOffsetParameters USING TRIANGLE METHOD --------------");
            Console.WriteLine("Finding Stars in image");
            List<StarCoordinates> img1stars = StarCoordinates.findStarCoordinates(rgbImg1);
            List<StarCoordinates> img2stars = StarCoordinates.findStarCoordinates(rgbImg2);

            Console.WriteLine("Sorting Triangles");
            Triangle[] Triangles1 = GenerateTriangles(img1stars, rgbImg1).ToArray();
            Triangle[] Triangles2 = GenerateTriangles(img2stars, rgbImg2).ToArray();
            Array.Sort(Triangles1);
            Array.Sort(Triangles2);

            // Draw triangles for testing purposes.
            RGBImage temp = (RGBImage)rgbImg1.Clone();
            foreach (Triangle t in Triangles1)
            { 
                t.DrawTriangle(temp);

            }
            temp.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\TrianglesOUT.JPG");


            // PUT TRIANGLES IN BUCKETS BASED OFF ANGLE SIZES TO ALLOW EASY IDENTIFICATION OF SIMILAR TRIANGLES.
            Console.WriteLine("Put Triangles in buckets");
            const double degreesInARadian = 180.0 / Math.PI;
            const double tolerance = 1.5 / degreesInARadian; // How much can two angles differ by to still be considered similar? 

            // Create buckets
            List<Triangle>[][] TriangleBuckets1 = new List<Triangle>[180][];
            List<Triangle>[][] TriangleBuckets2 = new List<Triangle>[180][];
            for (int largestAngle = 0; largestAngle < TriangleBuckets1.Length; largestAngle++)
            {
                int innerArrayLength = Math.Min(180 - largestAngle, largestAngle) + 1;

                TriangleBuckets1[largestAngle] = new List<Triangle>[innerArrayLength];
                TriangleBuckets2[largestAngle] = new List<Triangle>[innerArrayLength];

                for (int secondAngle = 0; secondAngle < TriangleBuckets1[largestAngle].Length; secondAngle++)
                {
                    TriangleBuckets1[largestAngle][secondAngle] = new List<Triangle>();
                    TriangleBuckets2[largestAngle][secondAngle] = new List<Triangle>();
                }
            }

            // Populate buckets.
            foreach (Triangle t in Triangles1)
            {
                int largestAngleBucket = (int)Math.Floor(t.angle1 * degreesInARadian);
                int secondAngleBucket = (int)Math.Floor(t.angle2 * degreesInARadian);

                TriangleBuckets1[largestAngleBucket][secondAngleBucket].Add(t);
            }
            foreach (Triangle t in Triangles2)
            {
                int largestAngleBucket = (int)Math.Floor(t.angle1 * degreesInARadian);
                int secondAngleBucket = (int)Math.Floor(t.angle2 * degreesInARadian);

                TriangleBuckets2[largestAngleBucket][secondAngleBucket].Add(t);
            }


            // IDENTIFY SIMILAR TRIANGES - ones in adjacent ot corresponding buckets could be similar
            Console.WriteLine("IDENTIFYING SIMILAR TRIANGLES");
            List<SimilarTriangles> similarTriangleList = new List<SimilarTriangles>();
            for (int largestAngle = 0; largestAngle < TriangleBuckets1.Length; largestAngle++)
            {
                for (int secondAngle = 0; secondAngle < TriangleBuckets1[largestAngle].Length; secondAngle++)
                {
                    foreach (Triangle t1 in TriangleBuckets1[largestAngle][secondAngle])
                    {
                        try
                        {
                            // Check for similar triangles from adjacent bucket
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    // bounds check
                                    if ((largestAngle + i > 0) && ((largestAngle + i < TriangleBuckets1.Length)))
                                    {
                                        if ((secondAngle + j > 0) && (secondAngle + j < TriangleBuckets1[largestAngle + i].Length))
                                        {
                                            AppendSimilarTriangle(t1, TriangleBuckets2[largestAngle + i][secondAngle + j], tolerance, similarTriangleList);
                                        }
                                    }

                                }
                            }

                        }
                        catch
                        {
                            Console.WriteLine("EEEP");
                            Console.WriteLine(secondAngle);

                        }
                    }
                }
            }

            Console.WriteLine($"WE HAVE FOUND: ${similarTriangleList.Count} SIMILAR TRIANGLES OUT OF: ${Triangles1.Length} & ${Triangles1.Length}");

            List<OffsetParameters> hypotheticalOffsets = new List<OffsetParameters>();
            foreach (SimilarTriangles similarTriangles in similarTriangleList)
            {
                // Hypothetical Offsets:
                double dx = similarTriangles.t1.centre.xCoord - similarTriangles.t2.centre.xCoord;
                double dy = similarTriangles.t1.centre.yCoord - similarTriangles.t2.centre.yCoord;

                double zoom = similarTriangles.t2.length1 / similarTriangles.t1.length1;

                //  To find the rotation -> take the longest vector from the centre of the triangle to vertex as a reference vector.
                double largestMedianBearing1 = Math.Atan2(similarTriangles.t1.largestSemiMedian.yCoord, similarTriangles.t1.largestSemiMedian.xCoord);
                double largestMedianBearing2 = Math.Atan2(similarTriangles.t2.largestSemiMedian.yCoord, similarTriangles.t2.largestSemiMedian.xCoord);

                double rotation = largestMedianBearing1 - largestMedianBearing2;

                
                if (Math.Abs(zoom - 1.0) < 0.05)
                {
                    OffsetParameters hypotheticalOffset = new OffsetParameters(dx, dy, rotation, zoom);
                    hypotheticalOffsets.Add(hypotheticalOffset);
                }


            }

            // TODO: See if there is a consensus amongst the hypotheticalOffsetParameter list to obtain the actual offset parameters.
            // Method: Sort the list based off of some order.
            // For each hypotherical parameter, see how many other parameters are 'close enough' to said parameter.
            // Since it is sorted, we can do this in approximately O(n log n);
            OffsetParameters[] possOffsets = hypotheticalOffsets.ToArray();
            Array.Sort(possOffsets);
            double transTolerance = 3.0;
            
            OffsetParameters bestOffset = null;
            int votes = 0;
            for (int i = 0; i < possOffsets.Length; i++)
            {
                int thisvotes = 0;

                int j = i - 1;
                while (j >= 0)
                {
                    // Translated too far left
                    if (possOffsets[j].X + transTolerance < possOffsets[i].X) { break; }

                    // Check whether this pair of Offsets are within tollerance.
                    if ((Math.Abs(possOffsets[j].X - possOffsets[i].X) < transTolerance) &&
                        (Math.Abs(possOffsets[j].Y - possOffsets[i].Y) < transTolerance) &&
                        (Math.Abs(possOffsets[j].Theta - possOffsets[i].Theta) < 0.001) &&
                        (Math.Abs(possOffsets[j].Zoom - possOffsets[i].Zoom) < 0.001))
                    {
                        thisvotes++;
                    }
                    j--;
                }

                j = i + 1;
                while (j < possOffsets.Length)
                {
                    // Translated too far right
                    if (possOffsets[j].X > possOffsets[i].X + transTolerance) { break; }

                    // Check whether this pair of Offsets are within tollerance.
                    if ((Math.Abs(possOffsets[j].X - possOffsets[i].X) < transTolerance) &&
                        (Math.Abs(possOffsets[j].Y - possOffsets[i].Y) < transTolerance) &&
                        (Math.Abs(possOffsets[j].Zoom - possOffsets[i].Zoom) < 0.001) &&
                        (Math.Abs(possOffsets[j].Zoom - possOffsets[i].Zoom) < 0.001)) 
                    {
                        thisvotes++;
                    }
                    j++;
                }

                if (thisvotes > votes)
                {
                    bestOffset = possOffsets[i];
                    votes = thisvotes;
                }
            }


            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

            hypotheticalOffsets.Sort();
            String[] hypOffsets = new String[hypotheticalOffsets.Count];
            int ii = 0;
            foreach  ( OffsetParameters offset in hypotheticalOffsets)
            {
                hypOffsets[ii++] = offset.GetStringRepresentation();
            }

            // Program.WriteStringArrayToFile($"C:\\Users\\Kier\\Developing\\ImageStackerConsole\\testImages\\TriTest_{unixTime}.txt", hypOffsets);

            return bestOffset;
        }

        private static void AppendSimilarTriangle(Triangle t1, IEnumerable<Triangle> t2list, double tolerance, List<SimilarTriangles> similarTriangleList)
        {
            foreach (Triangle t2 in t2list)
            {
                // Determine whether similar
                if ((Math.Abs(t1.angle1 - t2.angle1) > tolerance) || (Math.Abs(t1.angle2 - t2.angle2) > tolerance) || (Math.Abs(t1.angle3 - t2.angle3) > tolerance))
                {
                    // Not similar - do not append
                }
                else
                {
                    similarTriangleList.Add(new SimilarTriangles(t1, t2));
                }
            }
        }

        // Generates all possible triangles from choosing star coordinates.
        private static List<Triangle> GenerateTriangles(List<StarCoordinates> AllStarCoordinates, RGBImage img)
        {

            // Split image into an 8 x 8 grid of buckets. Put stars in each bucket depending on location.
            List<StarCoordinates>[,] starBuckets = new List<StarCoordinates>[8, 8];

            // Initialise buckets
            for (int i = 0; i < 64; i++)
            {
                starBuckets[i / 8, i % 8] = new List<StarCoordinates>();
            }

            // Put stars into buckets
            foreach (StarCoordinates starCoord in AllStarCoordinates)
            {
                int xBucket = (int)Math.Floor(8.0 * (starCoord.xCoord / img.ImageWidth));
                int yBucket = (int)Math.Floor(8.0 * (starCoord.yCoord / img.ImageHeight));

                starBuckets[yBucket, xBucket].Add(starCoord);
            }

            // Go over any 3 choices of distinct buckets.
            // Demand that each pair of buckets is seperated by at least one other bucket to ensure triangles are large.
            // Demand that each pair of buckets is no more than 3 buckets apart in order to ensure there are not too many triangles.
            const int minBucketSeperation = 1;
            const int maxBucketSeperation = 3;

            List<Triangle> Triangles = new List<Triangle>();
            for (int star1bucket = 0; star1bucket < 64; star1bucket++)
            {
                for (int star2bucket = star1bucket; star2bucket < 64; star2bucket++)
                {
                    for (int star3bucket = star2bucket; star3bucket < 64; star3bucket++)
                    {

                        // Ensure buckets are sensible distance apart
                        int dx12 = Math.Abs(star2bucket % 8 - star1bucket % 8);
                        int dy12 = Math.Abs(star2bucket / 8 - star1bucket / 8);
                        if (dx12 < minBucketSeperation || dx12 > maxBucketSeperation) { continue; }
                        if (dy12 < minBucketSeperation || dy12 > maxBucketSeperation) { continue; }

                        int dx23 = Math.Abs(star2bucket % 8 - star3bucket % 8);
                        int dy23 = Math.Abs(star2bucket / 8 - star3bucket / 8);
                        if (dx23 < minBucketSeperation || dx23 > maxBucketSeperation) { continue; }
                        if (dy23 < minBucketSeperation || dy23 > maxBucketSeperation) { continue; }

                        int dx31 = Math.Abs(star1bucket % 8 - star3bucket % 8);
                        int dy31 = Math.Abs(star1bucket / 8 - star3bucket / 8);
                        if (dx31 < minBucketSeperation || dx31 > maxBucketSeperation) { continue; }
                        if (dy31 < minBucketSeperation || dy31 > maxBucketSeperation) { continue; }

                        // Buckets are a sensible distance apart. Find all possible triangles whose points lie in these buckets.

                        foreach (StarCoordinates p1 in starBuckets[star1bucket / 8, star1bucket % 8])
                        {
                            foreach (StarCoordinates p2 in starBuckets[star2bucket / 8, star2bucket % 8])
                            {
                                foreach (StarCoordinates p3 in starBuckets[star3bucket / 8, star3bucket % 8])
                                {
                                    Triangles.Add(new Triangle(p1, p2, p3));
                                }
                            }
                        }

                    }
                }
            }
            Console.WriteLine("Found #triangles: " + Triangles.Count);
            return Triangles;
        } // End function

    }

    class Triangle : IComparable
    {
        public StarCoordinates point1 { get; }
        public StarCoordinates point2 { get; }
        public StarCoordinates point3 { get; }

        public StarCoordinates largestSemiMedian { get;  }

        public StarCoordinates centre { get; }

        // Sorted such that angle1 is the largest
        public double angle1 { get; }
        public double angle2 { get; }
        public double angle3 { get; }

        public double length1 { get; }
        public double length2 { get; }
        public double length3 { get; }

        public Triangle(StarCoordinates p1, StarCoordinates p2, StarCoordinates p3)
        {
            // Points
            point1 = p1;
            point2 = p2;
            point3 = p3;

            // Centre Point
            double centreX = p1.xCoord + p2.xCoord + p3.xCoord;
            double centreY = p1.yCoord + p2.yCoord + p3.yCoord;
            centre = new StarCoordinates(centreX / 3, centreY / 3);

            // Lengths
            double[] lengths = new double[3];

            lengths[0] = StarCoordinates.Distance(p1, p2);
            lengths[1] = StarCoordinates.Distance(p2, p3);
            lengths[2] = StarCoordinates.Distance(p3, p1);

            Array.Sort(lengths);

            length1 = lengths[2]; // invert order so it is descending.
            length2 = lengths[1];
            length3 = lengths[0];

            // Angles
            double[] angles = new double[3];

            angles[0] = cosineFormulaInverse(length1, length2, length3);
            angles[1] = cosineFormulaInverse(length2, length3, length1);
            angles[2] = cosineFormulaInverse(length3, length1, length2);

            Array.Sort(angles);

            angle1 = angles[2];
            angle2 = angles[1];
            angle3 = angles[0];

            // largestSemiMedian represents the largest vector between the centre and a vertex.
            double distance = 0.0;
            if (StarCoordinates.Distance(point1, centre) > distance)
            {
                distance = StarCoordinates.Distance(point1, centre);
                largestSemiMedian = new StarCoordinates(point1, centre);
            }
            if (StarCoordinates.Distance(point2, centre) > distance)
            {
                distance = StarCoordinates.Distance(point2, centre);
                largestSemiMedian = new StarCoordinates(point2, centre);
            }
            if (StarCoordinates.Distance(point3, centre) > distance)
            {
                distance = StarCoordinates.Distance(point3, centre);
                largestSemiMedian = new StarCoordinates(point3, centre);
            }



        }

        private double cosineFormulaInverse(double a, double b, double c)
        {
            return Math.Acos((a * a - (b * b + c * c)) / (-2.0 * b * c));
        }

        public void DrawTriangle(RGBImage img)
        {
            Random random = new Random();
            int color = random.Next(1, 16777216);

            //  Console.WriteLine($"Drawing triangle: {(int) point1.xCoord},{(int) point1.yCoord}; {(int) point2.xCoord},{(int) point2.yCoord}; {(int) point3.xCoord},{(int) point3.yCoord}");

            img.DrawLine((int)point1.xCoord, (int)point1.yCoord, (int)point2.xCoord, (int)point2.yCoord, color);
            img.DrawLine((int)point2.xCoord, (int)point2.yCoord, (int)point3.xCoord, (int)point3.yCoord, color);
            img.DrawLine((int)point3.xCoord, (int)point3.yCoord, (int)point1.xCoord, (int)point1.yCoord, color);
        }

        public int CompareTo(object obj)
        {
            return (int)(100.0 * (angle1 - ((Triangle)obj).angle1));
        }
    }

    class SimilarTriangles
    {
        public Triangle t1 { get; }
        public Triangle t2 { get; }

        public SimilarTriangles(Triangle triangle1, Triangle triangle2)
        {
            t1 = triangle1;
            t2 = triangle2;
        }
    }
}
