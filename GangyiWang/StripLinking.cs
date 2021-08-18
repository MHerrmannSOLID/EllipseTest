using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace GangyiWang
{
    public class StripLinking
    {
        private static readonly Random _rnd = new Random();
        private readonly Mat _contourImage;

        public List<Point[]> Strips { get; private set; } = new List<Point[]>();

        public static StripLinking PerformOn(Mat contourImg, int minStripLength =5)
        {
            var toReturn = new StripLinking(contourImg) {MinStripLength = minStripLength};
            toReturn.ExtractStrips();
            toReturn.ExtractLoops();
            return toReturn;
        }

        private StripLinking(Mat contourImg)
            => _contourImage = contourImg.Clone();


        public int MinStripLength { get; set; } = 5;

        private void ExtractStrips()
        {
            _contourImage.ForEachPosition((actPos) =>
            {
                if (IsInactivePosition(actPos)) return;
                var kernel = _contourImage.GetKernelAt(actPos);
                if (IsIntermediatePos(kernel)) return;

                RemovePixelAt(actPos);
                foreach (var direction in kernel.AllActiveKernelPositions)
                {
                    var strip = FollowStrip(new List<Point> {actPos}, actPos + direction).ToArray();
                    if (strip.Length > MinStripLength)
                        Strips.Add(strip);
                }
            });
        }

        private void ExtractLoops()
        {
            _contourImage.ForEachPosition((actPos) =>
            {
                if (IsInactivePosition(actPos)) return;
                var kernel = _contourImage.GetKernelAt(actPos);

                RemovePixelAt(actPos);
                foreach (var direction in kernel.AllActiveKernelPositions)
                {
                    var strip = FollowStrip(new List<Point> { actPos }, actPos + direction).ToArray();
                    if (strip.Length > MinStripLength)
                        Strips.Add(strip);
                }
            });
        }

        private List<Point> FollowStrip(List<Point> actStripPoints, Point actPoint)
        {
            var kernel = _contourImage.GetKernelAt(actPoint);
            actStripPoints.Add(actPoint);
            RemovePixelAt(actPoint);

            return kernel.CountNeighbors() != 1
                ? actStripPoints
                : FollowStrip(actStripPoints, actPoint + kernel.FirstActiveDirection);
        }

        private static bool IsIntermediatePos(Kernel kernel)
            => kernel.CountNeighbors() == 2;

        private void RemovePixelAt(Point pt)
            => _contourImage.At<byte>(pt.Y, pt.X) = 0;

        private bool IsInactivePosition(Point pt)
            => _contourImage.At<byte>(pt.Y, pt.X) == 0;

        public Mat CreateColoredStripsImage()
        {
            Mat stripImage = new Mat(_contourImage.Size(), MatType.CV_8UC3, 0);

            foreach (var strip in Strips) DrawStrip(strip, stripImage);
            return stripImage;
        }

        private void DrawStrip(Point[] strip, Mat stripImage)
        {
            var color = GetRandomColor();
            for (int j = 0; j < strip.Length; j++)
                stripImage.At<Vec3b>(strip[j].Y, strip[j].X) = color;
        }

        private  Vec3b GetRandomColor()
            => new Vec3b
            {
                Item0 = (byte) _rnd.Next(0, 255), Item1 = (byte) _rnd.Next(0, 255), Item2 = (byte) _rnd.Next(0, 255)
            };
    }
}