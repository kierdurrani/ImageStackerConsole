namespace ImageStackerConsole.AlignmnetMethods
{
    interface IAlignmentMethod
    {

        OffsetParameters CalculateOffsetParameters(RGBImage img1, RGBImage img2, LoadingBar loadingBar);


    }
}
