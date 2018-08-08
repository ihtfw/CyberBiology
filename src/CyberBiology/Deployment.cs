using System;
using System.Net;
using System.Threading.Tasks;
using Squirrel;

namespace CyberBiology
{
    class Deployment
    {
        public async Task<string> CheckForUpdates(Action<int> progress = null)
        {
            //hack to fix exception: The request was aborted: Could not create SSL/TLS secure channel
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            UpdateManager gitHubManager;
            try
            {
                gitHubManager = await UpdateManager.GitHubUpdateManager("https://github.com/ihtfw/CyberBiology");
            }
            catch 
            {
                //not deployed
                return "Not deployed";
            }

            try
            {
                var releaseEntry = await gitHubManager.UpdateApp(progress);
                if (releaseEntry != null)
                {
                    return gitHubManager.CurrentlyInstalledVersion() + " => " + releaseEntry.Version;
                }

                return gitHubManager.CurrentlyInstalledVersion().ToString();
            }
            catch
            {
                return gitHubManager.CurrentlyInstalledVersion() + " Error on update";
            }
            finally
            {
                gitHubManager.Dispose();
            }
        }
    }
}