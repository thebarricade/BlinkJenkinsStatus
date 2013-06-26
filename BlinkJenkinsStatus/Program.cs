using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Topshelf;

namespace BlinkJenkinsStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<JenkinsStatusChecker>(s =>
                {
                    s.ConstructUsing(name => new JenkinsStatusChecker());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Production and Delivery Jenkins Status Checker");
                x.SetDisplayName("TUI.JenkinsChecker");
                x.SetServiceName("TUI.JenkinsChecker");
            });
        }
    }
}
