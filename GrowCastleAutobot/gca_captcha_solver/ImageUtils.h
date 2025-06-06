#pragma once

#include "Main.h"

using namespace cv;
using namespace std;

vector<Point2f> findRotatedPart(const Mat& objectPart, const Mat& image, Mat& imageToDraw);
void SetConsoleColor(int color);

void drawQuadraticBezier(Mat& image, Point2f start, Point2f control, Point2f end, Scalar color, int thickness);
void drawQuadraticBezierWithArrow(Mat& image, Point2f start, Point2f control, Point2f end, Scalar color, int thickness);
void drawLineThroughPoint(cv::Mat& image, cv::Point point, double angle);

cv::Mat calculateHomography(const std::vector<cv::Point2f>& srcPoints, const std::vector<cv::Point2f>& dstPoints);
cv::Mat customFindHomography(const std::vector<cv::Point2f>& srcPoints, const std::vector<cv::Point2f>& dstPoints, int maxIterations = 2000, double reprojectionThreshold = 3.0);
