// Each image introduces a coordinate system called pixel-coordinate, which start in the top left with x -> right, y down

// Given two images of an overlapping region, an OffsetParameter instance represents the transformation
// which should be applied to the pixel coordinates in the first image, so that the pixel lines up with
// the pixel-coordinates of the second image.

// The rotation and dilation are applied first.
// The rotation is centered the origin of img1, going CCW in radians. 
// (x,y) is the translation which takes the (0,0) in img1 to its corresponding coordiante in img2 coords.

// (x', y') =  RotationMatrix(theta) * k * (x + X, y +Y)

using System;

namespace ImageStackerConsole
{
    public class OffsetParameters : IComparable
    {
        public double X { get; }
        public double Y { get; }
        public double Theta { get; }
        public double Zoom { get; }

        public OffsetParameters GetIdentityTransformation()
        {
            return new OffsetParameters(0.0, 0.0, 0.0, 1.0);
        }

        public OffsetParameters(double x, double y, double theta, double zoom)
        {
            this.X = x;
            this.Y = y;
            
            while (theta < 0) { theta =  + 2.0 * Math.PI; }
            this.Theta = theta % (2.0 * Math.PI); // range reduction
            
            // if (Zoom == 0) { throw  new ArgumentOutOfRangeException(); }
            this.Zoom = zoom;
        }

        public OffsetParameters(String stringRepresentation)
        { 
            string[] coords = stringRepresentation.Split(',');

            X     = double.Parse( coords[0] );
            Y     = double.Parse( coords[1] );
            Theta = double.Parse( coords[2] );
            Zoom  = double.Parse( coords[3] );
        }

        public string GetStringRepresentation()
        {
            return ( $"{Math.Round(X,1)},{Math.Round(Y, 1)},{Math.Round(Theta, 3)},{Math.Round(Zoom, 3)}");
        }

        public void PrintPretty()
        {
            Console.WriteLine($"({(int) X},{(int) Y}),{(float) Theta},{(float) Zoom}");
        }


        // Boolean methods for validation
        public bool IsCloseToIdentity()
        {
            // Ensure translational mismath is less than 2 pixels, rotational offset is small in radians, etc.
            bool b =  (Math.Abs(X) < 2.0) && (Math.Abs(Y) < 2.0) && ( Math.Abs(Theta) < 0.006) && (Math.Abs(Zoom - 1.0) < 0.006);
            return b;        
        }

        public bool IsNegationOf(OffsetParameters second)
        {
            return Compose(this, second).IsCloseToIdentity();
        }

        public static bool TriangleEquality(OffsetParameters first, OffsetParameters second, OffsetParameters third)
        {
            OffsetParameters total = Compose(Compose(first, second), third.CalculateInverse());
            return total.IsCloseToIdentity();
        }

        // GROUP ACTION TYPE METHODS
        public double[] TransformCoordinates(double PreTransform_X, double PreTransform_Y) 
        {
            //Console.WriteLine($"DEBUG: Transform in: {PreTransform_X},{PreTransform_Y}");
            double PostRotation_X = Zoom * (PreTransform_X * Math.Cos(Theta) + PreTransform_Y * Math.Sin(Theta));
            double PostRotation_Y = Zoom * (PreTransform_Y * Math.Cos(Theta) - PreTransform_X * Math.Sin(Theta));

           //Console.WriteLine($"DEBUG: Transform post rotation: {PostRotation_X},{PostRotation_Y}");
            return (new double[] { PostRotation_X + X, PostRotation_Y + Y } );
        }

        public double[][] TransformCoordinatesBulk(double[][] inCoords)
        {
            double zcos = Zoom * Math.Cos(Theta);
            double zsin = Zoom * Math.Sin(Theta);

            double[][] output = new double[inCoords.Length][];
            for (int index = 0; index < inCoords.Length; index++)
            {
                double newX = (+inCoords[index][0] * zcos + inCoords[index][1] * zsin) + this.X;
                double newY = (-inCoords[index][0] * zsin + inCoords[index][1] * zcos) + this.Y;
                
                output[index] = new double[] { newX, newY };
            }

            return output;
        }


        public OffsetParameters CalculateInverse()
        {            
            // Let X be the pretransformation coordinate vector and R(t,k) the rotation & scaling matrix.
            // X' = R(t, k) * X + trans.       Hence:   X = R^-1(t, k) (X' - trans.) 
            // So the translation in the inverse transformation is R^-1(t, k) ( - trans.)
            // Use the fact R(t,k)^[-1] = R(-t, 1/k)

            double Inverted_X = - 1.0/Zoom * (X * Math.Cos(-Theta) + Y * Math.Sin(-Theta));
            double Inverted_Y = - 1.0/Zoom * (Y * Math.Cos(-Theta) - X * Math.Sin(-Theta));

            return new OffsetParameters(Inverted_X, Inverted_Y, -Theta, 1.0 / Zoom); // TODO, this is probably wrong
        }

        public static OffsetParameters Compose(OffsetParameters FirstTransform, OffsetParameters LastTransform)
        {
            double ComposedAngle = FirstTransform.Theta + LastTransform.Theta;
            double ComposedZoom = FirstTransform.Zoom * LastTransform.Zoom;
            
            // The 2nd transformation's rotation/zoom composes with and affects the translation from the first transform.
            double[] TransformedTranslation = LastTransform.TransformCoordinates(FirstTransform.X, FirstTransform.Y);
            double ComposedX = TransformedTranslation[0];
            double ComposedY = TransformedTranslation[1];

            return new OffsetParameters(ComposedX, ComposedY, ComposedAngle, ComposedZoom);
        }

        public int CompareTo(object obj)
        {
            // return (int) (1000.0 * (Zoom - ((OffsetParameters) obj).Zoom));
            int diff = (int) (X - ((OffsetParameters) obj).X);
            return diff;
        }
    }
}
