#include "ImageUtils.h"
#include "Functions.h"
#include "Calculations.h"
#include "ColorUtils.h"
#include <random>

vector<Point2f> findRotatedPart(const Mat& objectPart, const Mat& image, Mat& imageToDraw) {

    //logP("findRotatedPart", 0);
    //cout << "New image" << endl;

    Mat imageCopy = image.clone();

    Mat grayObjectPart, grayImage;

    cvtColor(objectPart, grayObjectPart, COLOR_BGR2GRAY);
    cvtColor(image, grayImage, COLOR_BGR2GRAY);

    Ptr<SIFT> sift = SIFT::create();
    std::vector<KeyPoint> keypointsObject;
    Mat descriptorsObject;
    sift->detectAndCompute(grayObjectPart, noArray(), keypointsObject, descriptorsObject);

    int h = 105;
    int w = 105;
    int step = 50;

    vector<Point2f> foundRectangles;

    //logP("findRotatedPart", 1);
    for (int y = 0; y <= image.rows - h; y += step) {
        //logP("Y: ", y);
        for (int x = 0; x <= image.cols - w; x += step) {


            //logP("X: ", x);
            cv::Rect roi(x, y, w, h);

            Mat subImage = image(roi);

            if (pxlCount(subImage, Scalar(73, 87, 98), 0, 0, w - 1, h - 1) > w * h / 3) {
                continue;
            }


            Mat graySubImage;
            cvtColor(subImage, graySubImage, COLOR_BGR2GRAY);

            std::vector<KeyPoint> keypointsROI;
            Mat descriptorsROI;
            sift->detectAndCompute(graySubImage, noArray(), keypointsROI, descriptorsROI);

            if (!descriptorsObject.empty() && !descriptorsROI.empty() &&
                descriptorsObject.type() == descriptorsROI.type() &&
                descriptorsObject.cols == descriptorsROI.cols) {

                BFMatcher matcher(NORM_L2, true);
                vector<DMatch> matches;
                matcher.match(descriptorsObject, descriptorsROI, matches);

                vector<DMatch> goodMatches = matches;

                std::vector<Point2f> obj;
                std::vector<Point2f> scene;

                for (size_t i = 0; i < goodMatches.size(); i++) {

                    //logP("OBJ    X:        ", keypointsObject[goodMatches[i].queryIdx].pt.x);
                    //logP("OBJ    Y:        ", keypointsObject[goodMatches[i].queryIdx].pt.y);
                    obj.push_back(keypointsObject[goodMatches[i].queryIdx].pt);

                    //logP("SCENE X:             ", (keypointsROI[goodMatches[i].trainIdx].pt + Point2f(x, y)).x);
                    //logP("SCENE Y:             ", (keypointsROI[goodMatches[i].trainIdx].pt + Point2f(x, y)).y);
                    scene.push_back(keypointsROI[goodMatches[i].trainIdx].pt + Point2f(x, y)); // Adjust coordinates


                    //obj.push_back(Point2f((int)keypointsObject[goodMatches[i].queryIdx].pt.x, (int)keypointsObject[goodMatches[i].queryIdx].pt.y));
                    //scene.push_back(Point2f((int)(keypointsROI[goodMatches[i].trainIdx].pt + Point2f(x, y)).x, (int)(keypointsROI[goodMatches[i].trainIdx].pt + Point2f(x, y)).y)); // Adjust coordinates
                }
                removeDuplicatePoints(obj, scene);


                //logP("OBJ: ", x);
                //for (int i = 0; i < obj.size(); i++) {
                //    logP(to_string(obj[i].x) + "\t" + to_string(obj[i].y), 0);
                //}
                //logP("SCENE: ", x);
                //for (int i = 0; i < scene.size(); i++) {
                //    logP(to_string(scene[i].x) +"\t" + to_string(scene[i].y), 0);
                //}

                //logP("111        ", x);

                if (obj.size() >= 4 && scene.size() >= 4) {
                    //logP("Try find homography        ", x);
                    //logP("obj size:        ", obj.size());
                    //logP("scene size:        ", scene.size());

                    Mat H = findHomography(obj, scene, RANSAC);

                    //logP("Homography found        ", x);

                    // Proceed with the perspective transform and drawing
                    std::vector<Point2f> objCorners(4);
                    objCorners[0] = Point2f(0, 0);
                    objCorners[1] = Point2f(objectPart.cols, 0);
                    objCorners[2] = Point2f(objectPart.cols, objectPart.rows);
                    objCorners[3] = Point2f(0, objectPart.rows);

                    if (!H.empty()) {

                        //logP("perspective transform        ", x);
                        std::vector<Point2f> sceneCorners(4);
                        perspectiveTransform(objCorners, sceneCorners, H);
                        //logP("transformed        ", x);

                        // Draw matches and detected location
                        //drawMatches(objectPart, keypointsObject, image, keypointsImage, goodMatches, img_matches);

                        if (isApproxRectangle(sceneCorners[0], sceneCorners[1], sceneCorners[2], sceneCorners[3], 75, 101, 0.1)) {

                            //rectangle(imageCopy, roi, getColor(RED));
                            line(imageToDraw, sceneCorners[0], sceneCorners[1], Scalar(0, 255, 0), 1);
                            line(imageToDraw, sceneCorners[1], sceneCorners[2], Scalar(0, 255, 0), 1);
                            line(imageToDraw, sceneCorners[2], sceneCorners[3], Scalar(0, 255, 0), 1);
                            line(imageToDraw, sceneCorners[3], sceneCorners[0], Scalar(0, 255, 0), 1);

                            int centerX = (sceneCorners[0].x + sceneCorners[1].x + sceneCorners[2].x + sceneCorners[3].x) / 4;
                            int centerY = (sceneCorners[0].y + sceneCorners[1].y + sceneCorners[2].y + sceneCorners[3].y) / 4;

                            foundRectangles.push_back(Point2f(centerX, centerY));
                        }

                        //rectangle(img_matches, roi, getColor(RED), 2);

                    }
                }
                //logP("222        ", x);
            }
        }
    }

    clusterPoints(foundRectangles, 10);

    vector<Point2f> points;

    //cout << "Found "<< foundRectangles.size() << " points" << endl;
    for (int i = 0; i < foundRectangles.size(); i++) {
        circle(imageCopy, foundRectangles[i], 2, getColor(CYAN), 2);
        //boxPaths.push_back(foundRectangles[i]);
        points.push_back(foundRectangles[i]);
    }


    //cout << "Done" << endl;
    //imshow("Detected Part", imageToDraw);
    //waitKey(0);

    return points;

}


