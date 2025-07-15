using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gca_clicker.Classes.Utils;

namespace gca_clicker.Classes.MouseMove
{
    public class BezierMouseMovement : MouseMovement
    {
        private PointF cp1;
        private PointF cp2;
        private PointF cp3;

        private List<float> Ts = new();
        private List<float> Lengths = new();

        private float currentLength = 0;
        private float curveLength = 0;

        private Curve curve = null!;

        public BezierMouseMovement(PointF start, PointF cp1, PointF cp2, PointF cp3, PointF end) : base(start, end)
        {
            this.cp1 = cp1;
            this.cp2 = cp2;
            this.cp3 = cp3;

            Init();
        }

        public BezierMouseMovement(PointF start, PointF end) : base(start, end)
        {

            (PointF rectLU, PointF rectRD) = GetBoundsWithPadding(start, end, Distance(start, end) * 0.05f);

            Random rand = new Random();
            cp1.X = Math.Clamp(rectLU.X + (float)rand.NextDouble() * (rectRD.X - rectLU.X), 0, WinAPI.width);
            cp1.Y = Math.Clamp(rectLU.Y + (float)rand.NextDouble() * (rectRD.Y - rectLU.Y), 0, WinAPI.height);

            cp2.X = Math.Clamp(rectLU.X + (float)rand.NextDouble() * (rectRD.X - rectLU.X), 0, WinAPI.width);
            cp2.Y = Math.Clamp(rectLU.Y + (float)rand.NextDouble() * (rectRD.Y - rectLU.Y), 0, WinAPI.height);

            cp3.X = Math.Clamp(rectLU.X + (float)rand.NextDouble() * (rectRD.X - rectLU.X), 0, WinAPI.width);
            cp3.Y = Math.Clamp(rectLU.Y + (float)rand.NextDouble() * (rectRD.Y - rectLU.Y), 0, WinAPI.height);

            Init();
        }

        private void Init(int resolution = 1000)
        {

            curve = new Curve(2, 4, 3);

            curveLength = 0f;
            PointF prev = GetPoint(0f);

            Ts.Add(0f);
            Lengths.Add(0f);

            for (int i = 1; i <= resolution; i++)
            {
                float t = i / (float)resolution;
                PointF curr = GetPoint(t);
                curveLength += Distance(prev, curr);

                Ts.Add(t);
                Lengths.Add(curveLength);

                prev = curr;
            }
        }

        private float GetTForLength(float targetLength)
        {
            int low = 0, high = Lengths.Count - 1;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (Lengths[mid] < targetLength)
                    low = mid + 1;
                else
                    high = mid;
            }

            int i = Math.Max(1, low);
            float lenBefore = Lengths[i - 1];
            float lenAfter = Lengths[i];
            float tBefore = Ts[i - 1];
            float tAfter = Ts[i];

            float segmentLength = lenAfter - lenBefore;
            float ratio = (targetLength - lenBefore) / segmentLength;

            return tBefore + ratio * (tAfter - tBefore);
        }

        protected override PointF GetPoint(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            float uuuu = uuu * u;
            float tttt = ttt * t;

            float x = uuuu * startPoint.X
                    + 4 * uuu * t * cp1.X
                    + 6 * uu * tt * cp2.X
                    + 4 * u * ttt * cp3.X
                    + tttt * endPoint.X;

            float y = uuuu * startPoint.Y
                    + 4 * uuu * t * cp1.Y
                    + 6 * uu * tt * cp2.Y
                    + 4 * u * ttt * cp3.Y
                    + tttt * endPoint.Y;

            return new PointF(x, y);
        }

        protected override void IncreaseT()
        {
            float currentLengthT = currentLength / curveLength;
            currentLength += SpeedFactor * curve.GetValue(currentLengthT);

            if (currentLength >= curveLength)
            {
                t = 1f;
            }
            else
            {
                t = GetTForLength(currentLength);
            }
        }
    }
}
