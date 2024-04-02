using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Firebase.Messaging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppPush.Platforms.Android.Services
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        public const string PRIMARY_CHANNEL = "default";

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            string messageBody = string.Empty;

            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }

            // NOTE: test messages sent via the Azure portal will be received here
            else
            {
                if (message.Data.TryGetValue("message", out var messageString))
                {
                    messageBody = messageString;
                }
            }

            try
            {
                // convert the incoming message to a local notification
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SendLocalNotification(messageBody);
                });
            }
            catch
            {
            }
        }

        public bool IsForeground()
        {
            ActivityManager.RunningAppProcessInfo appProcessInfo = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(appProcessInfo);
            return (appProcessInfo.Importance == Importance.Foreground || appProcessInfo.Importance == Importance.Visible);
        }

        public override void HandleIntent(Intent intent)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {

                    base.HandleIntent(intent);
                });
            }
            catch (Exception ex)
            {
            }
        }

        public override async void OnNewToken(string token)
        {
            // Set the token name to secure storage
            await SecureStorage.SetAsync("FireBaseToken", token);
        }

        private void SendLocalNotification(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return;
            }
            NotificationManager notMan = (NotificationManager)GetSystemService(Context.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelDescription = string.Empty;
                var channel = new NotificationChannel(PRIMARY_CHANNEL, PRIMARY_CHANNEL, NotificationImportance.High);
                notMan.CreateNotificationChannel(channel);
            }

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", body);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var notificationBuilder = new NotificationCompat.Builder(this, PRIMARY_CHANNEL)
                .SetSmallIcon(Resource.Mipmap.appicon_round)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent)
                .SetNumber(6)
                .SetContentInfo("info");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                //notificationBuilder.SetSmallIcon(Resource.Drawable.);
                notificationBuilder.SetColor(GetColor(_Microsoft.Android.Resource.Designer.Resource.Color.colorPrimary));
            }
            else
            {
                notificationBuilder.SetSmallIcon(Resource.Mipmap.appicon_round);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationBuilder.SetChannelId(PRIMARY_CHANNEL);
            }

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(new Random().Next(), notificationBuilder.Build());
        }
    }
}
