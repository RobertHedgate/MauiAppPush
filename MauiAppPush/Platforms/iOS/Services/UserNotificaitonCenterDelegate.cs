using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace MauiAppPush.Platforms.iOS.Services
{
    public class UserNotificaitonCenterDelegate : UNUserNotificationCenterDelegate
    {
        public UserNotificaitonCenterDelegate() { }
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var r = notification?.Request;
            var c = r?.Content;
            var alertTitle = c?.Title ?? "App title";
            var alertBody = c?.Body;

            if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
            {
                if (alertBody != null)
                {
                    App.Current.MainPage.DisplayAlert(alertTitle, alertBody, "OK");
                }
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                completionHandler(UNNotificationPresentationOptions.Banner);
            else
                completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}
