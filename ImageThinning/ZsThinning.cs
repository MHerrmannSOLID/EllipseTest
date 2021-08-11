using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace GangyiWang
{
    public class ZsThinning
    {
        //(Y.  Zhang  and  C.  Y.  Suen ) -  A  Fast  Parallel  Thinning  Algorithm 
        // Paper: http://agcggs680.pbworks.com/f/Zhan-Suen_algorithm.pdf
        // Good explanation: https://rosettacode.org/wiki/Zhang-Suen_thinning_algorithm
        //
        // This is just a reference implementation optimized for readability not for speed.

        public static Mat PerformOn(Mat original)
        {
            var thinner = new ZsThinning(original);
            return thinner._inputImg;
        }

        private readonly Mat _inputImg;
        private readonly object _listLock = new object();

        private ZsThinning(Mat inputImage)
        {
            
            _inputImg = inputImage.Clone();

            var changeSetCond1 = new List<Point>();
            var changeSetCond2 = new List<Point>();
#if DEBUG
            var stw = new Stopwatch();
            stw.Start();
#endif
            do
            {
                changeSetCond1 = SearchForChangeConditions(Step1Condition);
                SwitchToBlack(changeSetCond1);
                changeSetCond2 = SearchForChangeConditions(Step2Condition);
                SwitchToBlack(changeSetCond2);
            } while (changeSetCond1.Count > 0 || changeSetCond2.Count > 0);
#if DEBUG
            stw.Stop();
            Console.WriteLine($"ZsThinning processing time: {stw.ElapsedMilliseconds}ms");
#endif
        }

        private void SwitchToBlack(List<Point> pixelToSwitch)
        {
            foreach (var point in pixelToSwitch)
                _inputImg.Set(point.Y, point.X, (byte) 0);
        }

        private bool Step1Condition(bool[] kernel)
        {
            var setA = CalcA(kernel);
            var setB = CalcB(kernel);
            var test1 = setB >= 2 && setB <= 6;
            var test2 = setA == 1;
            var test3 = !kernel[0] || !kernel[2] || !kernel[4];
            var test4 = !kernel[2] || !kernel[4] || !kernel[6];
            return test1 && test2 && test3 && test4;
        }

        private bool Step2Condition(bool[] kernel)
        {
            int setA = CalcA(kernel);
            int setB = CalcB(kernel);
            var test1 = setB >= 2 && setB <= 6;
            var test2 = setA == 1;
            var test3 = !kernel[0] || !kernel[2] || !kernel[6];
            var test4 = !kernel[0] || !kernel[4] || !kernel[6];
            return test1 && test2 && test3 && test4;
        }

        private void CalcSets(bool[] kernel,out int a, out int B)
        {
            a = 0;
            B = 0;
            for (int i = 0; i < kernel.Length - 1; i++)
            {
                a += (!kernel[i] && kernel[i + 1]) ? 1 : 0;
                B += (kernel[i]) ? 1 : 0;
            }
        }

        private int CalcA(bool[] kernel)
        {
            var a = 0;
            for (int i = 0; i < kernel.Length - 1; i++)
                a += (!kernel[i] && kernel[i + 1]) ? 1 : 0;
            return a;
        }

        private int CalcB(bool[] kernel)
        {
            var B = 0;
            for (int i = 0; i < kernel.Length - 1; i++)
                B += (kernel[i]) ? 1 : 0;
            return B;
        }


        private List<Point> SearchForChangeConditions(Func<bool[], bool> isConditionMet)
        {
            var changesPositions = new List<Point>();

            Parallel.For(1, _inputImg.Width - 1, x =>
            {
                for (int y = 1; y < _inputImg.Height - 1; y++)
                    if (_inputImg.At<byte>(y, x) > 0 && isConditionMet(GetKernelAt(y, x)))
                        lock (_listLock)
                            changesPositions.Add(new Point(x, y));
            });

            return changesPositions;
        }

        private bool[] GetKernelAt(int y, int x)
            => new[]
            {
                _inputImg.At<byte>(y - 1, x) > 0, _inputImg.At<byte>(y - 1, x + 1) > 0,
                _inputImg.At<byte>(y, x + 1) > 0, _inputImg.At<byte>(y + 1, x + 1) > 0,
                _inputImg.At<byte>(y + 1, x) > 0, _inputImg.At<byte>(y + 1, x - 1) > 0,
                _inputImg.At<byte>(y, x - 1) > 0, _inputImg.At<byte>(y - 1, x - 1) > 0,
                _inputImg.At<byte>(y - 1, x) > 0
            };
    }
}