namespace ImageStackerConsole.Alignment
{
    public class AlignmentOrchestrator
    {

        private string[] ImagePaths;
        private IAlignmentMethod AlignmentMethod;
        // public delegate OffsetParameters AlignPair(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar);

        public bool AllPairs {get; set;} = false;
        private LoadingBar AlignmentLoadingBar;

        public AlignmentOrchestrator(string[] paths, IAlignmentMethod method)
        {
            ImagePaths = paths;
            AlignmentMethod = method;
        }

        public AlignedImages CalculateOffsetParameterTable()
        {
            AlignmentLoadingBar = new LoadingBar();
            OffsetParameters[,] OffsetParametersArray = new OffsetParameters[ImagePaths.Length, ImagePaths.Length];

            // Populate the table
            for (int j = 0; j < OffsetParametersArray.GetLength(0); j++)
            {
                if (AllPairs || (j == 0))
                {
                    // Calculate the offset parameters explicity for the first row 
                    RGBImage jImage = new RGBImage(ImagePaths[j]);

                    for (int i = 0; i < OffsetParametersArray.GetLength(1); i++)
                    {
                        System.Console.WriteLine($"ALIGNING ({i},{j})");
                        RGBImage iImage = new RGBImage(ImagePaths[i]);
                        OffsetParametersArray[j, i] = AlignmentMethod.CalculateOffsetParameters(jImage, iImage, AlignmentLoadingBar);
                    }
                }
                else
                {
                    for (int i = 0; i < OffsetParametersArray.GetLength(1); i++)
                    {
                        // j -> i = (0 -> j)^-1 . (0 -> i)   - but remember, the right-most transform acts first
                        System.Console.WriteLine($"DEBUG: Calculating: ({j},{i})");
                        System.Console.WriteLine("Composing: " + OffsetParametersArray[0, i].GetStringRepresentation());
                        System.Console.WriteLine("Calculating inverse of: " + OffsetParametersArray[0, j].GetStringRepresentation());
                        System.Console.WriteLine("Inverse is: "  + OffsetParametersArray[0, j].CalculateInverse().GetStringRepresentation());
                        OffsetParametersArray[j, i] = OffsetParameters.Compose(OffsetParametersArray[0, i], OffsetParametersArray[0, j].CalculateInverse());
                        System.Console.WriteLine("Composition is:" + OffsetParametersArray[0, j].CalculateInverse().GetStringRepresentation());
                    }
                }
            }

            System.Console.WriteLine("Finished Alignment.");
            return  (new AlignedImages(ImagePaths, OffsetParametersArray));
        }
    }
}
