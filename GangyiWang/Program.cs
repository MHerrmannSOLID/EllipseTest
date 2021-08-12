using ImageThinning;
using OpenCvSharp;

namespace GangyiWang
{
    public class Program
    {

        static void Main(string[] args)
        {
            var img = new Mat("TassenTest.png", ImreadModes.Grayscale);

            var contourImage = img.Canny(60, 120).GuoHallThinning();
            
            Cv2.ImShow("Contours", contourImage);
            Cv2.ImShow("Original", img);
            Cv2.WaitKey();
        }
    }

   
}
