using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Логика взаимодействия для AutoClearer.xaml
    /// </summary>
    public partial class AutoClearer : UserControl
    {
        public AutoClearer()
        {
            InitializeComponent();
        }

        List<DiscordMessage> toClear;

        private async void Clear(object sender, RoutedEventArgs e)
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
                                int msgCount;
                                ulong startClear;

                                if (!int.TryParse(ClearCount.Text, out msgCount)) MessageBox.Show("Неправильное кол-во ссобщений.");
                                if (!ulong.TryParse(ClearerMsgId.Text, out startClear)) MessageBox.Show("Неправильный ID сообщения.");

                                try
                                {
                                    toClear = (await dm.GetMessagesAsync(msgCount, startClear)).Where(ex => ex.Author == MainWindow.userClient.CurrentUser).ToList();

                                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить свои {toClear.Count} сообщений в DM с \"{dmUser.Username}\" начиная с сообщения \"{toClear.Last().Content}\" и заканчивая \"{toClear.First().Content}\"?", "Ожидание подтверждения", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                                    if (result == MessageBoxResult.No)
                                    {
                                        MessageBox.Show("Удаление отменено.");
                                        return;
                                    }

                                    foreach (DiscordMessage msg in toClear)
                                    {
                                        try
                                        {
                                            await dm.DeleteMessageAsync(msg);
                                            Thread.Sleep(1300);
                                        }
                                        catch (Exception) { Thread.Sleep(600); }
                                    }
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Какая-то ошибка.");
                                }
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
                                int msgCount;
                                ulong startClear;

                                if (!int.TryParse(ClearCount.Text, out msgCount)) MessageBox.Show("Неправильное кол-во ссобщений.");
                                if (!ulong.TryParse(ClearerMsgId.Text, out startClear)) MessageBox.Show("Неправильный ID сообщения.");

                                try
                                {
                                    if ((OnlyMine.IsChecked ?? false) == false)
                                        toClear = (await channel.GetMessagesAsync(msgCount, startClear)).ToList();
                                    else
                                        toClear = (await channel.GetMessagesAsync(msgCount, startClear)).Where(ex => ex.Author == MainWindow.userClient.CurrentUser).ToList();

                                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить {toClear.Count}{((OnlyMine.IsChecked ?? false)?" своих":"")} сообщений в канале \"{channel.Name}\" на сервере \"{server.Name}\" начиная с сообщения \"{toClear.Last().Content}\" и заканчивая \"{toClear.First().Content}\"?", "Ожидание подтверждения", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                                    if (result == MessageBoxResult.No)
                                    {
                                        MessageBox.Show("Удаление отменено.");
                                        return;
                                    }

                                    foreach (DiscordMessage msg in toClear)
                                    {
                                        try
                                        {
                                            await channel.DeleteMessageAsync(msg);
                                            Thread.Sleep(1300);
                                        }
                                        catch (Exception) { Thread.Sleep(600); }
                                    }
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Какая-то ошибка.");
                                }
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
        }
    }
}
