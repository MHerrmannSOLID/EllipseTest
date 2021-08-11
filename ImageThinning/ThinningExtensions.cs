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
            => ImageThinning.MorphologicalThinning.PerformOn(original);

        public static Mat ZhangSuenThinning(this Mat original)
            => ZsThinning.PerformOn(original);
    }
}
