using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DCordBot
{
    class CustomPrecondition : PreconditionAttribute
    {

        private readonly ulong _roleId;

        public CustomPrecondition(ulong roleId)
        {
            _roleId = roleId;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)

        {
            var guildUser = context.User as IGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("This command cannot be executed outside of a guild.");

            var guild = guildUser.Guild;
            if (guild.Roles.All(r => r.Id != _roleId))
                return PreconditionResult.FromError(
                    $"The guild does not have the role ({_roleId}) required to access this command.");

            return guildUser.RoleIds.Any(rId => rId == _roleId)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("You do not have the sufficient role required to access this command.");
        }
    }
}
