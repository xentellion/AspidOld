using System;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Discord.Rest;
using System.Threading;
using System.Collections.Generic;
using Npgsql;
using System.Data.Common;

namespace Aspid.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        public static int counter = 0;

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

        //await Context.Channel.SendMessageAsync(Context.Guild.VoiceRegionId);

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
                await Context.Channel.SendMessageAsync("Таким хроническим самоубийцам нельзя давать в руки пистолет", false);
                return;
            }

            if (username.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync("Мертвецы не играют с удачей", false);
                return;
            }

            Thread.Sleep(1000); await Context.Channel.SendMessageAsync("Вы взяли в руки револьвер", false);
            Thread.Sleep(2000); await Context.Channel.SendMessageAsync("Вы проверили барабан и прокрутили его", false);
            Thread.Sleep(2000); await Context.Channel.SendMessageAsync("Вы приложили револьвер к виску...", false);
            Thread.Sleep(4000);

            Random rand = new Random();
            int i = rand.Next(0, 6);
            if (i == 0)
            {
                counter++;
                Config.bot.deadPeople++;
                SocketUser user = Context.User;
                await Context.Channel.SendMessageAsync($"Раздался выстрел и тело {user.Mention} с грохотом упало на пол", false);
                await (user as IGuildUser).AddRoleAsync(role);
            }
            else await Context.Channel.SendMessageAsync("Осечка! Передайте пистолет другому. Пусть он тоже испытает удачу.", false);
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
                await MissMessage("*ОТ АСПИДОВ ТАК ПРОСТО НЕ ИЗБАВИТЬСЯ*"); breaker = true;
            }
            if (message == Context.User || message == null)
            {
                await MissMessage("Стреляться вздумали? Не в мою смену"); breaker = true;
            }
            else if ((Context.User as SocketGuildUser).Roles.Contains(role1))
            {
                await MissMessage("Мертвым пушки не предлагать"); breaker = true;
            }
            else if ((Context.User as SocketGuildUser).Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished")))
            {
                await MissMessage("Таким мерзавцам нельзя давать в руки пистолет"); breaker = true;
            }
            else if (message.Roles.Contains(role1))
            {
                await MissMessage("Вы стреляете в мертвое тело. Как будто убийства было недостаточно"); breaker = true;
            }

            if (breaker) return;

            Random rand = new Random();
            int i = rand.Next(0, 2);
            if (i == 0)
            {
                counter++;
                Config.bot.deadPeople++;
                await Context.Channel.SendMessageAsync($"Вы застрелили {message.Mention}", false);
                await (message as IGuildUser).AddRoleAsync(role1);
            }
            else
            {
                await MissMessage("Промах! Какая досада <:PKHeh:575051447906074634>");
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
                }
            }
            await Context.Channel.SendMessageAsync($">>> {counter} человек было мертво\n\nВсего в мире умирали {Config.bot.deadPeople} раз \n", false);
            counter = 0;
        }


        [Command("punish")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Punish(SocketGuildUser user)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Punished");

            NpgsqlCommand command = new NpgsqlCommand(Queries.AddPunish(Context.Guild.Id, user.Id), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await (user as IGuildUser).AddRoleAsync(role);

            await Context.Channel.SendMessageAsync($"Внезапно ворвавшиеся в комнату кингсмолды отбирают у {user.Mention} револьвер, попутно наблюдая как он самостоятельно падает на их ноги, когти и мебель в комнате, после чего уходят", false);
        }

        #endregion

        #region Heroes

        [Command("hero")]
        public async Task GetCharacter(string name)
        {
            await Check();
            Hero character = new Hero();

            NpgsqlCommand getHero = new NpgsqlCommand(Queries.GetCharacter(Context.Guild.Id, name), Program.npgSqlConnection);

            using (NpgsqlDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        character = new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            owner = Convert.ToUInt64(record["CHAR_OWNER"]),
                            description = Convert.ToString(record["DESCRIPTION"])
                        };
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Такого персонажа нет");
                    return;
                }
            }

            EmbedBuilder builder = new EmbedBuilder();
            SocketUser user = Context.Guild.GetUser(character.owner);
            builder
                .WithColor(Color.DarkBlue)
                .WithTitle(character.name)
                .WithDescription(character.description + "\n\n Автор - " + user.Mention);
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

            NpgsqlCommand getHero = new NpgsqlCommand(Queries.GetCharacter(Context.Guild.Id, user.Id), Program.npgSqlConnection);

            using (NpgsqlDataReader reader = getHero.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foreach (DbDataRecord record in reader)
                    {
                        characters.Add(new Hero
                        {
                            name = Convert.ToString(record["CHAR_NAME"]),
                            owner = Convert.ToUInt64(record["CHAR_OWNER"]),
                            description = Convert.ToString(record["DESCRIPTION"])
                        });
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("У пользователя нет персонажей");
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
                    .WithDescription(characters[0].description + "\n\n Автор - " + user.Mention);
                if (characters[0].image != null)
                {
                    builder.WithImageUrl(characters[0].image);
                }
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else
            {
                string list = "";
                characters.OrderBy(x => x.name).ToList();
                foreach(Hero hero in characters)
                {
                    list += hero.name;
                    list += "\n";
                }
                EmbedBuilder builder = new EmbedBuilder();
                builder
                    .WithDescription(list + $"\n\n{user.Mention}")
                    .WithColor(Color.Blue)
                    .WithTitle($"**Персонажи пользователя**");
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
        }

        [Command("add")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetCharacter(string name, SocketUser owner, [Remainder] string info)
        {
            NpgsqlCommand command = new NpgsqlCommand(Queries.AddChar(Context.Guild.Id, name, owner.Id, info), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync($"Персонаж ***{name}*** добавлен");
        }

        [Command("delete")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteCharacter(string name)
        {
            NpgsqlCommand command = new NpgsqlCommand(Queries.DeleteChar(Context.Guild.Id, name), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync($"Персонаж ***{name}*** удален");
        }

        [Command("update")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UpdateCharacter(string name, [Remainder] string info)
        {
            NpgsqlCommand command = new NpgsqlCommand(Queries.ChangeDescription(Context.Guild.Id, name, info), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync($"Персонаж ***{name}*** обновлен");
        }

        [Command("image")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PicCharacter(string name, string info)
        {
            NpgsqlCommand command = new NpgsqlCommand(Queries.ChangeImage(Context.Guild.Id, name, info), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync($"Персонаж ***{name}*** обновлен");
        }

        [Command("heroes")]
        public async Task AllBois()
        {
            await Check();

            List<Hero> characters = new List<Hero>();

            NpgsqlCommand getHero = new NpgsqlCommand(Queries.GetAllCharacters(Context.Guild.Id), Program.npgSqlConnection);

            using (NpgsqlDataReader reader = getHero.ExecuteReader())
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
                    await Context.Channel.SendMessageAsync("На сервере нет персонажей");
                    return;
                }
            }

            string list = "";
            characters.OrderBy(x => x.name).ToList();

            foreach (Hero hero in characters)
            {
                list += hero.name;
                list += "\n";
            }

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithDescription(list)
                .WithColor(Color.Blue)
                .WithTitle($"**Персонажи**");
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("roll")]
        public async Task RollDice(string input = "20+0")
        {
            await Check();

            string a;
            int num;

            string output = "20";
            char action = '+';
            string added = "0";

            bool secPart = false;

            for (int j = 0; j < input.Length; j++)
            {
                a = input[j].ToString();

                bool isNum = int.TryParse(a, out num);
                if (!isNum)
                {
                    secPart = true;
                }

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

            int message = Convert.ToInt32(output);
            int addValue = Convert.ToInt32(added);

            if (message > 100 || addValue > 100)
            {
                await Context.Channel.SendMessageAsync("Ввод не должен превышать 100"); return;
            }

            if (message < 2)
            {
                await Context.Channel.SendMessageAsync("Ввод должен быть 2 или больше"); return;
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
                .WithDescription(Context.User.Mention + "\n \n Вам выпало `" + i.ToString() + "`")
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
            if (Global.HelpHandler != null)
            {
                try
                {
                    await Global.HelpHandler.DeleteAsync();
                }
                catch { }
            }
            Fields.curren = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder
                .WithTitle("***КОМАНДЫ БОТА***")
                .WithColor(Color.DarkBlue)
                .AddField(Fields.FielderTitle[Fields.curren], Fields.Fielder[Fields.curren])
                .WithFooter("Стр. " + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitle.Length.ToString());
                
            RestUserMessage a = await Context.Channel.SendMessageAsync("", false, builder.Build());

            await a.AddReactionAsync(new Emoji("◀"));
            await a.AddReactionAsync(new Emoji("▶"));
            Global.HelpHandler = a;
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
            else if (side == "▶" && Fields.curren < Fields.FielderTitle.Length - 1)
            {
                Fields.curren++;
                changed = true;
            }

            if (changed)
            {
                Global.HelpHandler.ModifyAsync(a =>
                {
                    EmbedBuilder bruh = new EmbedBuilder()
                    .WithTitle("***КОМАНДЫ БОТА***")
                    .WithColor(Color.DarkBlue)
                    .AddField(Fields.FielderTitle[Fields.curren], Fields.Fielder[Fields.curren])
                    .WithFooter("Стр. " + (Fields.curren + 1).ToString() + "/" + Fields.FielderTitle.Length.ToString());
                    a.Embed = bruh.Build();
                });
                Global.HelpHandler.UpdateAsync();
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
            builder.WithTitle("Используйте ссылку для добавления бота на свои сервера")
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
                .WithTitle("**ГОЛОСОВАНИЕ**")
                .WithDescription(
                    message + "\n\n" +
                    "<:THK_Good:575051447599628311> - если вы за \n" +
                    "<:THK_Bad:575796719078473738> - если вы против")
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
                .WithTitle("**О БОТЕ**")
                .WithDescription("**Primal Aspid** - бот для мессенджера **Discord** на языке **C#** на платформе **.NET Core** [с открытым исходным кодом](https://github.com/xentellion/AspidBot)")
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
                .WithTitle("**ИНФОРМАЦИЯ О СЕРВЕРЕ**")
                .WithDescription(
                $"Сервер **{guild.Name }** создан {guild.CreatedAt.ToLocalTime()}\n\n" +
                $"Всего на сервере **{guild.Users.Count}** пользователей, **{guild.Roles.Count}** ролей и **{guild.Channels.Count}** каналов\n\n"
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
                    $"Сервер **{guild.Name}** создан {guild.CreatedAt.ToLocalTime()}\n\n" +
                    $"Всего на сервере **{guild.Users.Count}** пользователей, **{guild.Roles.Count}** ролей и **{guild.Channels.Count}** каналов\n\n"
                    )
                    .WithCurrentTimestamp()
                    .WithThumbnailUrl(guild.IconUrl)
                    .WithColor(Color.DarkPurple);
                await Context.Channel.SendMessageAsync("", false, builder.Build());
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
            await Context.Channel.SendMessageAsync("Бот отключен");
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
            await Context.Channel.SendMessageAsync("Бот включен");
        }

        [Command("break")]
        public async Task StopBot()
        {
            if (Context.User.Id != 264811248552640522)
            {
                await Context.Message.DeleteAsync();
                return;
            }
            await Resurrect();
            try
            {
                await Global.HelpHandler.DeleteAsync();
            }
            catch { }
            await Context.Channel.SendMessageAsync("", false,
                new EmbedBuilder()
                .WithTitle("Бот отключен по техническим причинам")
                .WithColor(Color.DarkGreen)
                .WithImageUrl("https://media.discordapp.net/attachments/603600328117583874/615150516388757509/image0-5.png")
                .Build());//Pic("Бот отключен по техническим причинам").Build());
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
                case 'm': longitud = "минут"; break;
                case 'h': muteTime *= 60; longitud = "часов"; break;
                case 'd': muteTime *= 1440; longitud = "дней"; break;
                case 'w': muteTime *= 10080; longitud = "недель"; break;
                case 'y': muteTime *= 3679200; longitud = "лет"; break;
                default: return;
            }

            await Context.Channel.SendMessageAsync($"🔇 {user.Mention} *получил(а) мут на {show} {longitud}*");

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");

            NpgsqlCommand command = new NpgsqlCommand(Queries.AddMute(Context.Guild.Id, user.Id, (ulong)muteTime), Program.npgSqlConnection);
            await command.ExecuteNonQueryAsync();

            await user.AddRoleAsync(role);

            if((Context.Channel as IGuildChannel).GetPermissionOverwrite(role) == null) 
                await Program.AddChannelRestr(Context.Guild);

            var ls = await user.GetOrCreateDMChannelAsync();
            EmbedBuilder builder = new EmbedBuilder();
            if (reason == null) reason = "не указанной администрацией";
            builder.WithTitle("ВНИМАНИЕ")
                .WithDescription($"Вы получили мут на сервере **{Context.Guild.Name}** на **{show} {longitud}** по причине {reason}")
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
            int i = rand.Next(0, Global.messages.Count());
            string grub = Global.messages.ElementAt(i).Attachments.First().Url;
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
            Global.messages = await (channel as ISocketMessageChannel).GetMessagesAsync().FlattenAsync();
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
                await Context.Channel.SendMessageAsync("Аспиды не дают мертвецам себя гладить");
                return;
            }
            string[] frase = {
            "Аспиду не понравилось ваше поглаживание и он плюнул в вас кислотой. Вас убило.",
            "\"С крыльями осторожнее\"",
            "\"А можно ещё?\"",
            "Аспиду захотелось ещё, чтобы его погладили",
            "Аспид отлетел от вас, ибо вы задели его крылья",
            "Аспиду понравилось поглаживание и он требует больше",
            "Аспид хочет, чтобы ему почесали спинку",
            "*Довольное шипение*"
            };
            Random rand = new Random();
            int i = rand.Next(0, frase.Length);
            if (i == 0)
            {
                counter++;
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
                await Context.Channel.SendMessageAsync("Вы и о чем не спросили");
                return;
            }
            string[] answers =
            {
                "Бесспорно",
                "Конечно",
                "Никаких сомнений",
                "Определённо да",
                "Можешь быть уверен в этом",
                "Я умру, если нет",
                "Мне кажется — «да»",
                "Общество аспидов говорит — «да»",
                "Вероятнее всего",
                "Хорошие перспективы",
                "Знаки говорят — «да»",
                "Да",
                "Я уверен, что да",
                "Вопрос не ясен, перефразируй",
                "Спроси позже",
                "Лучше не знать тебе ответ",
                "Ты хочешь, что бы я умер, отвечая на такой сложный вопрос?",
                "Cейчас нельзя сказать",
                "Сначала погладь, а потом спроси",
                "Не хочу отвечать",
                "Даже не думай",
                "Мой ответ — «нет»",
                "Общество аспидов говорит — «нет»",
                "Перспективы не очень хорошие",
                "Весьма сомнительно",
                "Нет",
                "Не зли меня этим плохим вопросом",
            };
            Random rand = new Random();
            int i = rand.Next(0, answers.Length);
            await Context.Channel.SendMessageAsync(Context.User.Mention + " " + answers[i]);
        }

        #endregion
    }
}
