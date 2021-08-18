﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace GangyiWang
{
    public class Kernel
    {
        private readonly Mat _image;


        private static readonly Point[] _directions =
        {
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1), new Point(0, 1), new Point(-1, 1),
            new Point(-1, 0),
        };

        private bool[] _kernelData;

        public Kernel(Mat image)
        {
            _image = image;
        }

        public void GetKernel(int y, int x)
        {
            var kernel = new List<bool>();
            foreach (var direction in _directions)
                kernel.Add(IsValid(y + direction.Y, x + direction.X) &&
                           _image.At<byte>(y + direction.Y, x + direction.X) > 0);
            _kernelData = kernel.ToArray();
        }

        public IEnumerable<Point> AllActiveKernelPositions
        {
            get
            {
                for (int i = 0; i < _kernelData.Length; i++)
                {
                    if (!_kernelData[i]) continue;
                    yield return _directions[i];
                }
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

        private bool IsValid(int y, int x)
            => (x >= 0 && x < _image.Width) && (y >= 0 && y < _image.Height);

        public int CountNeighbors()
            => _kernelData.Aggregate(0, (ctr, val) => ctr += val ? 1 : 0);

    }
}