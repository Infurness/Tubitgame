#if UNITY_IOS
    using Unity.Notifications.iOS;
#elif UNITY_ANDROID
    using Unity.Notifications.Android;
#endif

public interface IPushNotificationsManager
{
    public void ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds, int id);
    public void UnScheduleNotification(int id);
}

public class PushNotificationsManager : IPushNotificationsManager
{
    private bool isInitialized = false;

    public void ScheduleNotification(string title, string subtitle, string text, float FireTimeSeconds, int id)
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

        if(AndroidNotificationCenter.CheckScheduledNotificationStatus(id) != NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", id);
        }

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
            Identifier = id.ToString(),
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

        AndroidNotificationCenter.CancelAllDisplayedNotifications();
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

    public void UnScheduleNotification(int id)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelNotification(id);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
#endif
    }
}
