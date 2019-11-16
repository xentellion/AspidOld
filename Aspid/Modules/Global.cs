using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aspid.Modules
{
    class Global
    {
        internal static ISocketMessageChannel channel = null;

        internal static List<Discord.Rest.RestUserMessage> Cashe = new List<Discord.Rest.RestUserMessage>();
        internal static List<SocketMessage> GrubCast = new List<SocketMessage>();

        internal static List<Discord.Rest.RestUserMessage> missCashe = new List<Discord.Rest.RestUserMessage>();
        internal static List<SocketMessage> shooter = new List<SocketMessage>();

        internal static Discord.Rest.RestUserMessage HelpHandler { get; set; }

        public static IEnumerable<Discord.IMessage> messages { get; set; }
    }
}
