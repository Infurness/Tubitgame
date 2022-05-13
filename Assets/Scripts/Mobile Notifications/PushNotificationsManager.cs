using Unity.Notifications.Android;
using Unity.Notifications.iOS;

public interface IPushNotificationsManager
{
    public void ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds);
}

public class PushNotificationsManager : IPushNotificationsManager
{
    private bool isInitialized = false;

    public void ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds)
    {
        if(!isInitialized)
        {
            Initialize();
            isInitialized = true;
        }

#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddSeconds(FireTimeSeconds);
        notification.SmallIcon = "logo";

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
#elif UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, FireTimeSeconds),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = title,
            Body = text,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    private void Initialize()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
#endif
    }
}
