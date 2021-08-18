using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace GangyiWang
{
    public class Kernel
    {
        private static readonly Point[] _directions =
        {
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1), new Point(0, 1), new Point(-1, 1),
            new Point(-1, 0),
        };

        private readonly Mat _image;
        private bool[] _kernelData;

        public Kernel(Mat image)
            => _image = image;
        

        public void InitializeAt(Point pos)
            => _kernelData = _directions.Select(direction => IsValid(pos + direction) &&
                                                             IsPosActive(pos + direction)).ToArray();
        
        private bool IsPosActive(Point pos)
            => _image.At<byte>(pos.Y, pos.X) > 0;
        

        public IEnumerable<Point> AllActiveKernelPositions
        {
            get
            {
                for (int i = 0; i < _kernelData.Length; i++)
                    if (_kernelData[i])
                        yield return _directions[i];
            }
        }

        public Point FirstActiveDirection
        {
            get
            {
                for (int i = 0; i < _kernelData.Length; i++)
                    if (_kernelData[i])
                        return _directions[i];
                throw new InvalidOperationException("There is no active direction!");
            }
        }

        private bool IsValid(Point pt)
            => (pt.X >= 0 && pt.X < _image.Width) && (pt.Y >= 0 && pt.Y < _image.Height);

        public int CountNeighbors()
            => _kernelData.Aggregate(0, (ctr, val) => ctr += val ? 1 : 0);

    }
}