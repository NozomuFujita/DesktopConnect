using UnityEngine;

public class NotificationModel
{
    private static NotificationData _notificationData;
    public static NotificationData notificationData => _notificationData;
    private static NotificationPosition _notificationPosition;
    public static NotificationPosition notificationPosition => _notificationPosition;

    static NotificationModel()
    {
        _notificationData = new NotificationData();
        _notificationPosition = new NotificationPosition();
    }
}
