#pragma once

#include "Main.h"

double distance(cv::Point2f p1, cv::Point2f p2);
void linearRegression(const std::vector<cv::Point2f>& points, double& m, double& c);
cv::Scalar interpolateScalar(const cv::Scalar& start, const cv::Scalar& end, double t);
cv::Point2f quadraticBezierDerivative(cv::Point2f start, cv::Point2f control, cv::Point2f end, float t);
cv::Point2f getQuadraticBezierPoint(const cv::Point2f& ps, const cv::Point2f& pc, const cv::Point2f pe, double t);
