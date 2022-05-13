using Unity.Notifications.Android;
using Unity.Notifications.iOS;

public interface IPushNotificationsManager
{
    public int ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds, string iOsIdentifier="");
    public void UnScheduleNotification(string identifier, int id);
}

public class PushNotificationsManager : IPushNotificationsManager
{
    private bool isInitialized = false;

    public int ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds, string iOsIdentifier="")
    {

        if(!isInitialized)
        {
            Initialize();
            isInitialized = true;
        }
        int id = -1;

#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddSeconds(FireTimeSeconds);
        notification.SmallIcon = "logo";

        id = AndroidNotificationCenter.SendNotification(notification, "channel_id");
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
            Identifier = iOsIdentifier,
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

    return id;
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

    public void UnScheduleNotification(string identifier, int id)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelNotification(id);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(identifier);
#endif
    }
}
