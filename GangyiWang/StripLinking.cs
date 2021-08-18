using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace GangyiWang
{
    public class StripLinking
    {
        private static readonly Random _rnd = new Random();
        private static readonly int _minStripLength = 5;
        private readonly Mat _contourImage;

        public Point[][] Strips { get; private set; }

        public static StripLinking PerformOn(Mat contourImg)
        {
            var toReturn = new StripLinking(contourImg);
            toReturn.ExtractStrips();
            return toReturn;
        }


        private StripLinking(Mat contourImg)
        {
            _contourImage = contourImg.Clone();
        }


        private void ExtractStrips()
        {
            var strips = new List<Point[]>();
            _contourImage.ForEachPosition((pt) =>
            {
                if (IsInactivePosition(pt)) return;

                var kernel = _contourImage.GetKernelAt(pt);

                if (kernel.CountNeighbors() == 2) return;

                RemovePixelAt(pt);
                foreach (var direction in kernel.AllActiveKernelPositions)
                {
                    var strip = FollowStrip(new List<Point> {pt}, pt + direction).ToArray();
                    if (strip.Length > _minStripLength)
                        strips.Add(strip);
                }
            });
            Strips = strips.ToArray();
        }

        private void RemovePixelAt(Point pt)
            => _contourImage.At<byte>(pt.Y, pt.X) = 0;


        private bool IsInactivePosition(Point pt)
            => _contourImage.At<byte>(pt.Y, pt.X) == 0;


        private List<Point> FollowStrip(List<Point> actStripPoints, Point actPoint)
        {
            var kernel = _contourImage.GetKernelAt(actPoint);
            actStripPoints.Add(actPoint);
            RemovePixelAt(actPoint);

            return kernel.CountNeighbors() != 1
                ? actStripPoints
                : FollowStrip(actStripPoints, actPoint + kernel.FirstActiveDirection);
        }

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