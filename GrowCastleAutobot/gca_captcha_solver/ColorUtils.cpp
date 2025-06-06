
#include "ColorUtils.h";


void COLORMODE(int mode, Mat& src) {

    unsigned char colorShift = 1 << mode;
    unsigned char mask = 255 << mode;

    for (int y = 0; y < src.rows; y++) {
        for (int x = 0; x < src.cols; x++) {
            Vec3b color = src.at<Vec3b>(Point(x, y));
            color[0] = ((color[0] + colorShift) & mask) - 1;
            color[1] = ((color[1] + colorShift) & mask) - 1;
            color[2] = ((color[2] + colorShift) & mask) - 1;
            src.at<Vec3b>(Point(x, y)) = color;
        }
    }
}



set<Vec3b, Vec3bComparator> getDistinctColors(const Mat& image) {
    set<Vec3b, Vec3bComparator> uniqueColors;
    for (int y = 0; y < image.rows; y++) {
        for (int x = 0; x < image.cols; x++) {
            uniqueColors.insert(image.at<Vec3b>(y, x));
        }
    }
    return uniqueColors;
}


void replaceColorsWithBlack(Mat& image, const set<Vec3b, Vec3bComparator>& colorsToReplace) {
    for (int y = 0; y < image.rows; ++y) {
        for (int x = 0; x < image.cols; ++x) {
            Vec3b color = image.at<Vec3b>(Point(x, y));
            if (colorsToReplace.find(color) != colorsToReplace.end()) {
                color = Vec3b(0, 0, 0);
                image.at<Vec3b>(Point(x, y)) = color;
            }
        }
    }
}

int pxlCount(const Mat& image, const set<Vec3b, Vec3bComparator>& distinctColors, int sx, int sy, int fx, int fy) {

    int pxlcount = 0;

    for (int y = sy; y <= fy; ++y) {
        for (int x = sx; x < fx; ++x) {
            Vec3b color = image.at<Vec3b>(y, x);
            if (distinctColors.find(color) != distinctColors.end()) {
                ++pxlcount;
            }
        }
    }

    return pxlcount;

}


int pxlCount(const Mat& image, Scalar s, int sx, int sy, int fx, int fy) {

    int pxlcount = 0;

    for (int y = sy; y <= fy; ++y) {
        for (int x = sx; x < fx; ++x) {
            Vec3b color = image.at<Vec3b>(y, x);
            if (s[0] == color[0] && s[1] == color[1] && s[2] == color[2]) {
                ++pxlcount;
            }
        }
    }

    return pxlcount;

}




std::vector<std::tuple<cv::Mat, cv::Point>> getSubImages(const cv::Mat& image, int subImageWidth, int subImageHeight, int step) {
    std::vector<std::tuple<cv::Mat, cv::Point>> subImages;

    for (int y = 0; y <= image.rows - subImageHeight; y += step) {
        for (int x = 0; x <= image.cols - subImageWidth; x += step) {
            cv::Rect roi(x, y, subImageWidth, subImageHeight);
            cv::Mat subImage = image(roi);
            cv::Point origin(x, y);
            subImages.push_back(std::make_tuple(subImage, origin));
        }
    }

    return subImages;
}



cv::Scalar getColor(Color color) {

    switch (color) {
    case RED:
        return cv::Scalar(0, 0, 255);
    case ORANGE:
        return cv::Scalar(0, 165, 255);
    case YELLOW:
        return cv::Scalar(0, 255, 255);
    case GREEN:
        return cv::Scalar(0, 255, 0);
    case CYAN:
        return cv::Scalar(255, 255, 0);
    case BLUE:
        return cv::Scalar(255, 105, 105);
    case MAGENTA:
        return cv::Scalar(255, 0, 255);
    case HOTPINK:
        return cv::Scalar(180, 105, 255);
    case DEEPPINK:
        return cv::Scalar(147, 20, 255);
    case MEDIUM_SLATE_BLUE:
        return cv::Scalar(238, 104, 123);
    case GOLD:
        return cv::Scalar(0, 215, 255);
    case CHOCOLATE:
        return cv::Scalar(30, 105, 210);
    case DEEP_SKY_BLUE:
        return cv::Scalar(255, 191, 0);
    case WHITE:
        return cv::Scalar(255, 255, 255);
    case BLACK:
        return cv::Scalar(0, 0, 0);
    default:
        return cv::Scalar(0, 0, 0);
    }
}


