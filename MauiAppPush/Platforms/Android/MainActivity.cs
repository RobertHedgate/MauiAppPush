using Android.App;
using Android.Content.PM;
using Android.Mtp;
using Android.Nfc;
using Android.OS;
using AndroidX.AppCompat.App;
using Firebase.Messaging;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Maui.Storage;
using static Android.Provider.Settings;
using Application = Android.App.Application;

namespace MauiAppPush
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private NotificationHubClient hub;
        private static string? GetDeviceId() => Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            try
            {
                Firebase.FirebaseApp.InitializeApp(this);
                // Get this from Azure Notification Hub
                var hubName = "<NotificationHubName>";
                var connectionString = "<ListenConnectionString>"; // Can be found in Access policy. Use Listen connection

                hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);

                CreateNotificationChannel();
            }
            catch (Exception ex)
            {
            }
        }

        private async void CreateNotificationChannel()
        {
            var token = await SecureStorage.GetAsync("FireBaseToken");

            if (token == null)
            {
                token = FirebaseMessaging.Instance.GetToken().ToString();
            }

            var installation = new Microsoft.Azure.NotificationHubs.Installation
            {
                InstallationId = GetDeviceId(),
                PushChannel = token,
                Platform = NotificationPlatform.FcmV1,
                Tags = ["TestTag"]
            };
            await hub.CreateOrUpdateInstallationAsync(installation);

            // There is no need to create a notification channel on older versions of Android.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelName = "default";
                var channelDescription = string.Empty;
                var channel = new NotificationChannel(channelName, channelName, NotificationImportance.High)
                {
                    Description = channelDescription
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}
