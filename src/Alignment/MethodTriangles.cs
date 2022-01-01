using System;
using System.Collections.Generic;

namespace ImageStackerConsole.Alignment
{
    class MethodTriangles : IAlignmentMethod
    {

        public OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar)
        {

            List<StarCoordinates> img1stars = StarCoordinates.findStarCoordinates(rgbImg1);
            List<StarCoordinates> img2stars = StarCoordinates.findStarCoordinates(rgbImg2);

            List<Triangle> Triangles1 = GenerateTriangles(img1stars, rgbImg1);
            foreach (Triangle t in Triangles1)
            {
                t.DrawTriangle(rgbImg1);
            }

            rgbImg1.SaveToDisk(@"C:\Users\Kier\Developing\ImageStackerConsole\testImages\TrianglesOUT.JPG");
            return null;
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
                int xBucket = (int) Math.Floor(8.0 * (starCoord.xCoord / img.ImageWidth));
                int yBucket = (int) Math.Floor(8.0 * (starCoord.yCoord / img.ImageHeight));

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
                                   
                                    Triangles.Add( new Triangle( p1, p2, p3) );
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
        StarCoordinates point1;
        StarCoordinates point2;
        StarCoordinates point3;
        
        StarCoordinates centre;
        
        // Sorted such that angle1 is the largest
        double angle1;
        double angle2;
        double angle3;

        double length1;
        double length2;
        double length3;

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

        }

        private double cosineFormulaInverse(double a, double b, double c)
        {
            return Math.Acos((a * a - (b * b + c * c)) / (-2.0 * b * c));
        }

        public void DrawTriangle( RGBImage img )
        {
            Random random = new Random();
            int color = random.Next(1, 16777216);

            Console.WriteLine($"Drawing triangle: {(int) point1.xCoord},{(int) point1.yCoord}; {(int) point2.xCoord},{(int) point2.yCoord}; {(int) point3.xCoord},{(int) point3.yCoord}");
            
            img.DrawLine((int) point1.xCoord, (int) point1.yCoord, (int) point2.xCoord, (int) point2.yCoord, color);
            img.DrawLine((int) point2.xCoord, (int) point2.yCoord, (int) point3.xCoord, (int) point3.yCoord, color);
            img.DrawLine((int) point3.xCoord, (int) point3.yCoord, (int) point1.xCoord, (int) point1.yCoord, color);
        }

        public int CompareTo(object obj)
        {
            return (int) (angle1 - ((Triangle) obj).angle1);
        }
    }
}
