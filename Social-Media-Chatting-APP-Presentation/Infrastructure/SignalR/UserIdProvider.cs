using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Social_Media_Chatting_APP_Presentation.Infrastructure.SignalR;

public class UserIdProvider : IUserIdProvider
{
  
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}