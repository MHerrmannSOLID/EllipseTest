using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace GangyiWang
{
    public class StripLinking
    {
        private static readonly int _minStripLength = 5;
        private readonly Mat _contourImage;

        public Point[][] Strips { get; private set; }

        private static readonly int[][] _directions =
        {
            new[] {-1, -1}, new[] {-1, 0}, new[] {-1, 1},
            new[] {0, 1},
            new[] {1, 1}, new[] {1, 0}, new[] {1, -1},
            new[] {0, -1},
        };

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
            for (int x = 0; x < _contourImage.Width; x++)
            {
                for (int y = 0; y < _contourImage.Height; y++)
                {
                    var actColor = _contourImage.At<byte>(y, x);
                    if (actColor == 0)
                        continue;

                    var kernel = GetKernel(y, x);
                    var neighbors = CountNeighbors(kernel);

                    if (neighbors == 2)
                        continue;

                    _contourImage.At<byte>(y, x) = 0;
                    for (int i = 0; i < kernel.Length; i++)
                    {
                        if (!kernel[i]) continue;

                        var strip = FollowStrip(new List<Point> { new Point(x, y) }, new Point(x + _directions[i][1], y + _directions[i][0])).ToArray();
                        if (strip.Length > _minStripLength)
                            strips.Add(strip);
                    }
                }

            }

            Strips = strips.ToArray();
        }

        private List<Point> FollowStrip(List<Point> actStripPoints, Point actPoint)
        {
            var kernel = GetKernel(actPoint.Y, actPoint.X);
            actStripPoints.Add(actPoint);
            _contourImage.At<byte>(actPoint.Y, actPoint.X) = 0;
            var neighbors = CountNeighbors(kernel);
            if (neighbors != 1) return actStripPoints;
            int i = 0;
            for (; i < kernel.Length; i++)
                if (kernel[i])
                    break;

            return FollowStrip(actStripPoints, new Point(actPoint.X + _directions[i][1], actPoint.Y + _directions[i][0]));
        }

        private int CountNeighbors(bool[] kernel)
            => kernel.Aggregate(0, (ctr, val) => ctr += val ? 1 : 0);


        private bool[] GetKernel(int y, int x)
        {
            var kernel = new List<bool>();
            foreach (var direction in _directions)
                kernel.Add(IsValid(y + direction[0], x + direction[1]) && _contourImage.At<byte>(y + direction[0], x + direction[1]) > 0);

            return kernel.ToArray();
        }

        private bool IsValid(int y, int x)
            => (x >= 0 && x < _contourImage.Width) && (y >= 0 && y < _contourImage.Height);


        public Mat CreateColoredStripsImage()
        {
            Mat stripImage = new Mat(_contourImage.Size(), MatType.CV_8UC3, 0);
            var rng = new Random();
            for (int i = 0; i < Strips.Length; i++)
            {
                var color = new Vec3b();
                color.Item0 = (byte)rng.Next(0, 256);
                color.Item1 = (byte)rng.Next(0, 256);
                color.Item2 = (byte)rng.Next(0, 256);
                for (int j = 0; j < Strips[i].Length; j++)
                    stripImage.At<Vec3b>(Strips[i][j].Y, Strips[i][j].X) = color;

            }
            return stripImage;
        }
    }
}