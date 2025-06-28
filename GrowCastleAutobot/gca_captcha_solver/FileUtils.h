#pragma once

#include "Main.h"

int saveImagesToFolder(std::string savePath);
bool directoryExists(const std::string& dirName);
bool fileExists(const std::string& filename);
std::string getCurrentDateTime();
