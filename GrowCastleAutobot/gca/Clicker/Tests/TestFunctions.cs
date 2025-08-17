using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Clicker.Tests
{
    public static class TestFunctions
    {
        public static IEnumerable<(int x, int y)> GetCirclePointsClockwise(int centerX, int centerY, int radius)
        {
            var points = new HashSet<(int x, int y)>();

            int x = 0;
            int y = radius;
            int d = 1 - radius;

            while (x <= y)
            {
                points.Add((centerX + x, centerY + y));
                points.Add((centerX + y, centerY + x));
                points.Add((centerX + y, centerY - x));
                points.Add((centerX + x, centerY - y));
                points.Add((centerX - x, centerY - y));
                points.Add((centerX - y, centerY - x));
                points.Add((centerX - y, centerY + x));
                points.Add((centerX - x, centerY + y));

                x++;

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }
            }

            return points
                .OrderByDescending(p =>
                {
                    double angle = Math.Atan2(p.y - centerY, p.x - centerX);
                    return angle;
                });
        }

        public static IEnumerable<(int x, int y)> GetRectangleBorderClockwise(int left, int top, int width, int height)
        {
            int right = left + width - 1;
            int bottom = top + height - 1;

            for (int x = left; x <= right; x++)
            {
                yield return (x, top);
            }

            for (int y = top + 1; y <= bottom; y++)
            {
                yield return (right, y);
            }

            for (int x = right - 1; x >= left; x--)
            {
                yield return (x, bottom);
            }

            for (int y = bottom - 1; y > top; y--)
            {
                yield return (left, y);
            }
        }

        public static IEnumerable<(int x, int y)> GetSpiral(int centerX, int centerY, int steps, int expansionStep = 100)
        {
            var result = new List<(int x, int y)>();
            int x = centerX, y = centerY;

            result.Add((x, y));

            int dx = 1, dy = 0;
            int segmentLength = 1;
            int segmentPassed = 0;
            int segmentCount = 0;

            for (int i = 1; i < steps; i++)
            {
                x += dx;
                y += dy;
                result.Add((x, y));
                segmentPassed++;

                if (segmentPassed == segmentLength)
                {
                    segmentPassed = 0;

                    int temp = dx;
                    dx = -dy;
                    dy = temp;

                    segmentCount++;

                    if (segmentCount % 2 == 0)
                    {
                        segmentLength += expansionStep;
                    }
                }
            }

            return result;
        }
    }
}