void SetConsoleColor(int color) {
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
    SetConsoleTextAttribute(hConsole, color);
}


void drawQuadraticBezier(Mat& image, Point2f start, Point2f control, Point2f end, Scalar color, int thickness) {
    Point2f prevPoint = start;

    for (float t = 0; t <= 1.0; t += 0.01) {
        Point2f currentPoint = getQuadraticBezierPoint(start, control, end, t);
        line(image, prevPoint, currentPoint, color, thickness);
        prevPoint = currentPoint;
    }
}


void drawQuadraticBezierWithArrow(Mat& image, Point2f start, Point2f control, Point2f end, Scalar color, int thickness) {
    Point2f prevPoint = start;
    Point2f currentPoint;

    for (float t = 0; t <= 1.0; t += 0.01) {
        currentPoint = getQuadraticBezierPoint(start, control, end, t);
        line(image, prevPoint, currentPoint, color, thickness);
        prevPoint = currentPoint;
    }

    // Calculate the direction at the end of the curve
    Point2f tangent = quadraticBezierDerivative(start, control, end, 1.0);
    float arrowLength = 20.0;
    float arrowAngle = CV_PI / 5;  // 30 deg

    Point2f arrowPoint1 = currentPoint - arrowLength * Point2f(cos(atan2(tangent.y, tangent.x) + arrowAngle), sin(atan2(tangent.y, tangent.x) + arrowAngle));
    Point2f arrowPoint2 = currentPoint - arrowLength * Point2f(cos(atan2(tangent.y, tangent.x) - arrowAngle), sin(atan2(tangent.y, tangent.x) - arrowAngle));

    line(image, currentPoint, arrowPoint1, color, thickness);
    line(image, currentPoint, arrowPoint2, color, thickness);
}



void drawLineThroughPoint(cv::Mat& image, cv::Point point, double angle) {
    double angleRad = angle * CV_PI / 180.0;

    double dx = std::cos(angleRad);
    double dy = std::sin(angleRad);

    int width = image.cols;
    int height = image.rows;

    cv::Point pt1, pt2;

    if (dy != 0) {
        pt1.x = point.x + (0 - point.y) / dy * dx;
        pt1.y = 0;

        pt2.x = point.x + (height - point.y) / dy * dx;
        pt2.y = height;
    }
    else {
        pt1.x = 0;
        pt1.y = point.y;
        pt2.x = width;
        pt2.y = point.y;
    }

    if (pt1.x < 0) {
        pt1.x = 0;
        pt1.y = point.y + (0 - point.x) / dx * dy;
    }
    if (pt1.x > width) {
        pt1.x = width;
        pt1.y = point.y + (width - point.x) / dx * dy;
    }
    if (pt2.x < 0) {
        pt2.x = 0;
        pt2.y = point.y + (0 - point.x) / dx * dy;
    }
    if (pt2.x > width) {
        pt2.x = width;
        pt2.y = point.y + (width - point.x) / dx * dy;
    }

    cv::line(image, pt1, pt2, getColor(WHITE), 1);
}






