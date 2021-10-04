using System;
using System.Linq;

namespace ImageStackerConsole.Alignmnet
{
    public class AlignedImages
    {
        private string[] ImagePaths { get; }
        OffsetParameters[,] OffsetParameterTable { get; }

        public AlignedImages(string[] Paths, OffsetParameters[,] alignmentParameters)
        {
            ImagePaths = Paths;
        }

        public string[] GetStringRepresentation()
        {
            string[] Output = Enumerable.Repeat(string.Empty, ImagePaths.Length + 1).ToArray(); // empty string array

            foreach (string ImagePath in ImagePaths)
            {
                Output[0] += (ImagePath + ";");
            }

            for (int y = 0; y <= OffsetParameterTable.GetLength(0) ; y++)
            {
                for (int x = 0; x <= OffsetParameterTable.GetLength(0); x++)
                {
                    Output[y + 1] += OffsetParameterTable[y,x].GetStringRepresentation() + ";";
                }
            }
            return Output;
        }

       
        public static AlignedImages ImportAlignmentParameters(string FilePath)
        {

            string[] lines = System.IO.File.ReadAllLines(FilePath);

            string[] imagePaths = lines[0].Split(';');

            OffsetParameters[,] offsetParameterTable = new OffsetParameters[imagePaths.Length, imagePaths.Length];
            for (int j = 0; j < imagePaths.Length; j++)
            {
                string[] lineOfCoords = lines[j + 1].Split(';');
                for (int i = 0; i < imagePaths.Length; i++)
                {
                    offsetParameterTable[j, i] = new OffsetParameters(lineOfCoords[i]);
                } 
            }
            
            return new AlignedImages(imagePaths, offsetParameterTable);
        }

        // TODO METHODS
        public bool IsConsistent()
        {
            return true;
        }

    }
}
