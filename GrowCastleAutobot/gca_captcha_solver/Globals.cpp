#pragma once

#include "Globals.h"


vector<Mat> originalImages;

vector<Mat> resultImages;

string currentDirectoryBackSlashes;
string currentDirectory;


std::string saveImageFormat = "png";


PathBezierPoints pbp[8][8] = { {PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 0 => 0
                                PathBezierPoints(Point2f(236,103), Point2f(281,30), Point2f(331,121)), // 0 => 1
                                PathBezierPoints(Point2f(236,103), Point2f(308,133), Point2f(363,226)), // 0 => 2
                                PathBezierPoints(Point2f(236,103), Point2f(283,215), Point2f(331,328)), // 0 => 3
                                PathBezierPoints(Point2f(236,103), Point2f(236,232), Point2f(236,361)), // 0 => 4
                                PathBezierPoints(Point2f(236,103), Point2f(194,203), Point2f(140,327)), // 0 => 5
                                PathBezierPoints(Point2f(236,103), Point2f(172,124), Point2f(107,226)), // 0 => 6
                                PathBezierPoints(Point2f(236,103), Point2f(184,32), Point2f(139,123))}, // 0 => 7

                               {PathBezierPoints(Point2f(339,139), Point2f(292,0), Point2f(243,87)), // 1 => 0
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 1 => 1
                                PathBezierPoints(Point2f(339,139), Point2f(359,140), Point2f(370,225)), // 1 => 2
                                PathBezierPoints(Point2f(339,139), Point2f(339,234), Point2f(339,329)), // 1 => 3
                                PathBezierPoints(Point2f(339,139), Point2f(292,253), Point2f(244,362)), // 1 => 4
                                PathBezierPoints(Point2f(339,139), Point2f(234,234), Point2f(150,326)), // 1 => 5
                                PathBezierPoints(Point2f(339,139), Point2f(219,134), Point2f(118,223)), // 1 => 6
                                PathBezierPoints(Point2f(339,139), Point2f(245,29), Point2f(148,123))}, // 1 => 7

                               {PathBezierPoints(Point2f(373, 244), Point2f(308,5), Point2f(246,89)), // 2 => 0
                                PathBezierPoints(Point2f(373,244), Point2f(359,41), Point2f(343,122)), // 2 => 1
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 2 => 2
                                PathBezierPoints(Point2f(373,244), Point2f(360,228), Point2f(341,333)), // 2 => 3
                                PathBezierPoints(Point2f(373,244), Point2f(316,257), Point2f(246,365)), // 2 => 4
                                PathBezierPoints(Point2f(373,244), Point2f(260,231), Point2f(153,329)), // 2 => 5
                                PathBezierPoints(Point2f(373,244), Point2f(250,134), Point2f(118,226)), // 2 => 6
                                PathBezierPoints(Point2f(373,244), Point2f(266,38), Point2f(150,124))}, // 2 => 7

                               {PathBezierPoints(Point2f(340,347), Point2f(291,16), Point2f(244,88)), // 3 => 0
                                PathBezierPoints(Point2f(340,347), Point2f(340,48), Point2f(340,124)), // 3 => 1
                                PathBezierPoints(Point2f(340,347), Point2f(356,140), Point2f(372,228)), // 3 => 2
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 3 => 3
                                PathBezierPoints(Point2f(340,347), Point2f(288,271), Point2f(245,363)), // 3 => 4
                                PathBezierPoints(Point2f(340,347), Point2f(242,241), Point2f(151,326)), // 3 => 5
                                PathBezierPoints(Point2f(340,347), Point2f(228,144), Point2f(115,227)), // 3 => 6
                                PathBezierPoints(Point2f(340,347), Point2f(248,47), Point2f(149,123))}, // 3 => 7

                               {PathBezierPoints(Point2f(236,380), Point2f(236,21), Point2f(236,85)), // 4 => 0
                                PathBezierPoints(Point2f(236, 380), Point2f(279, 55), Point2f(332,123)), // 4 => 1
                                PathBezierPoints(Point2f(236,380), Point2f(301,141), Point2f(366,230)), // 4 => 2
                                PathBezierPoints(Point2f(236,380), Point2f(282,245), Point2f(330,327)), // 4 => 3
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 4 => 4
                                PathBezierPoints(Point2f(236,380), Point2f(186,243), Point2f(140,329)), // 4 => 5
                                PathBezierPoints(Point2f(236,380), Point2f(170,146), Point2f(108,227)), // 4 => 6
                                PathBezierPoints(Point2f(236,380), Point2f(184,52), Point2f(141,122))}, // 4 => 7

                               {PathBezierPoints(Point2f(131,346), Point2f(177,17), Point2f(227,88)), // 5 => 0
                                PathBezierPoints(Point2f(131,346), Point2f(233,47), Point2f(319,122)), // 5 => 1
                                PathBezierPoints(Point2f(131,346), Point2f(240,147), Point2f(353,223)), // 5 => 2
                                PathBezierPoints(Point2f(131,346), Point2f(222,241), Point2f(319,327)), // 5 => 3
                                PathBezierPoints(Point2f(131,346), Point2f(180,270), Point2f(227,365)), // 5 => 4
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 5 => 5
                                PathBezierPoints(Point2f(131,346), Point2f(116,146), Point2f(100,225)), // 5 => 6
                                PathBezierPoints(Point2f(131,346), Point2f(131,44), Point2f(131,124))}, // 5 => 7

                               {PathBezierPoints(Point2f(97,242), Point2f(162,3), Point2f(227,92)), // 6 => 0
                                PathBezierPoints(Point2f(97,242), Point2f(210,40), Point2f(320,122)), // 6 => 1
                                PathBezierPoints(Point2f(97,242), Point2f(228,134), Point2f(352,227)), // 6 => 2
                                PathBezierPoints(Point2f(97,242), Point2f(224,236), Point2f(322,331)), // 6 => 3
                                PathBezierPoints(Point2f(97,242), Point2f(175,274), Point2f(224,363)), // 6 => 4
                                PathBezierPoints(Point2f(97,242), Point2f(112,231), Point2f(128,328)), // 6 => 5
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1)), // 6 => 6
                                PathBezierPoints(Point2f(97,242), Point2f(114,41), Point2f(128,122))}, // 6 => 7

                               {PathBezierPoints(Point2f(131,139), Point2f(181,-5), Point2f(229,92)), // 7 => 0
                                PathBezierPoints(Point2f(131,139), Point2f(229,30), Point2f(322,121)), // 7 => 1
                                PathBezierPoints(Point2f(131,139), Point2f(253,126), Point2f(355,228)), // 7 => 2
                                PathBezierPoints(Point2f(131,139), Point2f(234,228), Point2f(321,328)), // 7 => 3
                                PathBezierPoints(Point2f(131,139), Point2f(173,238), Point2f(227,362)), // 7 => 4
                                PathBezierPoints(Point2f(131,139), Point2f(131,233), Point2f(131,328)), // 7 => 5
                                PathBezierPoints(Point2f(131,139), Point2f(116,127), Point2f(99,226)), // 7 => 6
                                PathBezierPoints(Point2f(-1,-1), Point2f(-1,-1), Point2f(-1,-1))}, }; // 7 => 7











