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
        private static Mat[] _golayElements = null;

        private static Mat[] GolayElements => _golayElements ??= CreateGolayElements();

        public static Mat PerformOn(Mat original)
        {
            var thinned = original.Clone();

            while (true)
            {
                var hitMissResults =
                    GolayElements.Select(golayElement => thinned.MorphologyEx(MorphTypes.HitMiss, golayElement));

                var hitMissResult = hitMissResults.Aggregate(new Mat(thinned.Size(), MatType.CV_8U, 0),
                    (result, act) => result | act);

                if (hitMissResult.CountNonZero() == 0)
                    break;

                thinned = (thinned - hitMissResult).ToMat();
            }

            return thinned;
        }

        private static Mat[] CreateGolayElements()
        {
            var rotations = new[]
            {
                Cv2.GetRotationMatrix2D(new Point2f(1, 1), 90, 1),
                Cv2.GetRotationMatrix2D(new Point2f(1, 1), 180, 1),
                Cv2.GetRotationMatrix2D(new Point2f(1, 1), 270, 1)
            };

            var golaySe1 = new Mat(3, 3, MatType.CV_16S, new short[] {-1, -1, -1, 0, 1, 0, 1, 1, 1});
            var golaySe2 = new Mat(3, 3, MatType.CV_16S, new short[] {0, -1, -1, 1, 1, -1, 0, 1, 0});

            return new [] {
                golaySe1, golaySe1.WarpAffine(rotations[0], golaySe1.Size()),
                golaySe1.WarpAffine(rotations[1], golaySe1.Size()),
                golaySe1.WarpAffine(rotations[2], golaySe1.Size()),
                golaySe2, golaySe2.WarpAffine(rotations[0], golaySe2.Size()),
                golaySe2.WarpAffine(rotations[1], golaySe2.Size()),
                golaySe2.WarpAffine(rotations[2], golaySe2.Size())
            };

        }

    }
}