using Social_Media_Chatting_APP_Domain.Entities;
using Social_Media_Chatting_APP_Domain.Specifications;

namespace Social_Media_Chatting_APP_Service.Specification.PushSubscriptions;

/// <summary>
/// Fetches all push subscriptions belonging to a given user.
/// Used by PushNotificationService before sending to ensure we send to every
/// device the user has subscribed from (phone browser, laptop browser, etc.).
/// </summary>
public class PushSubscriptionByUserIdSpecification : BaseSpecification<PushSubscription>
{
    public PushSubscriptionByUserIdSpecification(string userId)
        : base(p => p.UserId == userId)
    {
    }
}
