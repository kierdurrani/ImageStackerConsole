namespace ImageStackerConsole.Alignmnet
{
    public interface IAlignmentMethod
    {

        OffsetParameters CalculateOffsetParameters(RGBImage rgbImg1, RGBImage rgbImg2, LoadingBar loadingBar);


    }
}
