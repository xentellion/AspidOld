using Discord;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Timers;

namespace Aspid.Modules
{
    internal static class Repeater
    {
        private static Timer looper;

        const int hour1 = 21;
        const int hour2 = 9;
        
        const int minute = 0;
        
        static bool canSend = true;

        static NpgsqlCommand dePunish;
        static NpgsqlCommand deMute;

        internal static Task Muted()
        {
            looper = new Timer
            {
                Interval = 60000,
                AutoReset = true,
                Enabled = true,
                
            };
            looper.Elapsed += DecreaseMute;
            looper.Elapsed += CheckTime;

            return Task.CompletedTask;
        }

        private static void CheckTime(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.UtcNow.Hour == hour1 || DateTime.UtcNow.Hour == hour2) && DateTime.UtcNow.Minute == minute && canSend)
            {
                canSend = false;
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Напоминание").WithColor(Color.Red)
                    .WithImageUrl("https://media.discordapp.net/attachments/614108079545647105/614108112730718249/primal_aspid.jpg?width=676&height=474");
                Global.channel.SendMessageAsync("", false, builder.Build());
            }
            if (!canSend && (DateTime.UtcNow.Hour == hour1 + 1 || DateTime.UtcNow.Hour == hour2 + 1)) canSend = true;
        }

        private static async void DecreaseMute(object sender, ElapsedEventArgs e)
        {
            foreach (Discord.WebSocket.SocketGuild guild in Program._client.Guilds)
            {
                NpgsqlCommand reduceMute = new NpgsqlCommand(Queries.PenaltyHandler(guild.Id), Program.npgSqlConnection);
                reduceMute.ExecuteNonQuery();

                dePunish = new NpgsqlCommand(Queries.GetPunished(guild.Id), Program.npgSqlConnection);
                deMute = new NpgsqlCommand(Queries.GetMuted(guild.Id), Program.npgSqlConnection);

                List<ulong> DePunished = new List<ulong>();
                List<ulong> DeMuted = new List<ulong>();

                using (NpgsqlDataReader reader = dePunish.ExecuteReader())
                {
                    foreach(DbDataRecord record in reader)
                    {
                        ulong a = Convert.ToUInt64(record["ID"]);
                        DePunished.Add(a);
                        await Ping.RemoveMute(guild.Id, a, "Punished");
                    }
                    reader.Close();
                }

                if(DePunished.Count > 0)
                {
                    NpgsqlCommand dePun;
                    foreach(ulong unit in DePunished)
                    {
                        dePun = new NpgsqlCommand(Queries.RemovePunish(guild.Id, unit), Program.npgSqlConnection);
                        dePun.ExecuteNonQuery();
                    }
                }

                using (NpgsqlDataReader reader = deMute.ExecuteReader())
                {
                    foreach (DbDataRecord record in reader)
                    {
                        ulong a = Convert.ToUInt64(record["ID"]);
                        DeMuted.Add(a);
                        await Ping.RemoveMute(guild.Id, a, "Muted");
                    }
                    reader.Close();
                }

                if (DeMuted.Count > 0)
                {
                    NpgsqlCommand dePun;
                    foreach (ulong unit in DeMuted)
                    {
                        dePun = new NpgsqlCommand(Queries.RemoveMute(guild.Id, unit), Program.npgSqlConnection);
                        dePun.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