cv::Mat calculateHomography(const std::vector<cv::Point2f>& srcPoints, const std::vector<cv::Point2f>& dstPoints) {
    cv::Mat A(8, 9, CV_64F);

    for (int i = 0; i < 4; i++) {
        double x1 = srcPoints[i].x;
        double y1 = srcPoints[i].y;
        double x2 = dstPoints[i].x;
        double y2 = dstPoints[i].y;

        A.at<double>(2 * i, 0) = x1;
        A.at<double>(2 * i, 1) = y1;
        A.at<double>(2 * i, 2) = 1.0;
        A.at<double>(2 * i, 3) = 0.0;
        A.at<double>(2 * i, 4) = 0.0;
        A.at<double>(2 * i, 5) = 0.0;
        A.at<double>(2 * i, 6) = -x2 * x1;
        A.at<double>(2 * i, 7) = -x2 * y1;
        A.at<double>(2 * i, 8) = -x2;

        A.at<double>(2 * i + 1, 0) = 0.0;
        A.at<double>(2 * i + 1, 1) = 0.0;
        A.at<double>(2 * i + 1, 2) = 0.0;
        A.at<double>(2 * i + 1, 3) = x1;
        A.at<double>(2 * i + 1, 4) = y1;
        A.at<double>(2 * i + 1, 5) = 1.0;
        A.at<double>(2 * i + 1, 6) = -y2 * x1;
        A.at<double>(2 * i + 1, 7) = -y2 * y1;
        A.at<double>(2 * i + 1, 8) = -y2;
    }


    cv::Mat h;
    cv::SVD::solveZ(A, h);


    cv::Mat H = h.reshape(1, 3);
    return H;
}


cv::Mat customFindHomography(const std::vector<cv::Point2f>& srcPoints, const std::vector<cv::Point2f>& dstPoints, int maxIterations, double reprojectionThreshold) {
    const int numPoints = srcPoints.size();
    std::vector<int> bestInliers;
    cv::Mat bestHomography;

    std::random_device rd;
    std::mt19937 rng(rd());
    std::uniform_int_distribution<int> dist(0, numPoints - 1);

    for (int iteration = 0; iteration < maxIterations; iteration++) {
        try {
            std::vector<cv::Point2f> selectedSrcPoints(4), selectedDstPoints(4);
            for (int i = 0; i < 4; i++) {
                int idx = dist(rng);
                selectedSrcPoints[i] = srcPoints[idx];
                selectedDstPoints[i] = dstPoints[idx];
            }

            cv::Mat H = findHomography(selectedSrcPoints, selectedDstPoints, RANSAC, 3.0, noArray(), 50);
            if (H.empty() || H.rows != 3 || H.cols != 3) {
                continue;
            }

            std::vector<int> inliers;
            for (int i = 0; i < numPoints; i++) {
                cv::Point2f p1 = srcPoints[i];
                cv::Point2f p2 = dstPoints[i];

                cv::Mat pt1 = (cv::Mat_<double>(3, 1) << p1.x, p1.y, 1.0);
                cv::Mat pt2 = H * pt1;
                if (pt2.at<double>(2) == 0) {
                    continue;
                }
                pt2 /= pt2.at<double>(2);

                double error = cv::norm(p2 - cv::Point2f(pt2.at<double>(0), pt2.at<double>(1)));

                if (error < reprojectionThreshold) {
                    inliers.push_back(i);
                }
            }

            if (inliers.size() > bestInliers.size()) {
                bestInliers = inliers;
                bestHomography = H.clone();
            }
        }
        catch (const cv::Exception& e) {
            std::cerr << "Exception during RANSAC iteration: " << e.what() << std::endl;
            continue;
        }
    }

    if (!bestInliers.empty()) {
        std::vector<cv::Point2f> finalSrcPoints, finalDstPoints;
        for (int idx : bestInliers) {
            finalSrcPoints.push_back(srcPoints[idx]);
            finalDstPoints.push_back(dstPoints[idx]);
        }
        bestHomography = calculateHomography(finalSrcPoints, finalDstPoints);
    }

    return bestHomography;
}