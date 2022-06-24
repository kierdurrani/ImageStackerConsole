// Each image introduces a coordinate system called pixel-coordinate, which start in the top left with x -> right, y V down

// An RAPixelMap instance is the map which should be applied to the pixel coodinates to get the corresponding coords on the Celesial Sphere.
// central coordinate (RA_0, Dec_0) 
// which should be applied to the pixel coordinates in the first image, so that the pixel lines up with
// the pixel-coordinates of the second image.

// The rotation and dilation are applied first.
// The rotation is centered the origin of img1, going CCW in radians. 
// (x,y) is the translation which takes the (0,0) in img1 to its corresponding coordiante in img2 coords.

// (x', y') =  RotationMatrix(theta) * k * (x + X, y +Y)

using System;

namespace ImageStackerConsole
{
    public class RAPixelMap : IComparable
    {
        public double RA0 { get; }
        public double   { get; }
        public double Theta { get; }
        public double Zoom { get; }


        public RAPixelMap(double x, double y, double theta, double zoom)
        {
            this.RA0 = x;
            this.DEC0 = y;
            
            while (theta < 0) { theta +=  + 2.0 * Math.PI; }
            this.Theta = theta % (2.0 * Math.PI); // range reduction
            
            if (Zoom < 0.0000001) { throw  new ArgumentOutOfRangeException(); }
            this.Zoom = zoom;
        }

        public RAPixelMap(String stringRepresentation)
        { 
            string[] coords = stringRepresentation.Split(',');
            try
            {
                RA0 = double.Parse(coords[0]);
                DEC0 = double.Parse(coords[1]);
                Theta = double.Parse(coords[2]);
                Zoom = double.Parse(coords[3]);
            }
            catch {
                Console.WriteLine(stringRepresentation);
                throw new FormatException();
            }
        }

        public string GetStringRepresentation()
        {
            return ( $"{Math.Round(RA0, 5)},{Math.Round(DEC0, 5)},{Math.Round(Theta, 5)},{Math.Round(Zoom, 5)}");
        }


        // GROUP ACTION TYPE METHODS
        public double[] TransformCoordinates(double PreTransform_X, double PreTransform_Y) 
        {
            //Console.WriteLine($"DEBUG: Transform in: {PreTransform_X},{PreTransform_Y}");
            double PostRotation_X = Zoom * (+ PreTransform_X * Math.Cos(Theta) + PreTransform_Y * Math.Sin(Theta));
            double PostRotation_Y = Zoom * (- PreTransform_X * Math.Sin(Theta) + PreTransform_Y * Math.Cos(Theta) );

           //Console.WriteLine($"DEBUG: Transform post rotation: {PostRotation_X},{PostRotation_Y}");
            return (new double[] { PostRotation_X + RA0, PostRotation_Y + DEC0 } );
        }

        public double[][] TransformCoordinatesBulk(double[][] inCoords)
        {
            double zcos = Zoom * Math.Cos(Theta);
            double zsin = Zoom * Math.Sin(Theta);

            double[][] output = new double[inCoords.Length][];
            for (int index = 0; index < inCoords.Length; index++)
            {
                double newX = (+inCoords[index][0] * zcos + inCoords[index][1] * zsin) + this.RA0;
                double newY = (-inCoords[index][0] * zsin + inCoords[index][1] * zcos) + this.DEC0;
                
                output[index] = new double[] { newX, newY };
            }

            return output;
        }


        public RAPixelMap CalculateInverse()
        {            
            // Let X be the pretransformation coordinate vector and R(t,k) the rotation & scaling matrix.
            // X' = R(t, k) * X + trans.       Hence:   X = R^-1(t, k) (X' - trans.) 
            // So the translation in the inverse transformation is R^-1(t, k) ( - trans.)
            // Use the fact R(t,k)^[-1] = R(-t, 1/k)

            double Inverted_X = - 1.0/Zoom * (RA0 * Math.Cos(-Theta) + DEC0 * Math.Sin(-Theta));
            double Inverted_Y = - 1.0/Zoom * (DEC0 * Math.Cos(-Theta) - RA0 * Math.Sin(-Theta));

            return new RAPixelMap(Inverted_X, Inverted_Y, -Theta, 1.0 / Zoom);
        }

        public int CompareTo(object obj)
        {
            // return (int) (1000.0 * (Zoom - ((RAPixelMap) obj).Zoom));
            int diff = (int) (RA0 - ((RAPixelMap) obj).RA0);
            return diff;
        }
    }
}
