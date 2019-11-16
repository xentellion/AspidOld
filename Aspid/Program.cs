using Discord;
using Discord.WebSocket;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aspid
{
    class Program
    {
        internal static DiscordSocketClient _client;
        CommandHandler _handler;

        internal static string connectionString =

                "Host=localhost;" +
                "Port=5432;" +
                "Database=aspid_user_database;" +
                "Username=postgres;" +
                "Password=admin;"
                ;

        internal static NpgsqlConnection npgSqlConnection;    

        static OverwritePermissions permissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Deny, PermValue.Deny);

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });

            _client.Log += Log;

            npgSqlConnection = new NpgsqlConnection
            {
                ConnectionString = connectionString
            };

            npgSqlConnection.Open();
            Console.WriteLine(DateTime.Now.TimeOfDay + " Connected to PostgreSQL database " + npgSqlConnection.Database);

            //_client.GuildAvailable += CheckLanguage();

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
                    NpgsqlCommand check = new NpgsqlCommand("SELECT * FROM guild_" + guild.Id, npgSqlConnection);
                    NpgsqlDataReader reader = check.ExecuteReader();
                    reader.Close();
                }
                catch
                {
                    NpgsqlCommand addTable = new NpgsqlCommand(Queries.CreateTable(guild.Id), npgSqlConnection);
                    addTable.ExecuteNonQuery();
                }

                foreach(SocketGuildUser user in guild.Users)
                {
                    NpgsqlCommand check = new NpgsqlCommand(Queries.GetUser(guild.Id, user.Id), npgSqlConnection);
                    long i = Convert.ToInt64(check.ExecuteScalar());
                    if (i == 0)
                    {
                        NpgsqlCommand addUser = new NpgsqlCommand(Queries.AddUser(guild.Id, user.Id), npgSqlConnection);
                        addUser.ExecuteNonQuery();
                    }
                }
            }
            return Task.CompletedTask;
        }

        #region Role Setting

        public async Task AddRoles(SocketGuild arg)
        {
            NpgsqlCommand addTable = new NpgsqlCommand(Queries.CreateTable(arg.Id), npgSqlConnection);
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
            SocketGuild f = _client.GetGuild(Config.bot.mainGuild);
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
                .WithDescription($"**{(arg as IUser).Username}** прибыл(а) на сервер")
                .WithTitle("Кто-то появился на пороге...");
            await arg.Guild.DefaultChannel.SendMessageAsync("", false, builder1.Build());

            var channel = _client.GetChannel(567770314642030592) as SocketTextChannel;
            if(arg.Guild.Id == Config.bot.mainGuild)
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder
                    .WithTitle($"Добро пожаловать!")
                    .WithDescription($"Должно быть, долгим был твой путь, {arg.Mention}, - и потому мы не будем томить тебя длинными речами. Все, что тебе нужно знать, от наших законов и обычаев, до путеводителя по нашему миру, ты можешь найти здесь - в разделе {arg.Guild.GetTextChannel(567768871654916112).Mention}. Если же тебе понадобится помощь или возникнут вопросы - то мы всегда будем ждать тебя у костра - {arg.Guild.GetTextChannel(567770314642030592).Mention}. А пока что, осматривайся, осваивайся, отдыхай после долгой дороги.\n" + "\n" +
                                    "И когда будешь готов - узри величайшую и единственную цивилизацию - Королевство Халлоунест!")
                    .WithColor(Color.DarkBlue);
                await channel.SendMessageAsync("", false, builder.Build());
            }

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand(Queries.AddUser(arg.Guild.Id, arg.Id), npgSqlConnection);
            npgSqlCommand.ExecuteNonQuery();
        }

        private async Task AnnounceUserLeft(SocketGuildUser arg)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithColor(Color.Red)
                .WithCurrentTimestamp()
                .WithThumbnailUrl(arg.GetAvatarUrl())
                .WithDescription($"**{(arg as IUser).Username}** покинул(а) сервер")
                .WithTitle("Чьи-то шаги затихают вдали...");
            await arg.Guild.DefaultChannel.SendMessageAsync("", false, builder.Build());

            try
            {
                NpgsqlCommand delUser = new NpgsqlCommand(Queries.DeleteUser(arg.Guild.Id, arg.Id), npgSqlConnection);
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
                if (Modules.Global.HelpHandler != null && reaction.MessageId == Modules.Global.HelpHandler.Id)
                {
                    if (reaction.Emote.Name == "◀" || reaction.Emote.Name == "▶")
                    {
                        Modules.Global.HelpHandler.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
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

        #endregion
    }
}
