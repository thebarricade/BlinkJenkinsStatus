using System;
using System.IO;
using System.Net;
using System.Timers;

namespace BlinkJenkinsStatus
{
    public class JenkinsStatusChecker
    {
        private JenkinsProperties.JenkinsStatus _jenkinsStatus { get; set; }
        private readonly Timer _timer;
        private string _jenkinsUrl = "http://stosdfs01:8080";
        private long _checkInterval = 10000;
        private readonly BlinkManager _blinkManager;

        private static string GetJenkinsStatusJSON(string statusUrl)
        {
            string responseFromServer = "";
            try
            {
                WebRequest r = WebRequest.Create(statusUrl);

                var response = r.GetResponse();

                Stream dataStream = response.GetResponseStream();
                if (dataStream != null)
                {
                    StreamReader reader = new StreamReader(dataStream);

                    responseFromServer = reader.ReadToEnd();

                    reader.Close();
                }
                response.Close();

            }
            catch (Exception)
            {
                // suppress all errors
            }

            return responseFromServer;
        }

        public void Start()
        {
            var bl = new BlinkManager();
            bl.BlinkOff();

            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();

            var bl = new BlinkManager();
            bl.BlinkOff();
        }

        public JenkinsStatusChecker()
        {
            _blinkManager = new BlinkManager();
            _jenkinsStatus = JenkinsProperties.JenkinsStatus.Unchecked;

            _timer = new Timer(_checkInterval) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => CheckJenkins(_jenkinsUrl);
        }

        public void CheckJenkins(string jenkinsUrl)
        {
            JenkinsProperties.JenkinsStatus status = JenkinsProperties.JenkinsStatus.Unchecked;
            int OK = 0;
            int Err = 0;
            int Mix = 0;

            try
            {
                string responseFromServer = GetJenkinsStatusJSON(jenkinsUrl + "/api/json");

                if (responseFromServer == "")
                {
                    throw new Exception("No response from server");
                }

                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseFromServer);


                foreach (dynamic d in json.jobs)
                {

                    if (((string)d.color) == "blue")
                    {
                        OK++;
                    }
                    if (((string)d.color).Contains("anime"))
                    {
                        Mix++;
                    }
                    if (((string)d.color) == "red")
                    {
                        Err++;
                    }
                }
            }
            catch (Exception ex)
            {
                Err = 1;
                status = JenkinsProperties.JenkinsStatus.BuildError;
            }

            if (OK > 0) { status = JenkinsProperties.JenkinsStatus.BuildOK; }
            if (Err > 0) { status = JenkinsProperties.JenkinsStatus.BuildError; }
            if (Mix > 0) { status = JenkinsProperties.JenkinsStatus.BuildMixed; }

            if (_jenkinsStatus != status)
            {
                _blinkManager.Manage(status);
            }

            _jenkinsStatus = status;
        }
    }
}