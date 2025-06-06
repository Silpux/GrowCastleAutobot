#pragma once

#include "Main.h"

using namespace cv;
using namespace std;

struct Vec3bComparator {
    bool operator() (const Vec3b& lhs, const Vec3b& rhs) const {
        if (lhs[0] != rhs[0]) return lhs[0] < rhs[0];
        if (lhs[1] != rhs[1]) return lhs[1] < rhs[1];
        return lhs[2] < rhs[2];
    }
};

struct PointPairComparator {
    bool operator()(const pair<Point2f, Point2f>& a, const pair<Point2f, Point2f>& b) const {
        if (a.first.x != b.first.x) return a.first.x < b.first.x;
        if (a.first.y != b.first.y) return a.first.y < b.first.y;
        if (a.second.x != b.second.x) return a.second.x < b.second.x;
        return a.second.y < b.second.y;
    }
};

struct PathBezierPoints {
    Point2f PS;
    Point2f PC;
    Point2f PE;
    PathBezierPoints(Point2f S, Point2f R, Point2f E) : PS(S), PC(R), PE(E) {}
};

enum Color {
    RED,
    ORANGE,
    YELLOW,
    GREEN,
    CYAN,
    BLUE,
    MAGENTA,
    HOTPINK,
    DEEPPINK,
    MEDIUM_SLATE_BLUE,
    GOLD,
    CHOCOLATE,
    DEEP_SKY_BLUE,
    WHITE,
    BLACK
};
