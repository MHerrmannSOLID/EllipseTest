using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace ImageThinning
{
    // References:

    // Nice presentation about morphological operation(and also thinning ~p 56) 
    // https://cs.brown.edu/courses/csci1290/labs/lab_compositing/Morphology.pdf

    // Book on Image processing
    // Digitale Bildverarbeitung , Jähne, Bernd (2005) page: 530
    public static class MorphologicalThinning
    {
        private static Mat[] _golayElements;
        private static Mat[] GolayElements => _golayElements ??= CreateGolayStructuredElements();

        public static Mat PerformOn(Mat original)
        {
            var thinned = original.Clone();
            var emptyImage = new Mat(thinned.Size(), MatType.CV_8U, 0);

            while (true)
            {
                var hitMissResults = GetGolayHitMissMapsFor(thinned);
                var disjunctMap = hitMissResults.CalcDisjunctMap(emptyImage);

                if (disjunctMap.CountNonZero() == 0) break;

                thinned = (thinned - disjunctMap).ToMat();
            }

            return thinned;
        }

        private static Mat CalcDisjunctMap(this IEnumerable<Mat> hitMissResults, Mat emptyImage)
            => hitMissResults.Aggregate(emptyImage, (result, act) => result | act);
        

        private static IEnumerable<Mat> GetGolayHitMissMapsFor(Mat thinned)
            => GolayElements.Select(golayElement => thinned.MorphologyEx(MorphTypes.HitMiss, golayElement));

        /// For the definition of the golay alphabet please see page 69 of:
        /// https://cs.brown.edu/courses/csci1290/labs/lab_compositing/Morphology.pdf
        private static Mat[] CreateGolayStructuredElements()
        {
            var seCenter = new Point2f(1, 1);
            var rotations = new[]
            {
                Cv2.GetRotationMatrix2D(seCenter, 90, 1),
                Cv2.GetRotationMatrix2D(seCenter, 180, 1),
                Cv2.GetRotationMatrix2D(seCenter, 270, 1)
            };

            var golayStructuredElement1 = new Mat(3, 3, MatType.CV_16S, new short[] {-1, -1, -1, 0, 1, 0, 1, 1, 1});
            var golayStructuredElement2 = new Mat(3, 3, MatType.CV_16S, new short[] {0, -1, -1, 1, 1, -1, 0, 1, 0});

            return new [] {
                golayStructuredElement1, golayStructuredElement1.WarpAffine(rotations[0], golayStructuredElement1.Size()),
                golayStructuredElement1.WarpAffine(rotations[1], golayStructuredElement1.Size()),
                golayStructuredElement1.WarpAffine(rotations[2], golayStructuredElement1.Size()),
                golayStructuredElement2, golayStructuredElement2.WarpAffine(rotations[0], golayStructuredElement2.Size()),
                golayStructuredElement2.WarpAffine(rotations[1], golayStructuredElement2.Size()),
                golayStructuredElement2.WarpAffine(rotations[2], golayStructuredElement2.Size())
            };

        }

    }
}