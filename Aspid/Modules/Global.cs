using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Aspid.Modules
{
    class Global
    {
        internal static ISocketMessageChannel channel = (ISocketMessageChannel)Program._client.GetGuild(567767402062807055).GetChannel(567770314642030592);

        internal static List<Discord.Rest.RestUserMessage> Cashe = new List<Discord.Rest.RestUserMessage>();
        internal static List<SocketMessage> GrubCast = new List<SocketMessage>();

        internal static List<Discord.Rest.RestUserMessage> missCashe = new List<Discord.Rest.RestUserMessage>();
        internal static List<SocketMessage> shooter = new List<SocketMessage>();

        internal static (Discord.Rest.RestUserMessage, string) HelpHandler { get; set; }
        internal static (Discord.Rest.RestUserMessage, int, string, string, ulong) CharacterSetter;
        /////////////////settled message/////////////state//record//name//guild
        internal static IEnumerable<Discord.IMessage> Messages { get; set; }

        internal static OverwritePermissions permissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Deny, PermValue.Deny);
    }
}