tuple<int, int> pairs[105][4]{ {{0, 1}, {2, 3}, {4, 5}, {6, 7}, },
                                {{0, 1}, {2, 3}, {4, 6}, {5, 7}, },
                                {{0, 1}, {2, 3}, {4, 7}, {5, 6}, },
                                {{0, 1}, {2, 4}, {3, 5}, {6, 7}, },
                                {{0, 1}, {2, 4}, {3, 6}, {5, 7}, },
                                {{0, 1}, {2, 4}, {3, 7}, {5, 6}, },
                                {{0, 1}, {2, 5}, {3, 4}, {6, 7}, },
                                {{0, 1}, {2, 5}, {3, 6}, {4, 7}, },
                                {{0, 1}, {2, 5}, {3, 7}, {4, 6}, },
                                {{0, 1}, {2, 6}, {3, 4}, {5, 7}, },
                                {{0, 1}, {2, 6}, {3, 5}, {4, 7}, },
                                {{0, 1}, {2, 6}, {3, 7}, {4, 5}, },
                                {{0, 1}, {2, 7}, {3, 4}, {5, 6}, },
                                {{0, 1}, {2, 7}, {3, 5}, {4, 6}, },
                                {{0, 1}, {2, 7}, {3, 6}, {4, 5}, },
                                {{0, 2}, {1, 3}, {4, 5}, {6, 7}, },
                                {{0, 2}, {1, 3}, {4, 6}, {5, 7}, },
                                {{0, 2}, {1, 3}, {4, 7}, {5, 6}, },
                                {{0, 2}, {1, 4}, {3, 5}, {6, 7}, },
                                {{0, 2}, {1, 4}, {3, 6}, {5, 7}, },
                                {{0, 2}, {1, 4}, {3, 7}, {5, 6}, },
                                {{0, 2}, {1, 5}, {3, 4}, {6, 7}, },
                                {{0, 2}, {1, 5}, {3, 6}, {4, 7}, },
                                {{0, 2}, {1, 5}, {3, 7}, {4, 6}, },
                                {{0, 2}, {1, 6}, {3, 4}, {5, 7}, },
                                {{0, 2}, {1, 6}, {3, 5}, {4, 7}, },
                                {{0, 2}, {1, 6}, {3, 7}, {4, 5}, },
                                {{0, 2}, {1, 7}, {3, 4}, {5, 6}, },
                                {{0, 2}, {1, 7}, {3, 5}, {4, 6}, },
                                {{0, 2}, {1, 7}, {3, 6}, {4, 5}, },
                                {{0, 3}, {1, 2}, {4, 5}, {6, 7}, },
                                {{0, 3}, {1, 2}, {4, 6}, {5, 7}, },
                                {{0, 3}, {1, 2}, {4, 7}, {5, 6}, },
                                {{0, 3}, {1, 4}, {2, 5}, {6, 7}, },
                                {{0, 3}, {1, 4}, {2, 6}, {5, 7}, },
                                {{0, 3}, {1, 4}, {2, 7}, {5, 6}, },
                                {{0, 3}, {1, 5}, {2, 4}, {6, 7}, },
                                {{0, 3}, {1, 5}, {2, 6}, {4, 7}, },
                                {{0, 3}, {1, 5}, {2, 7}, {4, 6}, },
                                {{0, 3}, {1, 6}, {2, 4}, {5, 7}, },
                                {{0, 3}, {1, 6}, {2, 5}, {4, 7}, },
                                {{0, 3}, {1, 6}, {2, 7}, {4, 5}, },
                                {{0, 3}, {1, 7}, {2, 4}, {5, 6}, },
                                {{0, 3}, {1, 7}, {2, 5}, {4, 6}, },
                                {{0, 3}, {1, 7}, {2, 6}, {4, 5}, },
                                {{0, 4}, {1, 2}, {3, 5}, {6, 7}, },
                                {{0, 4}, {1, 2}, {3, 6}, {5, 7}, },
                                {{0, 4}, {1, 2}, {3, 7}, {5, 6}, },
                                {{0, 4}, {1, 3}, {2, 5}, {6, 7}, },
                                {{0, 4}, {1, 3}, {2, 6}, {5, 7}, },
                                {{0, 4}, {1, 3}, {2, 7}, {5, 6}, },
                                {{0, 4}, {1, 5}, {2, 3}, {6, 7}, },
                                {{0, 4}, {1, 5}, {2, 6}, {3, 7}, },
                                {{0, 4}, {1, 5}, {2, 7}, {3, 6}, },
                                {{0, 4}, {1, 6}, {2, 3}, {5, 7}, },
                                {{0, 4}, {1, 6}, {2, 5}, {3, 7}, },
                                {{0, 4}, {1, 6}, {2, 7}, {3, 5}, },
                                {{0, 4}, {1, 7}, {2, 3}, {5, 6}, },
                                {{0, 4}, {1, 7}, {2, 5}, {3, 6}, },
                                {{0, 4}, {1, 7}, {2, 6}, {3, 5}, },
                                {{0, 5}, {1, 2}, {3, 4}, {6, 7}, },
                                {{0, 5}, {1, 2}, {3, 6}, {4, 7}, },
                                {{0, 5}, {1, 2}, {3, 7}, {4, 6}, },
                                {{0, 5}, {1, 3}, {2, 4}, {6, 7}, },
                                {{0, 5}, {1, 3}, {2, 6}, {4, 7}, },
                                {{0, 5}, {1, 3}, {2, 7}, {4, 6}, },
                                {{0, 5}, {1, 4}, {2, 3}, {6, 7}, },
                                {{0, 5}, {1, 4}, {2, 6}, {3, 7}, },
                                {{0, 5}, {1, 4}, {2, 7}, {3, 6}, },
                                {{0, 5}, {1, 6}, {2, 3}, {4, 7}, },
                                {{0, 5}, {1, 6}, {2, 4}, {3, 7}, },
                                {{0, 5}, {1, 6}, {2, 7}, {3, 4}, },
                                {{0, 5}, {1, 7}, {2, 3}, {4, 6}, },
                                {{0, 5}, {1, 7}, {2, 4}, {3, 6}, },
                                {{0, 5}, {1, 7}, {2, 6}, {3, 4}, },
                                {{0, 6}, {1, 2}, {3, 4}, {5, 7}, },
                                {{0, 6}, {1, 2}, {3, 5}, {4, 7}, },
                                {{0, 6}, {1, 2}, {3, 7}, {4, 5}, },
                                {{0, 6}, {1, 3}, {2, 4}, {5, 7}, },
                                {{0, 6}, {1, 3}, {2, 5}, {4, 7}, },
                                {{0, 6}, {1, 3}, {2, 7}, {4, 5}, },
                                {{0, 6}, {1, 4}, {2, 3}, {5, 7}, },
                                {{0, 6}, {1, 4}, {2, 5}, {3, 7}, },
                                {{0, 6}, {1, 4}, {2, 7}, {3, 5}, },
                                {{0, 6}, {1, 5}, {2, 3}, {4, 7}, },
                                {{0, 6}, {1, 5}, {2, 4}, {3, 7}, },
                                {{0, 6}, {1, 5}, {2, 7}, {3, 4}, },
                                {{0, 6}, {1, 7}, {2, 3}, {4, 5}, },
                                {{0, 6}, {1, 7}, {2, 4}, {3, 5}, },
                                {{0, 6}, {1, 7}, {2, 5}, {3, 4}, },
                                {{0, 7}, {1, 2}, {3, 4}, {5, 6}, },
                                {{0, 7}, {1, 2}, {3, 5}, {4, 6}, },
                                {{0, 7}, {1, 2}, {3, 6}, {4, 5}, },
                                {{0, 7}, {1, 3}, {2, 4}, {5, 6}, },
                                {{0, 7}, {1, 3}, {2, 5}, {4, 6}, },
                                {{0, 7}, {1, 3}, {2, 6}, {4, 5}, },
                                {{0, 7}, {1, 4}, {2, 3}, {5, 6}, },
                                {{0, 7}, {1, 4}, {2, 5}, {3, 6}, },
                                {{0, 7}, {1, 4}, {2, 6}, {3, 5}, },
                                {{0, 7}, {1, 5}, {2, 3}, {4, 6}, },
                                {{0, 7}, {1, 5}, {2, 4}, {3, 6}, },
                                {{0, 7}, {1, 5}, {2, 6}, {3, 4}, },
                                {{0, 7}, {1, 6}, {2, 3}, {4, 5}, },
                                {{0, 7}, {1, 6}, {2, 4}, {3, 5}, },
                                {{0, 7}, {1, 6}, {2, 5}, {3, 4}, }, };

Point2f squareToDetectCrystal[8] = { Point2f(229, 161),
                                    Point2f(290,187),
                                    Point2f(311,230),
                                    Point2f(293, 281),
                                    Point2f(231,309),
                                    Point2f(173,285),
                                    Point2f(144,234),
                                    Point2f(177,184) };

