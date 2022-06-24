using System.Collections.Generic;


namespace ImageStackerConsole.Alignment
{
    class StellarDBLoader
    {
        private string dataPath = @"C:\Users\Kier\Developing\ImageStackerConsole\StarDB.csv";


        private List<knownStar> importCSV() 
        {
            string[] lines = System.IO.File.ReadAllLines(dataPath);
            List<knownStar> starDB = new List<knownStar>();

            // TODO - make this dynamic
            int RAFieldNo  = 7; // 'RA or equi'
            int DecFieldNo = 8; // 'Dec or equi'
            int MagFieldNo = 11; // 'Parall.'
            int ParFieldNo = 15;  // 'Hp obs'

            for (int lineNo = 1; lineNo < lines.Length; lineNo++)
            {
                string[] starFields = lines[lineNo].Split(',');

                double RA  = double.Parse( starFields[RAFieldNo].Trim() );
                double Dec = double.Parse( starFields[DecFieldNo].Trim() );
                double Mag = double.Parse( starFields[MagFieldNo].Trim() );
                double Par = double.Parse( starFields[ParFieldNo].Trim() );
                string Name = (starFields[4] + "/" + starFields[5] + "/" +  starFields[6]).Trim();

                knownStar thisStar = new knownStar(RA, Dec, Mag, Par, Name);

                starDB.Add(thisStar);

            }

            return starDB;
        }

        private class knownStar
        {
            private double RA;
            private double Dec;
            private double Mag;
            private double Parallax;
            private string Name;
            private double par;

            public knownStar(double rA, double dec, double mag, double par, string name)
            {
                RA = rA;
                Dec = dec;
                Mag = mag;
                this.par = par;
                Name = name;
            }
        }

    }
}
