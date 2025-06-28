#include <direct.h>
#include <io.h>

#include "FileUtils.h"
#include "Globals.h"

using namespace cv;
using namespace std;

bool directoryExists(const std::string& dirName) {
    return _access(dirName.c_str(), 0) == 0;
}

bool fileExists(const std::string& filename) {
    struct stat buffer;
    return (stat(filename.c_str(), &buffer) == 0);
}

int saveImagesToFolder(string savePath) {

    int folderIndex = 1;
    std::string folderPath;

    if (!directoryExists(savePath)) {
        if (_mkdir(savePath.c_str()) != 0) {
            return 1;
        }
    }


    do {
        folderPath = savePath + "\\Captcha_" + std::to_string(folderIndex);
        folderIndex++;
    } while (directoryExists(folderPath));

    if (_mkdir(folderPath.c_str()) != 0) {
        return 2;
    }

    for (size_t i = 0; i < originalImages.size(); ++i) {
        std::string filePath = folderPath + "\\CaptchaImage_" + std::to_string(i + 1) + "." + saveImageFormat;
        cv::imwrite(filePath, originalImages[i]);
    }

    for (size_t i = 0; i < resultImages.size(); ++i) {
        std::string filePath = folderPath + "\\ResultImage_" + std::to_string(i) + "." + saveImageFormat;
        cv::imwrite(filePath, resultImages[i]);
    }

    cout << "Saved images to:\n" << folderPath << endl << endl;

    return 0;
}



string getCurrentDateTime() {
    std::time_t now = std::time(nullptr);

    std::tm localTime;

    localtime_s(&localTime, &now);

    std::ostringstream oss;
    oss << std::put_time(&localTime, "%d.%m.%Y %H:%M:%S");

    return oss.str();
}


