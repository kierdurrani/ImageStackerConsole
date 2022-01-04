using ImageStackerConsole.Alignment;
namespace ImageStackerConsole.Stacking
{
    interface IStackingMethod
    {

        RGBImage StackImages(AlignedImages alignedImages, LoadingBar LoadingBar);

    }
}
