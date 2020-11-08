using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для Embed_generator.xaml
    /// </summary>
    public partial class Embed_generator : UserControl
    {
        public Embed_generator()
        {
            InitializeComponent();
        }

        private void EmbGenAddParam(object sender, RoutedEventArgs e)
        {
            StackPanel newItem = new StackPanel();
            newItem.Orientation = Orientation.Horizontal;

            TextBox nameBox = new TextBox()
            {
                Width = 200,
                Height = 20
            };

            TextBox valueBox = new TextBox()
            {
                Width = 200,
                Height = 20
            };

            CheckBox inlineBox = new CheckBox()
            {
                Content = new TextBlock { Text = "Inline" }
            };

            Button deleteButton = new Button()
            {
                Width = 50,
                Content = new TextBlock { Text = "Удалить" }
            };
            deleteButton.Click += EmbGenDeleteParam;

            newItem.Children.Add(nameBox);
            newItem.Children.Add(valueBox);
            newItem.Children.Add(inlineBox);
            newItem.Children.Add(deleteButton);

            EmbGenParams.Items.Add(newItem);
        }

        private void EmbGenDeleteParam(object sender, RoutedEventArgs e)
        {
            Button senderBtn = (Button)sender;
            EmbGenParams.Items.Remove((StackPanel)senderBtn.Parent);
            //Console.WriteLine(((ListBoxItem)((StackPanel)senderBtn.Parent).Parent).Parent);
        }

        private async void SendEmbed(object sender, RoutedEventArgs e)
        {
            if (MainWindow.userClient != null)
            {
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

                embedBuilder.Title = EmbGenTitle.Text;

                embedBuilder.Description = EmbGenDescription.Text;

                Color col = EmbGenColor.SelectedColor ?? Colors.Black;
                embedBuilder.Color = new DiscordColor(col.R, col.G, col.B);

                embedBuilder.ImageUrl = EmbGenImgURL.Text;

                embedBuilder.ThumbnailUrl = EmbGenAvatarURL.Text;

                embedBuilder.Url = EmbGenTitleURL.Text;

                foreach (StackPanel panel in EmbGenParams.Items)
                {
                    TextBox nameBox = (TextBox)panel.Children[0];
                    TextBox valueBox = (TextBox)panel.Children[1];
                    CheckBox inlineBox = (CheckBox)panel.Children[2];

                    embedBuilder.AddField(nameBox.Text, valueBox.Text, inlineBox.IsChecked ?? false);
                }

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
                                MessageBoxResult result = MessageBox.Show($"Вы хотите отправить сообщение {dmUser.Username}. Продолжить?", "Ожидание подтверждения", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                                if (result == MessageBoxResult.No)
                                {
                                    MessageBox.Show("Отправка отменена.");
                                    return;
                                }
                                await dm.SendMessageAsync(embed: embedBuilder.Build());
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
                    else
                    {
                        MessageBox.Show("Ошибка: неверный ID сервера.");
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
                                MessageBoxResult result = MessageBox.Show($"Вы хотите отправить сообщение в канал {channel.Name} на сервере {server.Name}. Продолжить?", "Ожидание подтверждения", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                                if (result == MessageBoxResult.No)
                                {
                                    MessageBox.Show("Отправка отменена.");
                                    return;
                                }
                                await channel.SendMessageAsync(embed: embedBuilder.Build());
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
            else
            {
                MessageBox.Show("Ошибка: клиент не подключен.");
            }
        }
    }
}
