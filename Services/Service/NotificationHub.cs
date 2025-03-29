using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Service
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var roleIds = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value); // Giá trị là roleId, ví dụ: "4"

                foreach (var roleId in roleIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, roleId); // Gán vào group theo roleId
                }
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var roleIds = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value); // Giá trị là roleId, ví dụ: "4"

                foreach (var roleId in roleIds)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roleId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
