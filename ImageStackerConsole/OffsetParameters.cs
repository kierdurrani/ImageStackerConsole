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
    public class OffsetParameters
    {
        private double X { get; }
        private double Y { get; }
        private double Theta { get; }
        private double Zoom { get; }

        public OffsetParameters GetIdentityTransformation()
        {
            return new OffsetParameters(0.0, 0.0, 0.0, 1.0);
        }

        public OffsetParameters(double x, double y, double theta, double zoom)
        {
            this.X = x;
            this.Y = y;
            this.Theta = theta;
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
            return X + "," + Y + "," + Theta + "," + Zoom;
        }

        // Boolean methods for validation
        public bool IsCloseToIdentity()
        {
            // Ensure translational mismath is less than 2 pixels, rotational offset is small in radians, etc.
            bool b =  Math.Abs(X) < 2.0 && Math.Abs(Y) < 2.0 && Math.Abs(Theta) < 0.005 && Math.Abs(Zoom - 1.0) < 0.02;
            return b;        
        }

        public bool IsNegationOf(OffsetParameters second)
        {
            return Compose(this, second).IsCloseToIdentity();
        }

        public static bool TriangleEquality(OffsetParameters first, OffsetParameters second, OffsetParameters third)
        {
            OffsetParameters total = Compose(Compose(first, second), third.Invert());
            return total.IsCloseToIdentity();
        }

        // GROUP ACTION TYPE METHODS
        public double[] TransformCoordinates(double PreTransform_X, double PreTransform_Y) 
        {   
            double PostRotation_X = Zoom * (PreTransform_X * Math.Cos(Theta) + PreTransform_Y * Math.Sin(Theta));
            double PostRotation_Y = Zoom * (PreTransform_Y * Math.Cos(Theta) - PreTransform_X * Math.Sin(Theta));

            return (new double[] { PostRotation_X + X, PostRotation_Y + Y } );
        } 

        public OffsetParameters Invert()
        {            
            // Let X be the pretransformation coordinate vector and R(t,k) the rotation & scaling matrix.
            // X' = R(t, k) * X + trans.       Hence:   X = R^-1(t, k) (X' - trans.) 
            // So the translation in the inverse transformation is R^-1(t, k) ( - trans.)

            double Inverted_X = - (X * Math.Cos(-Theta) + Y * Math.Sin(-Theta));
            double Inverted_Y = - (Y * Math.Cos(-Theta) + X * Math.Sin(-Theta));

            return new OffsetParameters(Inverted_X, Inverted_Y, -Theta, 1.0 / Zoom); // TODO, this is probably wrong
        }

        public static OffsetParameters Compose(OffsetParameters FirstTransform, OffsetParameters LastTransform)
        {
            double EffectiveAngle = FirstTransform.Theta + LastTransform.Theta;
            double EffectiveZoom = FirstTransform.Zoom  * LastTransform.Zoom;
            
            // The 2nd transformation's rotation/zoom composes with and affects the translation from the first transform.
            double[] TransformedTranslation = LastTransform.TransformCoordinates(FirstTransform.X, FirstTransform.Y);
            double EffectiveX = TransformedTranslation[0] + LastTransform.X;
            double EffectiveY = TransformedTranslation[1] + LastTransform.Y;

            return new OffsetParameters( EffectiveX, EffectiveY, EffectiveAngle, EffectiveZoom);
        }


    }
}
