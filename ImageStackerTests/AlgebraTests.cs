using ImageStackerConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace ImageStackerTests
{
    [TestClass]
    public class AlgebraTests
    {
        OffsetParameters Param1 = new OffsetParameters(107500, 0, 0.01, 1.02);
        OffsetParameters Param2 = new OffsetParameters(114, 76, 0.025, 3.66);
        OffsetParameters Param3 = new OffsetParameters(-556, 50, -0.03, 0.17);

        double precision = 1E-7;

        public void VerifySame(OffsetParameters Method1, OffsetParameters Method2)
        {
            Assert.IsTrue( Math.Abs(Method1.X - Method2.X) <= precision);
            Assert.IsTrue( Math.Abs(Method1.Y - Method2.Y) <= precision);
            Assert.IsTrue( Math.Abs(Method1.Theta - Method2.Theta) <= precision);
            Assert.IsTrue( Math.Abs(Method1.Zoom - Method2.Zoom) <= precision);
        }

        [TestMethod]
        public void TestAssociativity()
        {
            OffsetParameters Method1 = OffsetParameters.Compose(Param1, OffsetParameters.Compose(Param2, Param3));
            OffsetParameters Method2 = OffsetParameters.Compose(OffsetParameters.Compose(Param1, Param2), Param3);

            Console.WriteLine("Confirm the different methods yield the same results indicates associativity holds:");
            Console.WriteLine(Method1.GetStringRepresentation());
            Console.WriteLine(Method2.GetStringRepresentation());
            
            VerifySame(Method1, Method2);
        }

        [TestMethod]
        public void TestInverseOfInverse()
        {
            Console.WriteLine("Inverse ");
            OffsetParameters Method1 = Param1.CalculateInverse().CalculateInverse();
            
            Console.WriteLine("Confirm the different methods yield the same results indicates inverse of inverse = self");
            Console.WriteLine(Param1.GetStringRepresentation());
            Console.WriteLine(Method1.GetStringRepresentation());

            VerifySame(Param1, Method1);
        }


        [TestMethod]
        public void TestInverseOfProduct() {

            Console.WriteLine("Inverse of product of 2: ");
            OffsetParameters Method1 = OffsetParameters.Compose(Param1, Param2).CalculateInverse();
            OffsetParameters Method2 = OffsetParameters.Compose(Param2.CalculateInverse(), Param1.CalculateInverse());

            Console.WriteLine(Method1.GetStringRepresentation());
            Console.WriteLine(Method2.GetStringRepresentation());
            Console.WriteLine("----------------------------------------------");

            VerifySame(Method1, Method2);
        }

    }
}
