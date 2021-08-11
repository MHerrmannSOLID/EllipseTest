using System;
using GangyiWang;
using OpenCvSharp;

namespace ImageThinning
{
    public class Program
    { 
        static void Main(string[] args)
        {
            // TODO: Needs to be implemented!!!!!
            // https://web.archive.org/web/20160314104646/http://opencv-code.com/quick-tips/implementation-of-guo-hall-thinning-algorithm/

            var original = new Mat("ThinningTest.png", ImreadModes.Grayscale);

            Cv2.ImShow("Zhang Suen Thinning ", original.ZhangSuenThinning());
            Cv2.ImShow("Morphological Thinning ", original.MorphologicalThinning());
            
            Cv2.ImShow("Original", original);
            
            Cv2.WaitKey();
        }
    } 
}
