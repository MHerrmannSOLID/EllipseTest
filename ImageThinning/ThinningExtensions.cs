using System;
using System.Collections.Generic;
using System.Text;
using GangyiWang;
using OpenCvSharp;

namespace ImageThinning
{
    public static class ThinningExtensions
    {

        public static Mat MorphologicalThinning(this Mat original)
            => ImageThinning.MorphologicalThinning.PerformOn(original.Threshold(20, 255, ThresholdTypes.Binary));

        public static Mat ZhangSuenThinning(this Mat original)
            => ZsThinning.PerformOn(original.Threshold(20, 255, ThresholdTypes.Binary));


        public static Mat GuoHallThinning(this Mat original)
            => GhThinning.PerformOn(original.Threshold(20, 255, ThresholdTypes.Binary));


    }
}
