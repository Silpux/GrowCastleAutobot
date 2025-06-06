#pragma once

#include "Main.h"

#include "Types.h"

using namespace cv;
using namespace std;

constexpr int TOTAL_SCREENS = 24;

extern vector<Mat> originalImages;
extern vector<Mat> resultImages;
extern string currentDirectoryBackSlashes;
extern string currentDirectory;
extern set<Vec3b, Vec3bComparator> uniqueColors;
extern std::string saveImageFormat;
extern PathBezierPoints pbp[8][8];
extern tuple<int, int> pairs[105][4];
extern Point2f squareToDetectCrystal[8];
