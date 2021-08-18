using System.Collections.Generic;
using OpenCvSharp;

namespace GangyiWang
{
    public class Program
    {

        static void Main(string[] args)
        {
            var testImages = new[] {"TassenTest.png", "rivets.png", "seats.jpg", "simple.jpg"};
            var img = new Mat(testImages[1], ImreadModes.Grayscale);

            var gwe = new GangyiWangEllipse(img)
            {
                ThinningMethod = GangyiWangEllipse.ThinningMethods.GuoHall,
                CannyThreshold1 = 60,
                CannyThreshold2 = 120
            };
            gwe.PerformSearch();

            Cv2.ImShow("Strip extraction (debug)", gwe.StripLinkage.CreateColoredStripsImage());
            Cv2.ImShow("Canny contour image", gwe.ContourImage);
            Cv2.ImShow("Original image", img);
            Cv2.WaitKey();
        }

    }
}
