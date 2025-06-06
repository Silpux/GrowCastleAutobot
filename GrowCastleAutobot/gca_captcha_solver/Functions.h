#pragma once

#include "Main.h"

std::string getPositionStr(int p);
double getMinDist(cv::Point2f& p, int captchaNum, double progress);
void sortValues(std::tuple<int, double> arr[], int size);
void clusterPoints(std::vector<cv::Point2f>& points, double threshold);
void TemplateMatching(const cv::Mat& img, const cv::Mat& templ, double threshold);
bool isApproxRectangle(cv::Point2f A, cv::Point2f B, cv::Point2f C, cv::Point2f D, double expectedWidth, double expectedHeight, double tolerance);
void removeDuplicatePoints(std::vector<cv::Point2f>& obj, std::vector<cv::Point2f>& scene);
