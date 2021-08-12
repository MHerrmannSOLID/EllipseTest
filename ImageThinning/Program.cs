using System;
using GangyiWang;
using OpenCvSharp;

namespace ImageThinning
{
    public class Program
    { 
        // I've been trying different methods of contour image thinning here,
        // just to get a feeling about performance, regarding result quality.
        // However the implementations of the algorithms are not focused on
        // execution speed, but rather on readability! 
        // None of these test are performing optimal regarding the execution speed,
        // they are just sandbox tests....
        static void Main(string[] args)
        {
            var original = new Mat("ThinningTest.png", ImreadModes.Grayscale);

            Cv2.ImShow("Zhang Suen Thinning ", original.ZhangSuenThinning());
            Cv2.ImShow("Morphological Thinning ", original.MorphologicalThinning());
            Cv2.ImShow("Guo-Hall", original.GuoHallThinning());

            Cv2.ImShow("Original", original);
            Cv2.WaitKey();
        }
    }
}
