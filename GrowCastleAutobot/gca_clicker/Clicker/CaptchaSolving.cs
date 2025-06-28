using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private const int SCREENS_COUNT = 24;
        private const int WHOLE_PATH_TIME = 2000;

        private int frameWait;

        private DateTime lastScreenTime;
        private int currentFrameWait;

        private int captchaAnswer;

        private void CheckDllErrors(int returnValue)
        {
            if (returnValue == -1)
            {
                MessageBox.Show("gca_captcha_solver.dll is missing or cannot be called. Should be in core folder", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                Halt();
            }
            else if(returnValue == 20)
            {
                MessageBox.Show("For some reason couldn't get current directory path. Try removing spaces and cyrillic symbols from path to core folder", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                Halt();
            }

        }

        private void InitCaptchaParams()
        {
            frameWait = (int)((float)WHOLE_PATH_TIME / (SCREENS_COUNT - 1));
            lastScreenTime = DateTime.MinValue;
            currentFrameWait = 0;
            captchaAnswer = -1;
        }

        private void SolveCaptcha()
        {

            dungeonFarm = dungeonFarmGlobal;

            int failCounter = 0;
            bool finished = false;
            DateTime startSolvingTime = DateTime.Now;

            int restarts = 0;

            Debug.WriteLine("Captcha solving start");

            while(!finished && failCounter < 4)
            {

                bool foundCrystal = false;
                solvingCaptcha = true;

                while(!foundCrystal && !finished)
                {
                    Wait(50);

                    Getscreen();

                    currentScreen = Colormode(5, currentScreen);

                    if(Pxl(744, 447) == Col(95, 223, 255))
                    {
                        foundCrystal = true;
                    }
                    else
                    {
                        Getscreen();

                        if (!CheckSky())
                        {

                            Debug.WriteLine("couldn't find crystal on captcha. Restart");
                            if (screenshotCaptchaErrors)
                            {
                                Screenshot(currentScreen, Cst.SCREENSHOT_CAPTCHA_ERRORS_PATH);
                            }

                            restarts++;
                            Debug.WriteLine(restarts + " restarts");
                            Restart();
                            abSkipNum++;
                            Replay();

                        }
                        else
                        {
                            Debug.WriteLine("Got in captcha solve block and didn't find crystal");
                            finished = true;
                        }
                    }

                }

                if (foundCrystal)
                {

                    Getscreen();

                    while(Pxl(420,732) == Col(75, 62, 52))
                    {
                        Debug.WriteLine("wait for captcha timer");
                        Wait(1000);
                        Getscreen();
                    }

                    bool clickAllowed = true;

                    InitCaptchaParams();

                    Debug.WriteLine("start click");
                    DateTime startClick = DateTime.Now;

                    RandomClickIn(1002, 671, 1123, 731);

                    int screenCounter = 0;

                    List<Bitmap> captchaScreens = new List<Bitmap>(SCREENS_COUNT);

                    while(screenCounter < SCREENS_COUNT - 1){

                        // captcha block coords: 504, 204, 972, 672

                        lastScreenTime = DateTime.Now;

                        //Getscreen(504, 204, 972, 672);
                        Getscreen();
                        captchaScreens.Add(CropBitmap(currentScreen, 504, 204, 972, 672));
                        currentFrameWait = (lastScreenTime - DateTime.Now + TimeSpan.FromMilliseconds(frameWait)).Milliseconds;

                        if(currentFrameWait > 0)
                        {
                            Wait(currentFrameWait);
                        }

                        screenCounter++;

                    }

                    Getscreen();
                    captchaScreens.Add(CropBitmap(currentScreen, 504, 204, 972, 672));

                    byte[] imageBytes = BitmapsToByteArray(captchaScreens, out int count, out int w, out int h, out int channels);

                    Debug.WriteLine("execute gca_captcha_solver.dll");

                    int returnedValue = -1;
                    double ratio0_1 = -1;
                    int trackedNumber = -1;

                    DateTime solvingStart = DateTime.Now;
                    try
                    {
                        returnedValue = execute(imageBytes, w, h, channels, count, captchaSaveScreenshotsAlways, false, out trackedNumber, out captchaAnswer, out ratio0_1, 0);
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        MessageBox.Show("Error occurred while solving captcha: \n" + e.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        Halt();
                    }
                    TimeSpan timeSolving = DateTime.Now - solvingStart;

                    CheckDllErrors(returnedValue);

                    Debug.WriteLine("returnedValue: " + returnedValue);
                    Debug.WriteLine("solving time: " + timeSolving);

                    Debug.WriteLine("answer: " + captchaAnswer);
                    Debug.WriteLine("0-1 ratio: " +  ratio0_1 + "%");
                    Debug.WriteLine("Time solving: " + timeSolving);

                    // wait to make sure that all boxes are clickable
                    if (DateTime.Now - startClick < TimeSpan.FromSeconds(4))
                    {
                        Wait((startClick - DateTime.Now + TimeSpan.FromSeconds(4)).Milliseconds);
                    }

                    // LCLICK(719,278,762,338)  // 1
                    // LCLICK(823,312,866,373)  // 2
                    // LCLICK(857,414,899,477)  // 3
                    // LCLICK(820,517,864,584)  // 4
                    // LCLICK(719,551,760,618)  // 5
                    // LCLICK(615,516,661,582)  // 6
                    // LCLICK(582,411,625,478)  // 7
                    // LCLICK(616,309,660,372)  // 8
                    Action<int, int, int, int> clickAction = CAPTCHA_TEST_MODE ? RandomMoveIn : RandomClickIn;


                    switch (captchaAnswer)
                    {
                        case 0:
                            clickAction(719, 278, 762, 338);
                            break;
                        case 1:
                            clickAction(823, 312, 866, 373);
                            break;
                        case 2:
                            clickAction(857, 414, 899, 477);
                            break;
                        case 3:
                            clickAction(820, 517, 864, 584);
                            break;
                        case 4:
                            clickAction(719, 551, 760, 618);
                            break;
                        case 5:
                            clickAction(615, 516, 661, 582);
                            break;
                        case 6:
                            clickAction(582, 411, 625, 478);
                            break;
                        case 7:
                            clickAction(616, 309, 660, 372);
                            break;
                        default:
                            Debug.WriteLine("Wrong captcha answer");
                            break;

                    }

                    Wait(500);
                    Getscreen();

                    TimeSpan totalSolvingTime = DateTime.Now - startSolvingTime;

                    bool solved = false;
                    

                    if(Pxl(547,134) == Col(98, 87, 73))
                    {

                        failCounter++;
                        Debug.WriteLine("Fail " + failCounter);

                        LClick(790, 714);

                        if (captchaSaveFailedScreenshots)
                        {

                            Debug.WriteLine("execute gca_captcha_solver.dll (FAIL mode)");

                            DateTime failModeSolvingStart = DateTime.Now;
                            try
                            {
                                returnedValue = execute(imageBytes, w, h, channels, count, captchaSaveScreenshotsAlways, true, out trackedNumber, out captchaAnswer, out ratio0_1, 0);
                            }
                            catch (Exception e) when (e is not OperationCanceledException)
                            {
                                MessageBox.Show("Error occurred while solving captcha in fail mode: \n" + e.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                Halt();
                            }
                            timeSolving = DateTime.Now - failModeSolvingStart;

                            CheckDllErrors(returnedValue);
                            string durationSolving = timeSolving.ToString("hh\\:mm\\:ss\\.fffffff");

                            Debug.WriteLine($"Saved screens of failed captcha in {durationSolving}");

                            Wait(200);

                        }

                    }
                    else
                    {

                        solved = true;
                        solvingCaptcha = false;

                        Debug.WriteLine($"Catpcha solved in {totalSolvingTime}");

                        finished = true;

                        lastReplayTime = DateTime.Now;

                        if (autobattleMode)
                        {
                            abSkipNum++;
                        }

                        if(autobattleMode || waveCanceling)
                        {
                            replaysForSkip--;
                            int quitCounter = 0;

                            while(quitCounter < 5 && Pxl(504,483) != Col(167, 118, 59) && Pxl(690,480) != Col(167, 118, 59) && Pxl(516,540) != Col(120, 85, 43) && Pxl(693,538) != Col(120, 85, 43) && Pxl(784,481) != Col(239, 209, 104) && Pxl(1024,536) != Col(235, 170, 23) && Pxl(869,486) != Col(242, 190, 35))
                            {
                                quitCounter++;
                                RClick(830, 362);
                                Wait(500);
                                Getscreen();
                            }

                            if(quitCounter < 5)
                            {
                                LClick(915, 507);
                                Wait(300);
                            }

                        }

                        if(restarts > 0 && dungeonNumber > 6 && dungeonFarm)
                        {
                            RClick(830, 362);
                            Wait(300);
                            LClick(915, 507);
                            Wait(300);
                        }

                    }

                    string solvedStateString = solved ? "+" : "-";
                    string startTimeString = startSolvingTime.ToString("dd.MM.yyyy HH:mm:ss.fff");
                    string solvingTimeString = totalSolvingTime.ToString("hh\\:mm\\:ss\\.fffffff");
                    string trackedAndAnswer = $"{trackedNumber} => {captchaAnswer}";
                    string ratioString = $"Ratio: {ratio0_1.ToString("P2", System.Globalization.CultureInfo.InvariantCulture)}";
                    string restartsString = solved ? $" Restarts: {restarts}" : string.Empty;

                    string captchaLogEntry = $"[{solvedStateString}] [{startTimeString}] [{solvingTimeString}] [{trackedAndAnswer}] {ratioString}{restartsString}\n";

                    File.AppendAllText(Cst.CAPTCHA_LOG_FILE_PATH, captchaLogEntry);

                    Getscreen();
                    
                }
            }

        }

    }
}
