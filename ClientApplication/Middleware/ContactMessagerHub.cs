using DatabaseAccess;
using DatabaseAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ClientApplication.Middleware
{
    public partial class ContactMessagerHub : Hub, IAsyncDisposable
    {
        public record class ContactMessagerModel(Message Message, string Group);
        protected IDbContextFactory<DatabaseContext> DatabaseFactory { get; set; } = default!;
        public ContactMessagerHub(IDbContextFactory<DatabaseContext> factory) : base() 
        { this.DatabaseFactory = factory; }

        public async Task JoinGroup(string groupName)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат в группу {groupName}");
        }
        public async Task SendMessage(ContactMessagerModel messageModel)
        {
            using (var dbcontext = await this.DatabaseFactory.CreateDbContextAsync())
            {
                await dbcontext.Messages.AddAsync(messageModel.Message);
                //await dbcontext.SaveChangesAsync();
            }
            await this.Clients.Group(messageModel.Group).SendAsync("GetMessage", messageModel.Message);
        }
        public override Task OnConnectedAsync() => base.OnConnectedAsync();
        public override Task OnDisconnectedAsync(Exception? exception) => base.OnDisconnectedAsync(exception);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
