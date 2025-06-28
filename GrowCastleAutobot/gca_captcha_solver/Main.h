#ifndef HEADER_H
#define HEADER_H

#include <opencv2/opencv.hpp>
#include <windows.h>

using namespace cv;
using namespace std;
using namespace chrono;

extern "C" __declspec(dllexport) int execute(unsigned char* imagesData, int width, int height, int channels, int count, bool saveScreenshots, bool failMode, int* trackedNumber, int* ans, double* ratio0_1, int testVal);

#endif
