using Discord;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Aspid
{
    class Program
    {
        internal static DiscordSocketClient _client;
        CommandHandler _handler;

        const string LinuxPrefix = "/home/xentellion/Aspid1/Data/AspidDataBase.db";

        internal static string connectionString = "C:/Data/AspidDataBase.db";

        internal static SqliteConnection sqliteConnection;

        static OverwritePermissions permissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Deny, PermValue.Deny);

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!File.Exists(LinuxPrefix))
                {
                    File.Create(LinuxPrefix);
                    return;
                }
                sqliteConnection = new SqliteConnection
                {
                    ConnectionString = "Filename = " + LinuxPrefix
                };
            }
            else
            {
                if (!File.Exists(connectionString))
                {
                    File.Create(connectionString);
                    return;
                }
                sqliteConnection = new SqliteConnection
                {
                    ConnectionString = "Filename = " + connectionString
                };
            }

            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });

            _client.Log += Log;

            sqliteConnection.Open();
            Console.WriteLine(DateTime.Now.TimeOfDay + " Connected to SQLite database " + sqliteConnection.Database);

            _client.Ready += Modules.Repeater.Muted;

            _client.Ready += FondGuild;
            _client.Ready += GetGrubs;
            _client.Ready += GetGuilds;

            _client.UserJoined += AnnounceUserJoined;
            _client.UserLeft += AnnounceUserLeft;

            _client.ChannelCreated += AddMute;

            _client.JoinedGuild += AddRoles;

            _client.ReactionAdded += AddedReaction;

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.SetGameAsync("ваши крики", null, ActivityType.Listening);
            await _client.StartAsync();

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private Task GetGuilds()
        {
            IEnumerable<SocketGuild> guilds = _client.Guilds;
            foreach(SocketGuild guild in guilds)
            {
                try
                {
                    SqliteCommand check = new SqliteCommand("SELECT * FROM guild_" + guild.Id, sqliteConnection);
                    SqliteDataReader reader = check.ExecuteReader();
                    reader.Close();
                }
                catch
                {
                    SqliteCommand addTable = new SqliteCommand(Queries.CreateTable(guild.Id), sqliteConnection);
                    addTable.ExecuteNonQuery();
                }

                foreach(SocketGuildUser user in guild.Users)
                {
                    SqliteCommand check = new SqliteCommand(Queries.GetUser(guild.Id, user.Id), sqliteConnection);
                    long i = Convert.ToInt64(check.ExecuteScalar());
                    if (i == 0)
                    {
                        SqliteCommand addUser = new SqliteCommand(Queries.AddUser(guild.Id, user.Id), sqliteConnection);
                        addUser.ExecuteNonQuery();
                    }
                }
            }
            return Task.CompletedTask;
        }

        #region Role Setting

        public async Task AddRoles(SocketGuild arg)
        {
            SqliteCommand addTable = new SqliteCommand(Queries.CreateTable(arg.Id), sqliteConnection);
            addTable.ExecuteNonQuery();

            IEnumerable<SocketRole> role = arg.Roles;

            if (Select(role, "Muted") == null)
                await arg.CreateRoleAsync("Muted", new GuildPermissions(1024), new Color(129, 131, 134), true);

            if (Select(role, "Punished") == null)
                await arg.CreateRoleAsync("Punished");

            if (Select(role, "Dead") == null)
                await arg.CreateRoleAsync("Dead", new GuildPermissions(), new Color(0, 0, 1), true);
        }

        public static async Task AddChannelRestr(SocketGuild arg)
        {
            IEnumerable<SocketGuildChannel> channels = arg.Channels;
            IEnumerable<SocketRole> roles = arg.Roles;
            var result = from b in roles
                         where b.Name == "Muted"
                         select b;
            IRole mute = result.FirstOrDefault();

            foreach (SocketGuildChannel a in channels)
            {
                try
                {
                    await a.AddPermissionOverwriteAsync(mute, permissions);
                }
                catch { }
            }
        }

        private Task AddMute(SocketChannel arg)
        {
            try
            {
                SocketRole role = (arg as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
                (arg as IGuildChannel).AddPermissionOverwriteAsync(role, permissions);
            }
            catch{}
            return Task.CompletedTask;
        }

        #endregion

        static Task FondGuild() //для постинга аспидов
        {
            SocketGuild f = _client.GetGuild(567767402062807055);
            Modules.Global.channel = (ISocketMessageChannel)f.GetChannel(567770314642030592);
            return Task.CompletedTask;
        }

        public static async Task GetGrubs()
        {
            SocketChannel channel = _client.GetChannel(627615133115482149);
            Modules.Global.messages = await (channel as ISocketMessageChannel).GetMessagesAsync(1000).FlattenAsync();
            Console.WriteLine();
        }

        #region Join/left

        private async Task AnnounceUserJoined(SocketGuildUser arg)
        {
            EmbedBuilder builder1 = new EmbedBuilder();
            builder1
                .WithColor(Color.DarkGreen)
                .WithCurrentTimestamp()
                .WithThumbnailUrl(arg.GetAvatarUrl())
                .WithDescription("**" + (arg as IUser).Username + Language(63, arg.Guild.Id))
                .WithTitle(Language(64, arg.Guild.Id));
            await arg.Guild.DefaultChannel.SendMessageAsync("", false, builder1.Build());

            var channel = _client.GetChannel(567770314642030592) as SocketTextChannel;
            if(arg.Guild.Id == 567767402062807055)
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder
                    .WithTitle($"Добро пожаловать!")
                    .WithDescription($"Должно быть, долгим был твой путь, {arg.Mention}, - и потому мы не будем томить тебя длинными речами. Все, что тебе нужно знать, от наших законов и обычаев, до путеводителя по нашему миру, ты можешь найти здесь - в разделе {arg.Guild.GetTextChannel(567768871654916112).Mention}. Если же тебе понадобится помощь или возникнут вопросы - то мы всегда будем ждать тебя у костра - {arg.Guild.GetTextChannel(567770314642030592).Mention}. А пока что, осматривайся, осваивайся, отдыхай после долгой дороги.\n" + "\n" +
                                    "И когда будешь готов - узри величайшую и единственную цивилизацию - Королевство Халлоунест!")
                    .WithColor(Color.DarkBlue);
                await channel.SendMessageAsync("", false, builder.Build());
            }

            SqliteCommand npgSqlCommand = new SqliteCommand(Queries.AddUser(arg.Guild.Id, arg.Id), sqliteConnection);
            npgSqlCommand.ExecuteNonQuery();
        }

        private async Task AnnounceUserLeft(SocketGuildUser arg)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithColor(Color.Red)
                .WithCurrentTimestamp()
                .WithThumbnailUrl(arg.GetAvatarUrl())
                .WithDescription($"**" + (arg as IUser).Username + Language(65, arg.Guild.Id))
                .WithTitle(Language(66, arg.Guild.Id));
            await arg.Guild.DefaultChannel.SendMessageAsync("", false, builder.Build());

            try
            {
                SqliteCommand delUser = new SqliteCommand(Queries.DeleteUser(arg.Guild.Id, arg.Id), sqliteConnection);
                delUser.ExecuteNonQuery();
            }
            catch { }
        }

        #endregion

        #region Misc

        private Task AddedReaction(Cacheable<IUserMessage, ulong> cashe, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.Value.IsBot)
            {
                if (Modules.Global.HelpHandler.Item1 != null && reaction.MessageId == Modules.Global.HelpHandler.Item1.Id)
                {
                    if (reaction.Emote.Name == "◀" || reaction.Emote.Name == "▶")
                    {
                        Modules.Global.HelpHandler.Item1.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                        Modules.Ping.Turn(reaction.Emote.Name);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        SocketRole Select(IEnumerable<SocketRole> role, string oof)
        {
            var result = from a in role
                         where a.Name == oof
                         select a;
            return result.FirstOrDefault();
        }

        string Language(int id, ulong guild)
        {
            switch (_client.GetGuild(guild).VoiceRegionId)
            {
                case "russia": return Modules.Languages.Russian.texts[id];
                default: return Modules.Languages.English.texts[id];
            }
        }

        #endregion
    }
}
