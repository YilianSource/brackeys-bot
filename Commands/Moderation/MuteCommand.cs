using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Linq;

using Discord.Commands;
using Discord;

namespace BrackeysBot.Commands.Moderation
{
    public class MuteCommand : ModuleBase
    {

        private readonly MuteTable _mutes;

        public MuteCommand(MuteTable mutes)
        {
            _mutes = mutes;
        }

        [Command("tempmute")]
        [HelpData("tempmute <member> <duration in hours> <reason> (optional)", "Mute a member for a specified amount of time.", AllowedRoles = UserType.Staff)]
        public async Task TempMute(IGuildUser user, double duration, [Optional] [Remainder] string reason)
        {
            _mutes.Set(user.Id.ToString() + "," + Context.Guild.Id.ToString(), (DateTime.UtcNow + new TimeSpan((long)(duration * TimeSpan.TicksPerHour))).ToBinary().ToString());
            string _displayName = user.GetDisplayName();
            await user.AddRoleAsync(Context.Guild.Roles.First(x => x.Name == "Muted"));
            IMessage messageToDel = await ReplyAsync($":white_check_mark: Successfully muted {_displayName} for {duration} hours.");
            _ = messageToDel.TimedDeletion(3000);
            await Context.Message.DeleteAsync();
        }

        [Command("mute")]
        [HelpData("mute <member> <reason> (optional)", "Mute a member.", AllowedRoles = UserType.Staff)]
        public async Task Mute(IGuildUser user, [Optional] [Remainder] string reason)
        {
            _mutes.Set(user.Id.ToString() + "," + Context.Guild.Id.ToString(), (long.MaxValue.ToString()));
            string _displayName = user.GetDisplayName();
            await user.AddRoleAsync(Context.Guild.Roles.First(x => x.Name == "Muted"));
            IMessage messageToDel = await ReplyAsync($":white_check_mark: Successfully muted {_displayName}.");
            _ = messageToDel.TimedDeletion(3000);
            await Context.Message.DeleteAsync();
        }

        [Command("unmute")]
        [HelpData("unmute <member>", "Mute a member.", AllowedRoles = UserType.Staff)]
        public async Task Unmute(IGuildUser user)
        {
            _mutes.Set(user.Id.ToString() + "," + Context.Guild.Id.ToString(), long.MinValue.ToString());
            string _displayName = user.GetDisplayName();
            await user.RemoveRoleAsync(Context.Guild.Roles.First(x => x.Name == "Muted"));
            IMessage messageToDel = await ReplyAsync($":white_check_mark: Successfully unmuted {_displayName}.");
            _ = messageToDel.TimedDeletion(3000);
            await Context.Message.DeleteAsync();
        }
    }
}
