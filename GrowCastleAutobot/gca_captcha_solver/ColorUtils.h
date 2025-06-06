#pragma once

#include "Main.h"
#include "Types.h"

using namespace cv;
using namespace std;

void COLORMODE(int mode, Mat& src);
set<Vec3b, Vec3bComparator> getDistinctColors(const Mat& image);
void replaceColorsWithBlack(Mat& image, const set<Vec3b, Vec3bComparator>& colorsToReplace);
int pxlCount(const Mat& image, const set<Vec3b, Vec3bComparator>& distinctColors, int sx, int sy, int fx, int fy);
int pxlCount(const Mat& image, Scalar s, int sx, int sy, int fx, int fy);
std::vector<std::tuple<cv::Mat, cv::Point>> getSubImages(const cv::Mat& image, int subImageWidth, int subImageHeight, int step);
cv::Scalar getColor(Color color);
