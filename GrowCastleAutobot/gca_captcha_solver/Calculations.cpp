#include "Calculations.h"

using namespace cv;

double distance(cv::Point2f p1, cv::Point2f p2) {
    return sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
}

void linearRegression(const std::vector<Point2f>& points, double& m, double& c) {
    int n = points.size();
    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

    for (const auto& p : points) {
        sumX += p.x;
        sumY += p.y;
        sumXY += p.x * p.y;
        sumX2 += p.x * p.x;
    }

    m = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
    c = (sumY - m * sumX) / n;
}



cv::Scalar interpolateScalar(const cv::Scalar& start, const cv::Scalar& end, double t) {
    if (t < 0)t = 0;
    if (t > 1) t = 1;

    cv::Scalar result;
    for (int i = 0; i < 4; ++i) {
        result[i] = start[i] + t * (end[i] - start[i]);
    }

    return result;
}



Point2f quadraticBezierDerivative(Point2f start, Point2f control, Point2f end, float t) {
    return 2 * (1 - t) * (control - start) + 2 * t * (end - control);
}





Point2f getQuadraticBezierPoint(const Point2f& ps, const Point2f& pc, const Point2f pe, double t) {

    double x = (1 - t) * (1 - t) * ps.x + 2 * t * (1 - t) * pc.x + t * t * pe.x;
    double y = (1 - t) * (1 - t) * ps.y + 2 * t * (1 - t) * pc.y + t * t * pe.y;

    return Point2f(x, y);

}





