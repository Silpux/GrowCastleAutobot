using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes.MouseMove
{
    public class Curve
    {
        private readonly List<PointF> points = new();

        private float min, max;
        public Curve(float min, float max, int intermediateCount)
        {
            Random rand = new Random();

            this.min = min;
            this.max = max;

            int totalPoints = intermediateCount + 2;

            for (int i = 0; i < totalPoints; i++)
            {
                float x = (float)i / (totalPoints - 1);
                float y = min + (float)rand.NextDouble() * (max - min);
                points.Add(new PointF(x, y));
            }
        }

        public float GetValue(float t)
        {
            t = Math.Clamp(t, 0f, 1f);

            int segmentCount = points.Count - 1;
            float scaledT = t * segmentCount;
            int i = Math.Min((int)scaledT, segmentCount - 1);
            float localT = scaledT - i;

            PointF p0 = GetSafePoint(i - 1);
            PointF p1 = GetSafePoint(i);
            PointF p2 = GetSafePoint(i + 1);
            PointF p3 = GetSafePoint(i + 2);

            return Math.Clamp(CatmullRom(p0.Y, p1.Y, p2.Y, p3.Y, localT), min, max);
        }

        private PointF GetSafePoint(int index)
        {
            index = Math.Clamp(index, 0, points.Count - 1);
            return points[index];
        }

        private static float CatmullRom(float y0, float y1, float y2, float y3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * (2 * y1 + (y2 - y0) * t + (2 * y0 - 5 * y1 + 4 * y2 - y3) * t2 + (-y0 + 3 * y1 - 3 * y2 + y3) * t3);
        }
    }
}
