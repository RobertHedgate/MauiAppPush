using Foundation;
using MauiAppPush.Platforms.iOS.Services;
using MetalPerformanceShaders;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Maui.Controls;
using UIKit;
using UserNotifications;

namespace MauiAppPush
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
            {
                if (granted && error == null)
                {
                    this.InvokeOnMainThread(() =>
                    {
                        UIApplication.SharedApplication.RegisterForRemoteNotifications();
                        UNUserNotificationCenter.Current.Delegate = new UserNotificaitonCenterDelegate();
                    });
                }
            });

            return result;
        }

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            string token = null!;
            if (deviceToken.Length > 0)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    var data = deviceToken.ToArray();
                    token = BitConverter
                        .ToString(data)
                        .Replace("-", "")
                        .Replace("\"", "");
                }
                else if (!string.IsNullOrEmpty(deviceToken.Description))
                {
                    token = deviceToken.Description.Trim('<', '>');
                }
            }

            // Get this from Azure Notification Hub
            var hubName = "<NotificationHubName>"; 
            var connectionString = "<ListenConnectionString>"; // Can be found in Access policy. Use Listen connection

            try
            {
                var hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
                var installation = new Installation
                {
                    InstallationId = token,
                    PushChannel = token,
                    Platform = NotificationPlatform.Apns,
                    Tags = ["TestTag"]
                };

                await hub.CreateOrUpdateInstallationAsync(installation);
            }
            catch (Exception ex) { }
        }
    }
}
