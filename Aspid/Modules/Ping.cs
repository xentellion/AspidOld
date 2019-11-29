﻿using System;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Discord.Rest;
using System.Threading;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Aspid.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        public async Task Emotion(string emotion)
        {
            Emote emote = Emote.Parse(emotion);
            await Context.Message.AddReactionAsync(emote);
        }

        public async Task Check()
        {
            SocketRole role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished");
            if ((Context.User as SocketGuildUser).Roles.Contains(role))
            {
                await Context.Channel.DeleteMessageAsync(Context.Message);
                return;
            }
        }

        public string Language(int id)
        {
            switch (Context.Guild.VoiceRegionId)
            {
                case "russia":  return Languages.Russian.texts[id];
                default:        return Languages.English.texts[id];    
            }
        }

        #region Shoot

        [Command("gun")]
        public async Task RussianRulette()
        {
            await Check();
            await Emotion("<:revolver:603601152885522465>");
            var username = Context.User as SocketGuildUser;

            SocketRole role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Dead");

            if (username.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished")))
            {
                await Context.Channel.SendMessageAsync(Language(6), false);
                return;
            }

            if (username.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync(Language(7), false);
                return;
            }

            Thread thread = new Thread(x =>
            {
                Thread.Sleep(1000); Context.Channel.SendMessageAsync(Language(0), false);
                Thread.Sleep(2000); Context.Channel.SendMessageAsync(Language(1), false);
                Thread.Sleep(2000); Context.Channel.SendMessageAsync(Language(2), false);
                Thread.Sleep(4000);

                Random rand = new Random();
                int i = rand.Next(0, 6);
                if (i == 0)
                {
                    Config.bot.deadPeople++;

                    Context.Channel.SendMessageAsync(Language(3) + Context.User.Mention + Language(4), false);
                    (Context.User as IGuildUser).AddRoleAsync(role);
                }
                else Context.Channel.SendMessageAsync(Language(5), false);
            });

            thread.Start();
        }

        [Command("shoot")]
        public async Task Shoot(SocketGuildUser message)
        {
            await Check();
            await Emotion("<:revolver:603601152885522465>");

            bool breaker = false;

            SocketRole role1 = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Dead");

            if (message.IsBot)
            {
                await MissMessage(Language(8)); breaker = true;
            }
            if (message == Context.User || message == null)
            {
                await MissMessage(Language(9)); breaker = true;
            }
            else if ((Context.User as SocketGuildUser).Roles.Contains(role1))
            {
                await MissMessage(Language(10)); breaker = true;
            }
            else if ((Context.User as SocketGuildUser).Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished")))
            {
                await MissMessage(Language(11)); breaker = true;
            }
            else if (message.Roles.Contains(role1))
            {
                await MissMessage(Language(12)); breaker = true;
            }

            if (breaker) return;

            Random rand = new Random();
            int i = rand.Next(0, 2);
            if (i == 0)
            {
                Config.bot.deadPeople++;
                await Context.Channel.SendMessageAsync(Language(13) + message.Mention, false);
                await (message as IGuildUser).AddRoleAsync(role1);
            }
            else
            {
                await MissMessage(Language(14) + "<:PKHeh:575051447906074634>");
            }
        }

        async Task MissMessage(string oof)
        {
            RestUserMessage miss = await Context.Channel.SendMessageAsync(oof, false);

            Global.shooter.Add(Context.Message);
            Global.missCashe.Add(miss);

            if (Global.missCashe.Count > 3)
            {
                await Global.missCashe[0].DeleteAsync();
                Global.missCashe.RemoveAt(0);

                await Global.shooter[0].DeleteAsync();
                Global.shooter.RemoveAt(0);
            }
        }
        #endregion

        #region ShootControl

        [Command("save")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Resurrect()
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Dead");
            var users = Context.Guild.Users;
            int counter = 0;
            foreach(SocketGuildUser a in users)
            {
                var roles = a.Roles;
                var isDead = from b in roles
                             where b.Name == "Dead"
                             select b;
                var c = isDead.FirstOrDefault();
                if(c != null)
                {
                        await (a as IGuildUser).RemoveRoleAsync(role);
                    counter++;
                }
            }
            await Context.Channel.SendMessageAsync(">>> " + counter + Language(15) + Config.bot.deadPeople + Language(16), false);
        }


        [Command("punish")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Punish(SocketGuildUser user)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished");

            SqliteCommand command = new SqliteCommand(Queries.AddPunish(Context.Guild.Id, user.Id), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await (user as IGuildUser).AddRoleAsync(role);

            await Context.Channel.SendMessageAsync(Language(17) + user.Mention + Language(18), false);
        }

        #endregion

        #region Heroes

        [Command("hero")]
        public async Task GetCharacter(string name)
        {
            await Check();
            Hero character = new Hero();

            SqliteCommand getHero = new SqliteCommand(Queries.GetCharacter(Context.Guild.Id, name), Program.sqliteConnection);

            using (SqliteDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        character = new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            owner = Convert.ToUInt64(record["CHAR_OWNER"]),
                            description = Convert.ToString(record["DESCRIPTION"]),
                            image = Convert.ToString(record["CHAR_IMAGE"]),
                        };
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Language(19));
                    return;
                }
            }

            EmbedBuilder builder = new EmbedBuilder();
            SocketUser user = Context.Guild.GetUser(character.owner);
            builder
                .WithColor(Color.DarkBlue)
                .WithTitle(character.name)
                .WithDescription(character.description + Language(20) + user.Mention);
            if (character.image != null)
            {
                builder.WithImageUrl(character.image);
            }
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("hero")]
        public async Task GetCharacter(SocketGuildUser user)
        {
            await Check();

            List<Hero> characters = new List<Hero>();

            SqliteCommand getHero = new SqliteCommand(Queries.GetCharacter(Context.Guild.Id, user.Id), Program.sqliteConnection);

            using (SqliteDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        characters.Add(new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            owner = Convert.ToUInt64(record["CHAR_OWNER"]),
                            description = Convert.ToString(record["DESCRIPTION"]),
                            image = Convert.ToString(record["CHAR_IMAGE"]),
                        });
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Language(21));
                    return;
                }
            }

            if(characters.Count == 1)
            {
                EmbedBuilder builder = new EmbedBuilder();
                SocketUser guy = Context.Guild.GetUser(characters[0].owner);
                builder
                    .WithColor(Color.DarkBlue)
                    .WithTitle(characters[0].name)
                    .WithDescription(characters[0].description + Language(20) + user.Mention);
                if (characters[0].image != null)
                {
                    builder.WithImageUrl(characters[0].image);
                }
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else
            {
                string list = "";
                characters.Sort((x, y) => string.Compare(x.name, y.name));
                foreach (Hero hero in characters)
                {
                    list += hero.name;
                    list += "\n";
                }
                EmbedBuilder builder = new EmbedBuilder();
                builder
                    .WithDescription(list + $"\n\n{user.Mention}")
                    .WithColor(Color.Blue)
                    .WithTitle(Language(22));
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
        }

        [Command("add")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddCharacter(string name, SocketUser owner, [Remainder] string info)
        {
            SqliteCommand command = new SqliteCommand(Queries.AddChar(Context.Guild.Id, name, owner.Id, info), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);

            EmbedBuilder builder = new EmbedBuilder////
            {
                Title = name,
                Color = Color.Default,
                Description =
                Language(23) + name + Language(24) + "\n\n" +
               "Выберите основное оружие персонажа\n" +
               "0 - безоружен\n" +
               "1 - легкое\n" +
               "2 - среднее\n" +
               "3 - тяжелое"
            };
            RestUserMessage message = await Context.Channel.SendMessageAsync("", false, builder.Build());

            Emoji[] emojis = { new Emoji("0️⃣"), new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣") };
            await message.AddReactionsAsync(emojis);

            Global.CharacterSetter = (message, 0, "", name, Context.Guild.Id);
        }

        [Command("delete")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteCharacter(string name)
        {
            SqliteCommand command = new SqliteCommand(Queries.DeleteChar(Context.Guild.Id, name), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(Language(23) + name + Language(25));
        }

        [Command("update")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UpdateCharacter(string name, [Remainder] string info)
        {
            SqliteCommand command = new SqliteCommand(Queries.ChangeDescription(Context.Guild.Id, name, info), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(Language(23) + name + Language(26));
        }

        [Command("image")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PicCharacter(string name, string info)
        {
            SqliteCommand command = new SqliteCommand(Queries.ChangeImage(Context.Guild.Id, name, info), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(Language(23) + name + Language(26));
        }

        [Command("heroes")]
        public async Task AllBois()
        {
            await Check();

            List<Hero> characters = new List<Hero>();

            SqliteCommand getHero = new SqliteCommand(Queries.GetAllCharacters(Context.Guild.Id), Program.sqliteConnection);

            using (SqliteDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        characters.Add(new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"])
                        });
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Language(27));
                    return;
                }
            }

            characters.Sort((x, y) => string.Compare(x.name, y.name));

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithColor(Color.Default)
                .WithTitle(Language(28));

            foreach (Hero hero in characters)
            {
                string name = hero.name;
                name = name.Replace(@"\", "");

                int a = name.Length / 2;
                string b = "";

                if (a < 5)
                {
                    for (int i = 0; i < 5 - a; i++)
                        b += " ";
                }

                builder.AddField(x =>
                {
                    x.Name = "<:left:648177971185844235>-----------<:right:648177971504611338>";
                    x.Value = $"```{b + name}```";
                    x.IsInline = true;
                });
            }
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("roll")]
        public async Task RollDice([Remainder] string input = "20+0")
        {
            await Check();

            string a;

            string output = "";
            char action = '+';
            string added = "0";

            bool secPart = false;

            for (int j = 0; j < input.Length; j++)
            {
                a = input[j].ToString();

                bool isNum = int.TryParse(a, out int num);

                if (!isNum)
                    secPart = true;

                if (isNum)
                {
                    if (!secPart)
                        output += a;
                    else
                        added += a;
                }
                else
                {
                    if (a != " ")
                        action = input[j];
                }
            }

            if (output == "")
                output = "20";

            int message = Convert.ToInt32(output);
            int addValue = Convert.ToInt32(added);

            if (message > 100 || addValue > 100)
            {
                await Context.Channel.SendMessageAsync(Language(29)); return;
            }

            if (message < 2)
            {
                await Context.Channel.SendMessageAsync(Language(30)); return;
            }

            Random rand = new Random();
            int i = rand.Next(0, message) + 1;

            switch (action)
            {
                case '+': i += addValue; break;
                case '-': i -= addValue; break;
                default: await Context.Channel.SendMessageAsync("Incorrect sign"); return;
            }

            if (i > message) i = message;
            else if (i < 1) i = 1;

            EmbedBuilder builder = new EmbedBuilder();
            Color color;
            int z = message / 4;
            if (i == 1) color = Color.Default;
            else if (i <= z) color = Color.DarkRed;
            else if (i < z + z) color = Color.Red;
            else if (i <= z + z + z) color = Color.Green;
            else if (i < message) color = Color.DarkGreen;
            else color = Color.Gold;

            builder
                .WithColor(color)
                .WithDescription(Context.User.Mention + Language(31) + i.ToString() + "`")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/603600328117583874/627468862153293824/422823435530403850.png")
                .WithCurrentTimestamp();

            if (i == 13) builder.WithThumbnailUrl("https://media.discordapp.net/attachments/614108079545647105/633423003161591808/IMG_20190831_190131.jpg?width=355&height=474");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        #endregion

        #region Help

        [Command("help")]
        public async Task HelpAsync()
        {
            await Check();
            await Emotion("<:ThinkRadiance:567800282797309957>");
            if (Global.HelpHandler.Item1 != null)
            {
                try
                {
                    await Global.HelpHandler.Item1.DeleteAsync();
                }
                catch { }
            }
            Fields.curren = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle(Language(32))
                .WithColor(Color.DarkBlue);
            
            switch (Context.Guild.VoiceRegionId)
            {
                case "russia": 
                    builder
                        .AddField(Fields.FielderTitleRU[Fields.curren], Fields.FielderRU[Fields.curren])
                        .WithFooter(Language(33) + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitleRU.Length.ToString());
                    break;
                default:
                    builder
                        .AddField(Fields.FielderTitleEN[Fields.curren], Fields.FielderEN[Fields.curren])
                        .WithFooter(Language(33) + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitleEN.Length.ToString());
                    break;
            }

            RestUserMessage a = await Context.Channel.SendMessageAsync("", false, builder.Build());

            await a.AddReactionAsync(new Emoji("◀"));
            await a.AddReactionAsync(new Emoji("▶"));
            Global.HelpHandler = (a, Context.Guild.VoiceRegionId);
        }

        public static Task Turn(string side)
        {
            bool changed = false;

            if (side == null) return Task.CompletedTask;
            else if (side == "◀" && Fields.curren > 0)
            {
                Fields.curren--;
                changed = true;
            }
            else if (side == "▶" && Fields.curren < Fields.FielderTitleRU.Length - 1)
            {
                Fields.curren++;
                changed = true;
            }         

            if (changed)
            {
                Global.HelpHandler.Item1.ModifyAsync(a =>
                {
                    EmbedBuilder bruh = new EmbedBuilder()
                    //.WithTitle("***КОМАНДЫ БОТА***")
                    .WithColor(Color.DarkBlue);
                    
                    switch (Global.HelpHandler.Item2)
                    {
                        case "russia": 
                            bruh
                                .WithTitle(Languages.Russian.texts[32])
                                .AddField(Fields.FielderTitleRU[Fields.curren], Fields.FielderRU[Fields.curren])
                                .WithFooter("Стр. " + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitleRU.Length.ToString());
                            break;
                        default:
                            bruh
                                .WithTitle(Languages.English.texts[32])
                                .AddField(Fields.FielderTitleEN[Fields.curren], Fields.FielderEN[Fields.curren])
                                .WithFooter("Page " + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitleEN.Length.ToString());
                            break;
                    }
                    a.Embed = bruh.Build();
                });
                Global.HelpHandler.Item1.UpdateAsync();
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Miscellanous

        [Command("join")]
        public async Task AddBot()
        {
            IDMChannel ls = await Context.User.GetOrCreateDMChannelAsync();
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(Language(34))
                .WithColor(Color.DarkGreen)
                .WithDescription("https://discordapp.com/oauth2/authorize?&client_id=581221797295554571&scope=bot&permissions=1345846480")
                .WithThumbnailUrl("https://media.discordapp.net/attachments/603600328117583874/615150515210420249/primal_aspid_king.gif");
            await ls.SendMessageAsync("", false, builder.Build());

        }

        [Command("batya")]
        public async Task SendBatya()
        {
            await Check();
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithColor(Color.DarkGreen)
                .WithImageUrl("https://cdn.discordapp.com/attachments/557592875735580702/581237030663618570/IMG_20190502_170310.jpg");
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        
        [Command("vote")]
        public async Task CreateVote([Remainder] string message)
        {
            await Check();
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle(Language(35))
                .WithDescription(
                    message + "\n\n" +
                    "<:THK_Good:575051447599628311>" + Language(36) +
                    "<:THK_Bad:575796719078473738>" + Language(37))
                .WithFooter("Предложил(а) " + (Context.User as SocketGuildUser).Nickname)
                .WithColor(Color.Red);
            await Context.Message.DeleteAsync();

            RestUserMessage lmao = await Context.Channel.SendMessageAsync("", false, builder.Build());
            await lmao.AddReactionAsync(Emote.Parse("<:THK_Good:575051447599628311>"));
            await lmao.AddReactionAsync(Emote.Parse("<:THK_Bad:575796719078473738>"));

        }

        [Command("code")]
        public async Task Code()
        {
            await Check();
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle(Language(38))
                .WithDescription(Language(39))
                .WithColor(Color.Red)
                .WithImageUrl("https://media.discordapp.net/attachments/614108079545647105/629782304738377738/3032408cf9e547dc.png")
                .WithCurrentTimestamp();
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("server")]
        public async Task ServerInfo()
        {
            await Check();
            SocketGuild guild = Context.Guild;
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle(Language(40))
                .WithDescription(
                Language(41) + guild.Name + Language(42) + guild.CreatedAt.ToLocalTime() + "\n\n" +
                Language(43) + guild.Users.Count + Language(44) + guild.Roles.Count + Language(45) + guild.Channels.Count + Language(46)
                )
                .WithCurrentTimestamp()
                .WithThumbnailUrl(guild.IconUrl)
                .WithColor(Color.DarkPurple);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("servers")]
        public async Task Servers()
        {
            if (Context.User.Id != 264811248552640522)
            {
                await Context.Message.DeleteAsync();
                return;
            }
            IEnumerable<SocketGuild> guilds = Program._client.Guilds;
            foreach(SocketGuild guild in guilds)
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder
                    .WithTitle("**ИНФОРМАЦИЯ О СЕРВЕРЕ**")
                    .WithDescription(
                    Language(41) + guild.Name + Language(42) + guild.CreatedAt.ToLocalTime() + "\n\n" +
                    Language(43) + guild.Users.Count + Language(44) + guild.Roles.Count + Language(45) + guild.Channels.Count + Language(46)
                    )
                    .WithCurrentTimestamp()
                    .WithThumbnailUrl(guild.IconUrl)
                    .WithColor(Color.DarkPurple);
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
        }

        [Command("global")]
        public async Task GlobalAnnounce([Remainder] string text)
        {
            if (Context.User.Id != 264811248552640522)
            {
                await Context.Message.DeleteAsync();
                return;
            }

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle("ATTENCION")
                .WithColor(Color.Default)
                .WithDescription(text);

            IEnumerable<SocketGuild> guilds = Program._client.Guilds;
            foreach (SocketGuild guild in guilds)
            {
                await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
            }
        }

        #endregion

        #region PowerHandler

        [Command("stop")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task KillAspid()
        {
            SocketGuild myguild = Context.Guild;
            SocketRole role = myguild.Roles.FirstOrDefault(x => x.Name == "Muted");
            SocketUser bot = myguild.GetUser(581221797295554571);
            await Context.Channel.SendMessageAsync(Language(47));
            await (bot as IGuildUser).AddRoleAsync(role);
        }

        [Command("start")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StartAspid()
        {
            SocketGuild myguild = Context.Guild;
            SocketRole role = myguild.Roles.FirstOrDefault(x => x.Name == "Muted");
            SocketUser bot = myguild.GetUser(581221797295554571);
            await (bot as IGuildUser).RemoveRoleAsync(role);
            await Context.Channel.SendMessageAsync(Language(48));
        }

        [Command("break")]
        public async Task StopBot()
        {
            if (Context.User.Id != 264811248552640522)
            {
                await Context.Message.DeleteAsync();
                return;
            }

            Config.SaveDead();

            await Context.Channel.SendMessageAsync("", false,
            new EmbedBuilder()
            .WithTitle(Language(49))
            .WithColor(Color.DarkGreen)
            .WithImageUrl("https://media.discordapp.net/attachments/603600328117583874/615150516388757509/image0-5.png")
            .Build());

            try
            {
                await Global.HelpHandler.Item1.DeleteAsync();
            }
            catch { Console.WriteLine("Cannot delete that message"); }
            
            Environment.Exit(1);
        }
        
        #endregion

        #region Mute

        [Command("mute")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Mute(SocketGuildUser user, string timer, [Remainder]string reason = null)
        {
            if (user.IsBot)
            {
                await Context.Message.DeleteAsync();
                return;
            }

            char[] a;
            a = timer.ToCharArray();

            char[] b = new char[a.Count() - 1];

            for (int i = 0; i < b.Count(); i++)
            {
                b[i] = a[i];
            }
            char time = a.Last();
            string c = new string(b);
            int muteTime = Convert.ToInt32(c);
            int show = muteTime;

            string longitud;
            switch (time)
            {
                case 'm': longitud = Language(50); break;
                case 'h': muteTime *= 60; longitud = Language(51); break;
                case 'd': muteTime *= 1440; longitud = Language(52); break;
                case 'w': muteTime *= 10080; longitud = Language(53); break;
                case 'y': muteTime *= 3679200; longitud = Language(54); break;
                default: return;
            }

            await Context.Channel.SendMessageAsync("🔇 "  + user.Mention + Language(55) + show + " " + longitud + "**");

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");

            SqliteCommand command = new SqliteCommand(Queries.AddMute(Context.Guild.Id, user.Id, (ulong)muteTime), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await user.AddRoleAsync(role);

            if((Context.Channel as IGuildChannel).GetPermissionOverwrite(role) == null) 
                await Program.AddChannelRestr(Context.Guild);

            var ls = await user.GetOrCreateDMChannelAsync();
            EmbedBuilder builder = new EmbedBuilder();
            if (reason == null) reason = Language(56);
            builder.WithTitle(Language(57))
                .WithDescription(Language(58) + Context.Guild.Name + Language(59) + show + " " + longitud + Language(60) + reason)
                .WithColor(Color.Red)
                .WithThumbnailUrl("https://media.discordapp.net/attachments/603600328117583874/615150515709411357/ezgif.com-gif-maker_31.gif")
                .WithCurrentTimestamp();
            await ls.SendMessageAsync("", false, builder.Build());
        }

        public static async Task RemoveMute(ulong guildId, ulong id, string roleName)
        {
            SocketGuild guild = Program._client.GetGuild(guildId);
            var role = guild.Roles.FirstOrDefault(x => x.Name == roleName);
            SocketGuildUser user = guild.GetUser(id);
            await (user as IGuildUser).RemoveRoleAsync(role);
        }

        #endregion

        #region Purge

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("purge")]
        public async Task Purge(int amount = 0)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("purge")]
        public async Task Purge(string type, string oof = "(", int amount = 100)
        {
            if (amount > 100 || amount < 0) return;
            if (type == "startswith")
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
                List<IMessage> clone = new List<IMessage>();
                foreach (IMessage a in messages)
                {
                    if (a.Content.StartsWith(oof))
                    {
                        clone.Add(a);
                    }
                }
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(clone);
                await ((ITextChannel)Context.Channel).DeleteMessageAsync(Context.Message);
            }
        }

        #endregion

        #region Grubs

        [Command("grub")]
        public async Task Grub()
        {
            await Emotion("<:cosplay:603601170346541058>");
            Random rand = new Random();
            int i = rand.Next(0, Global.Messages.Count());
            string grub = Global.Messages.ElementAt(i).Attachments.First().Url;
            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithImageUrl(grub)
                .WithColor(Color.Green);
            RestUserMessage oof = await Context.Channel.SendMessageAsync("", false, builder.Build());

            Global.Cashe.Add(oof);
            Global.GrubCast.Add(Context.Message);
            if (Global.Cashe.Count > 2)
            {
                await Global.Cashe[0].DeleteAsync();
                Global.Cashe.RemoveAt(0);

                await Global.GrubCast[0].DeleteAsync();
                Global.GrubCast.RemoveAt(0);
            }
        }

        [Command("refresh")]
        public async Task Refresh()
        {
            SocketChannel channel = Program._client.GetChannel(627615133115482149);
            Global.Messages = await (channel as ISocketMessageChannel).GetMessagesAsync().FlattenAsync();
            await Context.Message.DeleteAsync();
        }

        #endregion

        #region Aspid Interactions

        [Command("pet")]
        public async Task Pet()
        {
            await Check();
            await Emotion("<:Aspid:567801319197245448>");
            var username = Context.User as SocketGuildUser;
            SocketGuild myguild = Context.Guild;
            SocketRole role = myguild.Roles.FirstOrDefault(x => x.Name == "Dead");
            if (username.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync(Language(61));
                return;
            }
            string[] frase;

            switch (Context.Guild.VoiceRegionId)
            {
                case "russia": frase = Languages.Russian.pets; break;
                default: frase = Languages.English.pets; break;
            }

            Random rand = new Random();
            int i = rand.Next(0, frase.Length);
            if (i == 0)
            {
                Config.bot.deadPeople++;
                await (username as IGuildUser).AddRoleAsync(role);
            }
            await Context.Channel.SendMessageAsync(frase[i]);
        }

        [Command("ask")]
        public async Task Ask([Remainder]string question)
        {
            await Check();
            await Emotion("<:Aspid:567801319197245448>");
            if (question == null)
            {
                await Context.Channel.SendMessageAsync(Language(62));
                return;
            }
            string[] answers;
            switch (Context.Guild.VoiceRegionId)
            {
                case "russia": answers = Languages.Russian.asks; break;
                default: answers = Languages.English.asks; break;
            }
            Random rand = new Random();
            int i = rand.Next(0, answers.Length);
            await Context.Channel.SendMessageAsync(Context.User.Mention + " " + answers[i]);
        }

        #endregion

        #region AdvancedRP

        [Command("skills")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddSkills(string name, [Remainder] string skill)
        {
            string[] words = skill.Split(new char[] { '|',',' });
            if (words.Length != 6) 
                return;

            SqliteCommand command = new SqliteCommand(Queries.AddSkills(Context.Guild.Id, name, skill), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();
            await Context.Channel.SendMessageAsync("Навыки распределены");
        }

        [Command("talent")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddTalent(string name, [Remainder] string skill)
        {
            string[] words = skill.Split(new char[] { '|' });
            if (words.Length != 2) 
                return;
            SqliteCommand command = new SqliteCommand(Queries.AddTalent(Context.Guild.Id, name, skill), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.SendMessageAsync("Талант распределен");
        }

        class Stats
        {
            internal string name { get; set; }
            internal string stats { get; set; }
            internal string talent { get; set; }
            internal string hp { get; set; }
            internal int currentHp { get; set; }
            internal string image { get; set; }
        };

        [Command("stats")]
        public async Task GetStats(string name)
        {
            Stats stats = new Stats();

            SqliteCommand getHero = new SqliteCommand(Queries.GetCharacter(Context.Guild.Id, name), Program.sqliteConnection);

            using (SqliteDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        stats = new Stats
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            stats = Convert.ToString(record["CHAR_SKILLS"]),
                            talent = Convert.ToString(record["CHAR_TALENTS"]),
                            hp = Convert.ToString(record["CHAR_HP"]),
                            currentHp = Convert.ToInt32(record["CHAR_CURHP"]),
                            image = Convert.ToString(record["CHAR_IMAGE"])
                        };
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Language(19));
                    return;
                }
            }

            string[] words = stats.stats.Split(new char[] { '|',',' });
            if (words.Length != 6)
                return;

            string[] talent = stats.talent.Split(new char[] { '|', ',' });
            if (talent.Length != 2)
                return;

            string rs = "<:red:650103765042593812>";
            string gs = "<:green:650103747904667648>";

            string res = "";

            int HP = Convert.ToInt32(stats.hp[2].ToString());

            for (int i = 0; i < stats.currentHp; i++)
            {
                res += gs;
            }
            for (int i = 0; i < HP - stats.currentHp; i++)
            {
                res += rs;
            }

            string a1 = $"Урон Основным оружием - {stats.hp[0]}";
            string a2 = "";

            if (Convert.ToInt32(stats.hp[1].ToString()) > 0)
                a2 = $"Урон Вспомогательным оружием - { stats.hp[1]}";

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle(stats.name)
                .WithDescription($"Здоровье - {res}\n\n{a1}\n{a2}")
                .WithColor(Color.DarkBlue)
                .WithThumbnailUrl(stats.image);

                builder.AddField(x =>
                {
                    x.Name = $"**{talent[0]}**";
                    x.Value = talent[1];
                });

            for(int i = 0; i < 6; i+=2)
            {
                builder.AddField(x =>
                {
                    x.Name = $"**{words[i]}**";
                    x.Value = words[i+1];
                    x.IsInline = true;
                });
            }

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        public static Task NextPage(string emojie, int state)
        {
            switch (state)
            {
                case 0:
                    switch (emojie)
                    {
                        case "0️⃣":
                            Global.CharacterSetter.Item3 += 0; break;
                        case "1️⃣":
                            Global.CharacterSetter.Item3 += 1; break;
                        case "2️⃣":
                            Global.CharacterSetter.Item3 += 2; break;
                        case "3️⃣":
                            Global.CharacterSetter.Item3 += 3; break;
                        default:
                            return Task.CompletedTask;
                    }
                    Global.CharacterSetter.Item1.RemoveAllReactionsAsync();

                    Global.CharacterSetter.Item2++;

                    Global.CharacterSetter.Item1.ModifyAsync(message =>
                    {
                        EmbedBuilder builder = new EmbedBuilder////
                        {
                            Title = Global.CharacterSetter.Item4,
                            Color = Color.Default,
                            Description =
                            "Выберите второстепенное оружие персонажа\n" +
                            "0 - отсутствует\n" +
                            "1 - легкое\n" +
                            "2 - среднее\n"
                        };
                        message.Embed = builder.Build();
                    });
                    Global.CharacterSetter.Item1.UpdateAsync();

                    Emoji[] emojis = { new Emoji("0️⃣"), new Emoji("1️⃣"), new Emoji("2️⃣")};
                    Global.CharacterSetter.Item1.AddReactionsAsync(emojis);
                    break;
                case 1:
                    switch (emojie)
                    {
                        case "0️⃣":
                            Global.CharacterSetter.Item3 += 0; break;
                        case "1️⃣":
                            Global.CharacterSetter.Item3 += 1; break;
                        case "2️⃣":
                            Global.CharacterSetter.Item3 += 2; break;
                        default:
                            return Task.CompletedTask;
                    }
                    Global.CharacterSetter.Item1.RemoveAllReactionsAsync();

                    Global.CharacterSetter.Item2++;

                    Global.CharacterSetter.Item1.ModifyAsync(message =>
                    {
                        EmbedBuilder builder = new EmbedBuilder
                        {
                            Title = Global.CharacterSetter.Item4,
                            Color = Color.Default,
                            Description =
                                "Выберите количество здоровья персонажа\n"
                        };
                        message.Embed = builder.Build();
                    });
                    Global.CharacterSetter.Item1.UpdateAsync();

                    Emoji[] emojis1 = { new Emoji("3️⃣"), new Emoji("5️⃣"), new Emoji("7️⃣") };
                    Global.CharacterSetter.Item1.AddReactionsAsync(emojis1);
                    break;

                case 2:
                    switch (emojie)
                    {
                        case "3️⃣":
                            Global.CharacterSetter.Item3 += 3;
                            break;
                        case "5️⃣":
                            Global.CharacterSetter.Item3 += 5;
                            break;
                        case "7️⃣":
                            Global.CharacterSetter.Item3 += 7;
                            break;
                        default:
                            return Task.CompletedTask;
                    }
                    Global.CharacterSetter.Item1.RemoveAllReactionsAsync();
                    Global.CharacterSetter.Item2++;

                    SqliteCommand command = new SqliteCommand(Queries.AddHP(Global.CharacterSetter.Item5, Global.CharacterSetter.Item4, Global.CharacterSetter.Item3, Convert.ToString(Global.CharacterSetter.Item3.Last())), Program.sqliteConnection);
                    command.ExecuteNonQuery();

                    Global.CharacterSetter.Item1.ModifyAsync(message =>
                    {
                        EmbedBuilder builder = new EmbedBuilder
                        {
                            Title = Global.CharacterSetter.Item4,
                            Color = Color.Default,
                            Description =
                                "Здоровье и Оружие персонажа установлены\nНе забудьте установить навыки и талант"
                        };
                        message.Embed = builder.Build();
                    });
                    Global.CharacterSetter.Item1.UpdateAsync();
                    Global.CharacterSetter = (null, 0, null, null, 0);
                    break;
            }
            return Task.CompletedTask;
        }

        [Command("damage")]
        public async Task Damage(string name, int damage)
        {
            await Check();
            Hero character = new Hero();
            int curHP = 0;
            int maxHP = 0;

            SqliteCommand getHero = new SqliteCommand(Queries.GetCharacter(Context.Guild.Id, name), Program.sqliteConnection);

            using (SqliteDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        character = new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            owner = Convert.ToUInt64(record["CHAR_OWNER"]),
                        };
                        curHP = Convert.ToInt32(record["CHAR_CURHP"]);
                        maxHP = Convert.ToInt32(Convert.ToString(record["CHAR_HP"])[2].ToString());
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Language(19));
                    return;
                }
            }

            curHP -= damage;

            if (damage > 0)
            {
                if (curHP <= 0)
                {
                    curHP = 0;
                    await Context.Channel.SendMessageAsync("> Персонаж тяжело ранен и не может далее вести бой");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"> Персонаж потерял {damage} единиц здоровья");
                }
            }
            else
            {
                if (curHP > maxHP)
                {
                    curHP = maxHP;
                    await Context.Channel.SendMessageAsync("> Персонаж полностью излечен");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"> Персонаж излечил {damage * -1} единиц здоровья");
                }
            }

            SqliteCommand command = new SqliteCommand(Queries.DamageChar(Context.Guild.Id, name, curHP), Program.sqliteConnection);
            await command.ExecuteNonQueryAsync();
        }

        [Command("heal")]
        public async Task Heal(string name, int damage)
        {
            await Damage(name, damage * -1);
        }
        #endregion
    }
}
