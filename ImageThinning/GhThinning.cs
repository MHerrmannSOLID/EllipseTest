using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OpenCvSharp;

namespace ImageThinning
{
    public static class GhThinning
    {
        // Not sure how long this link will be alive, but this was the starting point for the implementation:
        // https://web.archive.org/web/20160314104646/http://opencv-code.com/quick-tips/implementation-of-guo-hall-thinning-algorithm/

        // Parallel thinning with two-subiteration algorithms
        // Unfortunately no free link .... :-(
        // https://dl.acm.org/doi/10.1145/62065.62074

        // This paper "A Comparative Study of Fingerprint Thinning Algorithms" also contains
        // a short description of the Guo-Hall algorithm (page 3, section "B. Guo-Hal")
        //https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.232.1226&rep=rep1&type=pdf

        public static Mat PerformOn(Mat originalImage)
        {
            Mat image = originalImage.Clone();
            var changeCounter = 0;

            do
            {
                changeCounter = image.PerformIterationStep(EvenCondition);
                changeCounter+= image.PerformIterationStep(OddCondition);
            } while (changeCounter > 0); 

            return image;
        }

        private static int PerformIterationStep(this Mat originalImage,Func<bool[],bool> stepCondition)
        {
            var emptyImage = new Mat(originalImage.Size(), MatType.CV_8U, 0);
            var count = 0;
            Parallel.For(1, originalImage.Rows, i =>
            {
                for (int j = 1; j < originalImage.Cols; j++)
                {
                    if (originalImage.IsBlackPoint(i, j)) continue;

                    var kernel = originalImage.CreateKernelAt(i, j);
                    var c = CalcC(kernel);
                    var n = CalcN(kernel);
                    var m = stepCondition(kernel);

                    if (!(c == 1 && n >= 2 && n <= 3 && !m))  continue;

                    emptyImage.At<byte>(i, j) = 255;
                    count++;
                }
            });
            (originalImage - emptyImage).ToMat().CopyTo(originalImage);
            return count;
        }

        private static bool IsBlackPoint(this Mat originalImage, int i, int j)
        {
            return originalImage.At<byte>(i, j) == 0;
        }

        private static bool[] CreateKernelAt(this Mat originalImage, int i, int j)
            => new[]
            {
                originalImage.At<byte>(i - 1, j) > 0,
                originalImage.At<byte>(i - 1, j + 1) > 0,
                originalImage.At<byte>(i, j + 1) > 0,
                originalImage.At<byte>(i + 1, j + 1) > 0,
                originalImage.At<byte>(i + 1, j) > 0,
                originalImage.At<byte>(i + 1, j - 1) > 0,
                originalImage.At<byte>(i, j - 1) > 0,
                originalImage.At<byte>(i - 1, j - 1) > 0
            };

        private static bool EvenCondition(bool[] p)
        => (p[4] | p[5] | !p[7]) & p[6];
        
        private static bool OddCondition(bool[] p)
        => (p[0] | p[1] | !p[3]) & p[2];
        

    private static int CalcN(bool[] kernel)
        {
            int n1 = CalcN1(kernel);
            int n2 = CalcN2(kernel);

            return n1 < n2 ? n1 : n2;
        }

        private static int CalcN1(bool[] kernel) => new[]
        {
            (kernel[7] | kernel[0]), (kernel[1] | kernel[2]), (kernel[3] | kernel[4]), (kernel[5] | kernel[6])
        }.CountTrueConditions();


        private static int CalcN2(bool[] kernel) => new[]
        {
            (kernel[0] | kernel[1]), (kernel[2] | kernel[3]), (kernel[4] | kernel[5]), (kernel[6] | kernel[7])
        }.CountTrueConditions();


        // --> Let C(P) be the number of distinct 8 - connected components of 1's in Ps 8-neighborhood. <---
        //
        // Since this is a bit hard to understand, I searched for a better explanation and found it in: 
        // "Thinning algorithms comparison for vectorization of engineering drawings" on page 3
        //https://www.researchgate.net/publication/316476002_Comparison_of_thinning_algorithms_for_vectorization_of_engineering_drawings/link/590848f60f7e9bc0d59ae365/download
        private static int CalcC(bool[] kernel)
            => new[]
            {
                (!kernel[0] & (kernel[1] | kernel[2])), (!kernel[2] & (kernel[3] | kernel[4])),
                (!kernel[4] & (kernel[5] | kernel[6])), (!kernel[6] & (kernel[7] | kernel[0]))
            }.CountTrueConditions();


        private static int CountTrueConditions(this bool[] conditions)
            => conditions.Aggregate(0, (sumActive, actCond) => sumActive += actCond ? 1 : 0);

    }
}