#include "Functions.h"
#include "ImageUtils.h"
#include "Calculations.h"
#include "Globals.h"
#include "Types.h"

using namespace cv;
using namespace std;

double getMinDist(Point2f& p, int captchaNum, double progress) {

    double spread = 0.25;

    double progressMin = progress - spread < 0 ? 0 : progress - spread;
    double progressMax = progress + spread > 1 ? 1 : progress + spread;

    int splitPatrs = 1000;

    double stepSize = 1.0 / splitPatrs;


    double minDist = 999999;

    for (int i = 0; i < 4; i++) {

        int ps = std::get<0>(pairs[captchaNum][i]);
        int pf = std::get<1>(pairs[captchaNum][i]);

        for (double j = progressMin; j <= progressMax; j += stepSize) {

            double dst = distance(p, getQuadraticBezierPoint(pbp[ps][pf].PS, pbp[ps][pf].PC, pbp[ps][pf].PE, j));
            double dst2 = distance(p, getQuadraticBezierPoint(pbp[pf][ps].PS, pbp[pf][ps].PC, pbp[pf][ps].PE, j));

            if (dst < minDist) {
                minDist = dst;
            }
            if (dst2 < minDist) {
                minDist = dst2;
            }

        }


    }

    return minDist;

}

std::string getPositionStr(int p) {
    switch (p) {
    case(0): return "Up";
    case(1): return "Right up";
    case(2): return "Right";
    case(3): return "Right down";
    case(4): return "Down";
    case(5): return "Left down";
    case(6): return "Left";
    case(7): return "Left up";
    default: return ".";
    }
}


void sortValues(std::tuple<int, double> arr[], int size) {
    std::sort(arr, arr + size, [](const std::tuple<int, double>& a, const std::tuple<int, double>& b) {
        return std::get<1>(a) < std::get<1>(b);
    });
}






void clusterPoints(std::vector<cv::Point2f>& points, double threshold) {
    std::vector<bool> visited(points.size(), false);
    std::vector<cv::Point2f> newPoints;

    for (size_t i = 0; i < points.size(); ++i) {
        if (visited[i]) continue;

        std::vector<cv::Point> cluster;
        cluster.push_back(points[i]);
        visited[i] = true;

        for (size_t j = i + 1; j < points.size(); ++j) {
            if (!visited[j] && distance(points[i], points[j]) < threshold) {
                cluster.push_back(points[j]);
                visited[j] = true;
            }
        }

        cv::Point avgPoint(0, 0);
        for (const auto& pt : cluster) {
            avgPoint += pt;
        }
        avgPoint.x /= cluster.size();
        avgPoint.y /= cluster.size();

        newPoints.push_back(avgPoint);
    }

    points = newPoints;
}







//
//
//void logP(const std::string& message, int value) {
//    std::ostringstream logEntry;
//    logEntry << message << " " << value;  // Concatenate message and variable
//
//    std::ofstream progressFile("progress.txt", std::ios::out | std::ios::app);
//    if (progressFile.is_open()) {
//        progressFile << logEntry.str() << "\n";  // Write formatted string to file
//        progressFile.close();
//    }
//    else {
//        std::cerr << "Unable to open file";
//    }
//}
//



void TemplateMatching(const Mat& img, const Mat& templ, double threshold)
{
    int result_cols = img.cols - templ.cols + 1;
    int result_rows = img.rows - templ.rows + 1;
    Mat result(result_rows, result_cols, CV_32FC1);

    matchTemplate(img, templ, result, TM_CCOEFF_NORMED);

    normalize(result, result, 0, 1, NORM_MINMAX, -1, Mat());

    while (true)
    {
        double minVal, maxVal;
        Point minLoc, maxLoc;
        minMaxLoc(result, &minVal, &maxVal, &minLoc, &maxLoc, Mat());

        if (maxVal < threshold)
            break;

        Point matchLoc = maxLoc;
        rectangle(img, matchLoc, Point(matchLoc.x + templ.cols, matchLoc.y + templ.rows), Scalar::all(255), 2, 8, 0);

        rectangle(result, matchLoc, Point(matchLoc.x + templ.cols, matchLoc.y + templ.rows), Scalar::all(0), FILLED);
    }

    imshow("Detected Matches", img);
    waitKey(0);
}







bool isApproxRectangle(cv::Point2f A, cv::Point2f B, cv::Point2f C, cv::Point2f D, double expectedWidth, double expectedHeight, double tolerance) {
    double AB = distance(A, B);
    double BC = distance(B, C);
    double CD = distance(C, D);
    double DA = distance(D, A);

    double AC = distance(A, C);
    double BD = distance(B, D);

    double lowerBoundWidth = expectedWidth * (1.0 - tolerance);
    double upperBoundWidth = expectedWidth * (1.0 + tolerance);
    double lowerBoundHeight = expectedHeight * (1.0 - tolerance);
    double upperBoundHeight = expectedHeight * (1.0 + tolerance);

    bool config1 =
        (AB > lowerBoundWidth && AB < upperBoundWidth && CD > lowerBoundWidth && CD < upperBoundWidth) &&
        (BC > lowerBoundHeight && BC < upperBoundHeight && DA > lowerBoundHeight && DA < upperBoundHeight);

    bool config2 =
        (AB > lowerBoundHeight && AB < upperBoundHeight && CD > lowerBoundHeight && CD < upperBoundHeight) &&
        (BC > lowerBoundWidth && BC < upperBoundWidth && DA > lowerBoundWidth && DA < upperBoundWidth);

    bool diagonalsCondition = std::abs(AC - BD) < AC * tolerance;

    return (config1 || config2) && diagonalsCondition;
}













void removeDuplicatePoints(vector<Point2f>& obj, vector<Point2f>& scene) {
    if (obj.size() != scene.size()) {
        return;
    }

    set<pair<Point2f, Point2f>, PointPairComparator> uniquePairs;

    vector<Point2f> filteredObj, filteredScene;

    for (size_t i = 0; i < obj.size(); ++i) {
        pair<Point2f, Point2f> currentPair = make_pair(obj[i], scene[i]);

        if (uniquePairs.find(currentPair) == uniquePairs.end()) {
            uniquePairs.insert(currentPair);
            filteredObj.push_back(obj[i]);
            filteredScene.push_back(scene[i]);
        }
    }

    obj = filteredObj;
    scene = filteredScene;
}
