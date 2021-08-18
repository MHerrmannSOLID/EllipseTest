using System;
using ImageThinning;
using OpenCvSharp;

namespace GangyiWang
{
    public class GangyiWangEllipse
    {
        private readonly Mat _original;
        private Mat _contourImage;

        public  enum ThinningMethods
        {
            GuoHall =0,
            Zhangsuen = 1,
            Morphological = 2,
        }

        private readonly Func<Mat, Mat>[] _thinningFunctions = {
            (inoutImg) => inoutImg.GuoHallThinning(),
            (inoutImg) => inoutImg.ZhangSuenThinning(),
            (inoutImg) => inoutImg.MorphologicalThinning(),
        };

        public GangyiWangEllipse(Mat original)
        {
            _original = original;
        }

        public Mat ContourImage => _contourImage;

        public void PerformSearch()
        {
            var thinningMethodIdx = (int) ThinningMethod;
            var cannyFiltered = _original.Canny(CannyThreshold1, CannyThreshold2);
            _contourImage = _thinningFunctions[thinningMethodIdx](cannyFiltered);
            StripLinkage = StripLinking.PerformOn(_contourImage, minStripLength: 5);

        }

        public ThinningMethods ThinningMethod { get; set; } = 0;

        public StripLinking StripLinkage { get; private set; }

        public int CannyThreshold1 { get; set; } = 60;
        public int CannyThreshold2 { get; set; } = 120;
    }
}