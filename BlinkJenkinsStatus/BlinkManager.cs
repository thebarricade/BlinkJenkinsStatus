using System;
using System.Threading;

namespace BlinkJenkinsStatus
{
    public class BlinkManager
    {
        private Blink1 blinkServer;

        private readonly BlinkColor BRed = new BlinkColor(255, 0, 0);
        private readonly BlinkColor BGreen = new BlinkColor(0, 255, 0);
        private readonly BlinkColor BBlue = new BlinkColor(0, 0, 255);
        private readonly BlinkColor BYellow = new BlinkColor(255, 255, 0);

        public int Interval { get; set; }

        public BlinkManager(int interval = 250)
        {
            Interval = interval;
        }

        public void FlashBlink(BlinkColor color, int times = 5)
        {
            for (int i = 0; i < times; i++)
            {
                blinkServer.setRGB(color.Red, color.Green, color.Blue);
                Thread.Sleep(Interval);
                blinkServer.setRGB(0, 0, 0);
                Thread.Sleep(Interval);
            }
        }

        public void SetBlinkColor(BlinkColor color)
        {
            blinkServer.setRGB(color.Red, color.Green, color.Blue);
        }

        public void BlinkOff()
        {
            var server = new Blink1();

            if (server.open())
            {
                server.setRGB(0, 0, 0);
                server.close();
            }
            else
            {
                throw new Exception("No Blink-device found.");
            };
        }

        public void Manage(JenkinsProperties.JenkinsStatus status)
        {
            BlinkColor bColor;

            switch (status)
            {
                case JenkinsProperties.JenkinsStatus.BuildError:
                    bColor = BRed;
                    break;
                case JenkinsProperties.JenkinsStatus.BuildMixed:
                    bColor = BBlue;
                    break;
                default:
                    bColor = BGreen;
                    break;
            }

            blinkServer = new Blink1();
            if (blinkServer.open())
            {
                FlashBlink(bColor, 5);
                SetBlinkColor(bColor);
                blinkServer.close();
            }
            else
            {
                //throw new Exception("No Blink-device found.");
            }
        }
    }
}