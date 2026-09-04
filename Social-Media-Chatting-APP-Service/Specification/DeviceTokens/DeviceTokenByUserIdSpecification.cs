using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.DeviceTokens;

/// <summary>
/// Fetches all FCM device tokens belonging to a given user.
/// Used by FcmNotificationService to fan-out to every registered device.
/// </summary>
public class DeviceTokenByUserIdSpecification : BaseSpecification<DeviceToken>
{
    public DeviceTokenByUserIdSpecification(string userId)
        : base(d => d.UserId == userId)
    {
    }
}
