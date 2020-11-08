using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordUserTools.Forms
{
    /// <summary>
    /// Логика взаимодействия для AutoSpammer.xaml
    /// </summary>
    public partial class AutoSpammer : UserControl
    {
        public AutoSpammer()
        {
            InitializeComponent();
        }

        Timer spamTimer;

        string msg;
        DiscordChannel targChannel;

        private async void StartStopSpamming(object sender, RoutedEventArgs e)
        {
            Button senderBtn = (Button)sender;
            if (spamTimer == null)
            {
                try
                {
                    if (IsDirect.IsChecked ?? false)
                    {
                        DiscordGuild server;
                        ulong serverId = Convert.ToUInt64(GuildId.Text);
                        if (MainWindow.userClient.Guilds.TryGetValue(serverId, out server))
                        {
                            ulong channelIdVal = Convert.ToUInt64(ChannelId.Text);
                            try
                            {
                                DiscordMember dmUser = await server.GetMemberAsync(channelIdVal);
                                DiscordDmChannel dm = await dmUser.CreateDmChannelAsync();
                                if (dm != null)
                                {
                                    targChannel = dm;
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка: неверный ID пользователя.");
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка: неверный ID пользователя.");
                            }
                        }
                    }
                    else
                    {
                        DiscordGuild server;
                        ulong serverId = Convert.ToUInt64(GuildId.Text);
                        if (MainWindow.userClient.Guilds.TryGetValue(serverId, out server))
                        {
                            ulong channelIdVal = Convert.ToUInt64(ChannelId.Text);
                            try
                            {
                                DiscordChannel channel = server.GetChannel(channelIdVal);
                                if (channel != null)
                                {
                                    targChannel = channel;
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка: неверный ID канала.");
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка: неверный ID канала.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка: неверный ID сервера.");
                        }
                    }
                }
                catch (Exception) { return; }

                int interval = 0;

                if (!int.TryParse(SpamPeriod.Text, out interval)) return;

                msg = SpamContent.Text;
                spamTimer = new Timer()
                {
                    Interval = interval
                };
                spamTimer.Elapsed += sendMsg;
                spamTimer.Start();
                senderBtn.Content = "Стоп";
            }
            else
            {
                spamTimer.Stop();
                spamTimer = null;
                senderBtn.Content = "Старт";
            }
        }

        void sendMsg(object s, EventArgs e)
        {
            targChannel.SendMessageAsync(msg);
        }
    }
}
