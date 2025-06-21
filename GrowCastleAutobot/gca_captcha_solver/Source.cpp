#include "Main.h";
#include "ImageData.h"
#include "ColorUtils.h"
#include "Globals.h"
#include "ImageUtils.h"
#include "Functions.h"
#include "FileUtils.h"

#include <direct.h>

extern "C" __declspec(dllexport) int execute(unsigned char* imagesData, int width, int height, int channels, int count, int trackThingNum, bool saveScreenshots, bool failMode, int* ans, double* ratio0_1, int testVal) {

    if (testVal == 0) {
        Mat m = cv::Mat::zeros(100, 100, CV_8UC3);

        Vec3b color = m.at<Vec3b>(Point(1, 1));
        color[0] = 11;
        color[1] = 12;
        color[2] = 13;
        m.at<Vec3b>(Point(1, 1)) = color;

        *ans = 123;

        return m.at<Vec3b>(Point(1, 1))[2] - m.at<Vec3b>(Point(1, 1))[1] + m.at<Vec3b>(Point(1, 1))[0] - 10;
    }

    originalImages.clear();

    bool gotCWD = false;
    char cwd[_MAX_PATH];
    if (_getcwd(cwd, sizeof(cwd)) != NULL) {
        currentDirectoryBackSlashes = string(cwd);
        currentDirectory = string(cwd);
        std::replace(currentDirectory.begin(), currentDirectory.end(), '\\', '/');
        gotCWD = true;
    }
    else {
        cout << "Couldn't get cwd" << endl;
        return 20;
    }


    int imageSize = width * height * channels;

    for (int i = 0; i < count; ++i)
    {
        const unsigned char* imageData = imagesData + (i * imageSize);
        cv::Mat img(height, width, CV_MAKETYPE(CV_8U, channels), (void*)imageData);
        originalImages.push_back(img.clone());
    }
    cv::Mat boxImage(101, 75, CV_8UC3, imageArray);



    //cv::imshow("RESULT", originalImages[0]);
    //cv::waitKey(0);
        

    return 123;


    int thingToTrack = -1;
    int squareSize = 16;

    for (int i = 0; i < 15; i++) {
        for (int j = 0; j < 8; j++) {
            if (pxlCount(originalImages[i], Scalar(73, 87, 98), squareToDetectCrystal[j].x, squareToDetectCrystal[j].y, squareToDetectCrystal[j].x + squareSize, squareToDetectCrystal[j].y + squareSize) < squareSize * squareSize) {
                thingToTrack = j;
                break;
            }
        }
        if (thingToTrack != -1) {
            break;
        }
    }

    //cout << "Tracking " << thingToTrack << endl;

    //Mat firstCopy = originalImages[0].clone();

    for (int i = 0; i < TOTAL_SCREENS; i++) {
        resultImages.push_back(originalImages[i].clone());
    }

    tuple<int, double> totalError[105];

    vector<Point2f> allPoints[TOTAL_SCREENS];

    for (int j = 7; j < TOTAL_SCREENS; j++) {


        //cout << "Image " << j + 1 << endl;
        vector<Point2f> pointsOnFrame = findRotatedPart(boxImage, originalImages[j], resultImages[j]);


        for (int i = 0; i < pointsOnFrame.size(); i++) {
            //circle(resultImages[j], pointsOnFrame[i], 3, interpolateScalar(getColor(GREEN), getColor(CYAN), (j - 7) / (TOTAL_SCREENS - 1 - 7)), 2);
            allPoints[j].push_back(pointsOnFrame[i]);
            for (int k = 0; k < 105; k++) {
                std::get<0>(totalError[k]) = k;
                double dst = getMinDist(pointsOnFrame[i], k, (double)(j - 7) / (TOTAL_SCREENS - 1 - 7));
                std::get<1>(totalError[k]) += dst * dst;
            }

        }

        //double threshold = 1;

        //TemplateMatching(originalImages[j], part1ToFind, threshold);

        //cv::imshow("Detected Lines", resultImage);
        //cv::waitKey(0);
    }

    sortValues(totalError, 105);

    //for (int i = 0; i < 105; i++) {
    //    cout << "Captcha " << std::get<0>(totalError[i]) << ": " << std::get<1>(totalError[i]) << endl;
    //}

    int captchaInd = std::get<0>(totalError[0]);
    *ans = -1;

    for (int i = 0; i < 4; i++) {
        int p1 = std::get<0>(pairs[captchaInd][i]);
        int p2 = std::get<1>(pairs[captchaInd][i]);

        if (p1 == thingToTrack) {
            *ans = p2;
            break;
        }
        if (p2 == thingToTrack) {
            *ans = p1;
            break;
        }
    }

    Point2f redraw_ps;
    Point2f redraw_pc;
    Point2f redraw_pe;

    for (int i = 0; i < 4; i++) {


        int p1 = std::get<0>(pairs[captchaInd][i]);
        int p2 = std::get<1>(pairs[captchaInd][i]);

        Point2f ps = pbp[p1][p2].PS;
        Point2f pc = pbp[p1][p2].PC;
        Point2f pe = pbp[p1][p2].PE;


        Scalar c = getColor(DEEP_SKY_BLUE);

        if (thingToTrack == p1) {
            c = getColor(GREEN);
            redraw_ps = ps;
            redraw_pc = pc;
            redraw_pe = pe;
        }

        for (int i = 7; i < TOTAL_SCREENS; i++) {
            drawQuadraticBezierWithArrow(resultImages[i], ps, pc, pe, c, 4);

            //double progress = (double)(i - 15) / (47 - 15);
            //double negProgress = 1 - progress;
            //circle(resultImages[i], getQuadraticBezierPoint(ps,pc,pe, 1 - (negProgress * negProgress)), 2, getColor(CYAN), 3);

        }

        ps = pbp[p2][p1].PS;
        pc = pbp[p2][p1].PC;
        pe = pbp[p2][p1].PE;


        c = getColor(DEEP_SKY_BLUE);

        if (thingToTrack == p2) {
            c = getColor(GREEN);
            redraw_ps = ps;
            redraw_pc = pc;
            redraw_pe = pe;
        }

        for (int i = 7; i < TOTAL_SCREENS; i++) {
            drawQuadraticBezierWithArrow(resultImages[i], ps, pc, pe, c, 4);


            //double progress = (double)(i - 15) / (47 - 15);
            //double negProgress = 1 - progress;
            //circle(resultImages[i], getQuadraticBezierPoint(ps, pc, pe, 1 - (negProgress * negProgress)), 2, getColor(CYAN), 3);
        }
    }

    for (int i = 7; i < TOTAL_SCREENS; i++) {
        drawQuadraticBezierWithArrow(resultImages[i], redraw_ps, redraw_pc, redraw_pe, getColor(GREEN), 4);
    }

    for (int i = 7; i < TOTAL_SCREENS; i++) {
        for (int j = 0; j < allPoints[i].size(); j++) {
            circle(resultImages[i], allPoints[i][j], 5, getColor(RED), -1);
        }
    }

    //for (int i = 0; i < 24; i++) {
    //    for (int j = 0; j < allPoints[i].size(); j++) {
    //        circle(resultImages[TOTAL_SCREENS - 1], allPoints[i][j], 5, getColor(RED), -1);
    //    }
    //}

    //std::ofstream outputFile("example.txt");

    //if (outputFile.is_open()) {
        // Write data to the file
    //    for (int i = 0; i < boxPaths.size(); i++) {
    //        outputFile << boxPaths[i].x << "\t" << boxPaths[i].y << endl;
    //    }
        // Close the file
    //    outputFile.close();
    //}
    //else {
    //    std::cerr << "Failed to open the file." << std::endl;
    //}

    //imshow("Paths", firstCopy);
    //waitKey(0);
    int rightCaptchaIndex = std::get<0>(totalError[0]);

    for (int i = 104; i >= 0; i--) {

        //logP("C ", std::get<0>(totalError[i]));
        //logP("C       ", std::get<1>(totalError[i]));

        int captchaIndex = std::get<0>(totalError[i]);




        cout << "Captcha " << captchaIndex << ": "
            << "(" << std::get<0>(pairs[captchaIndex][0]) << " => " << std::get<1>(pairs[captchaIndex][0]) << ") "
            << "(" << std::get<0>(pairs[captchaIndex][1]) << " => " << std::get<1>(pairs[captchaIndex][1]) << ") "
            << "(" << std::get<0>(pairs[captchaIndex][2]) << " => " << std::get<1>(pairs[captchaIndex][2]) << ") "
            << "(" << std::get<0>(pairs[captchaIndex][3]) << " => " << std::get<1>(pairs[captchaIndex][3]) << ") "

            << (int)std::get<1>(totalError[i]) << " total error";


        if (i != 104) {
            cout << " (" << std::get<1>(totalError[i]) / std::get<1>(totalError[i + 1]) * 100 << "% comparing to next)" << endl;
            if (i == 0) {
                cout << endl;
            }
        }
        else {
            cout << endl;
        }

    }

    cout << "Most likely captcha: " << rightCaptchaIndex << " "

        << "(" << getPositionStr(std::get<0>(pairs[rightCaptchaIndex][0])) << " => " << getPositionStr(std::get<1>(pairs[rightCaptchaIndex][0])) << ") "
        << "(" << getPositionStr(std::get<0>(pairs[rightCaptchaIndex][1])) << " => " << getPositionStr(std::get<1>(pairs[rightCaptchaIndex][1])) << ") "
        << "(" << getPositionStr(std::get<0>(pairs[rightCaptchaIndex][2])) << " => " << getPositionStr(std::get<1>(pairs[rightCaptchaIndex][2])) << ") "
        << "(" << getPositionStr(std::get<0>(pairs[rightCaptchaIndex][3])) << " => " << getPositionStr(std::get<1>(pairs[rightCaptchaIndex][3])) << ") " << endl << endl;


    cout << "Tracking: " << getPositionStr(thingToTrack) << endl;
    cout << "Result: " << getPositionStr(*ans) << endl << endl;


    *ratio0_1 = std::get<1>(totalError[0]) / std::get<1>(totalError[1]);

    if (failMode == 1) {
        saveImagesToFolder("Failed captchas");
    }
    else if (saveScreenshots == 1) {
        saveImagesToFolder("Captchas");
    }
    else {
        cout << "No saving screenshoots" << endl << endl;
    }
    //logP("saved images  ", 0);

    //cv::imshow("Detected Lines", result);
    //cv::waitKey(0);

    //logP("last steps  ", 0);


    return 0;
}