namespace ImageStackerConsole.Alignmnet
{
    public class AlignmentOrchestrator
    {

        private string[] ImagePaths;
        private IAlignmentMethod AlignmentMethod;
        public bool allPairs {get; set;} = false;

        private LoadingBar AlignmentLoadingBar;

        // LoadingBar<OffsetParameters[][]>


        public AlignmentOrchestrator(string[] paths, IAlignmentMethod method)
        {
            ImagePaths = paths;
            AlignmentMethod = method;
        }

        public AlignedImages CalculateOffsetParameterTable()
        {
            AlignmentLoadingBar = new LoadingBar();
            OffsetParameters[,] OffsetParametersArray = new OffsetParameters[ImagePaths.Length, ImagePaths.Length];

            // Calculate the first row.
            RGBImage jImage = new RGBImage(ImagePaths[0]);
            for (int i = 0; i < OffsetParametersArray.GetLength(0) ; i++)
            {
                RGBImage iImage = new RGBImage(ImagePaths[i]);
                OffsetParametersArray[0,i] =  AlignmentMethod.CalculateOffsetParameters(jImage, jImage, AlignmentLoadingBar);

                // TODO, set progress!
            }

            // Calculate all other rows either explicitly, or by composing entries in the first row.
            for (int j = 1; j < OffsetParametersArray.GetLength(0); j++)
            {
                if (allPairs)
                {
                    jImage = new RGBImage(ImagePaths[j]);
                    for (int i = 0; i < OffsetParametersArray.GetLength(1); i++)
                    {
                        RGBImage iImage = new RGBImage(ImagePaths[i]);
                        OffsetParametersArray[j, i] = AlignmentMethod.CalculateOffsetParameters(jImage, jImage, AlignmentLoadingBar);
                    }
                }
                else
                {
                    for (int i = 1; i < OffsetParametersArray.GetLength(1); i++)
                    {
                        RGBImage iImage = new RGBImage(ImagePaths[i]);

                        // j -> i = (0 -> j)^-1 . (0 -> i)   - but remember, the right-most one acts first
                        OffsetParametersArray[j, i] = OffsetParameters.Compose( OffsetParametersArray[0, i], (OffsetParametersArray[0, j]).Invert() );
                    }
                }
            }

            return  (new AlignedImages(ImagePaths, OffsetParametersArray));
        }







    }
}
