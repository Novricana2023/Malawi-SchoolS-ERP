using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MaphunziroBlackboard.Web.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private static readonly Dictionary<string, HashSet<string>> UserConnections = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            lock (UserConnections)
            {
                if (!UserConnections.ContainsKey(userId))
                {
                    UserConnections[userId] = new HashSet<string>();
                }
                UserConnections[userId].Add(Context.ConnectionId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            lock (UserConnections)
            {
                if (UserConnections.ContainsKey(userId))
                {
                    UserConnections[userId].Remove(Context.ConnectionId);
                    if (UserConnections[userId].Count == 0)
                    {
                        UserConnections.Remove(userId);
                    }
                }
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendNotificationToUser(string userId, string title, string message)
    {
        await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
        {
            title,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendNotificationToGroup(string groupName, string title, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveNotification", new
        {
            title,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    public static bool IsUserOnline(string userId)
    {
        lock (UserConnections)
        {
            return UserConnections.ContainsKey(userId) && UserConnections[userId].Count > 0;
        }
    }

    public static int GetOnlineUserCount()
    {
        lock (UserConnections)
        {
            return UserConnections.Count;
        }
    }
}
