using System.Net;
using System.Text;
using OpenCvSharp;

namespace GangyiWang
{
    public static class OcvExtensions
    {
        public  delegate void PositionCallback(Point pos);

        public static void ForEachPosition(this Mat image, PositionCallback callback)
        {
            for (int x = 0; x < image.Width; x++)
            for (int y = 0; y < image.Height; y++)
                callback(new Point(x, y));
        }

        public static Kernel GetKernelAt(this Mat image, Point position)
        {
            var kernel = new Kernel(image);
            kernel.GetKernel(position);
            return kernel;
        }
    }
}
