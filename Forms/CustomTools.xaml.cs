using DiscordUserTools;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordUserTools.Forms
{
    /// <summary>
    /// Логика взаимодействия для CustomTools.xaml
    /// </summary>
    public partial class CustomTools : UserControl
    {
        public CustomTools()
        {
            InitializeComponent();
        }

        static bool autoEmbedderEnabled = false;
        static byte autoEmbedderMode;

        private void EnableAutoEmbedder(object sender, RoutedEventArgs e)
        {
            autoEmbedderEnabled = true;
        }

        private void DisableAutoEmbedder(object sender, RoutedEventArgs e)
        {
            autoEmbedderEnabled = false;
        }

        public static async Task onMsgCreated(MessageCreateEventArgs args)
        {
            if (!autoEmbedderEnabled) return;
            if (args.Author != MainWindow.userClient.CurrentUser) return;

            DiscordMessage msg = args.Message;
            if (msg.Content == null) return;
            if (msg.Content.Length < 1) return;

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

            embedBuilder.Title = $"Сообщение в {DateTime.Now.ToString()} от {(msg.Channel.IsPrivate ? msg.Author.Username : (((DiscordMember)msg.Author).Nickname!=null ? ((DiscordMember)msg.Author).Nickname : msg.Author.Username))}";
            embedBuilder.Description = msg.Content;
            if (msg.Attachments != null && msg.Attachments.Count > 0)
            {
                DiscordAttachment img = msg.Attachments[0];
                if(img.FileName.EndsWith(".jpg",true,CultureInfo.InvariantCulture)
                || img.FileName.EndsWith(".png", true, CultureInfo.InvariantCulture)
                || img.FileName.EndsWith(".bmp", true, CultureInfo.InvariantCulture))
                {
                    string url = img.Url;
                    embedBuilder.ImageUrl = url;
                }
                else
                {
                    string url = img.Url;
                    embedBuilder.Url = url;
                }
            }

            Random random = new Random();
            DiscordColor randColor;
            switch (random.Next(0, 7))
            {
                case 0: randColor = DiscordColor.Red; break;
                case 1: randColor = DiscordColor.Green; break;
                case 2: randColor = DiscordColor.Blue; break;
                case 3: randColor = DiscordColor.Yellow; break;
                case 4: randColor = DiscordColor.Cyan; break;
                case 5: randColor = DiscordColor.Violet; break;
                case 6: randColor = DiscordColor.Azure; break;
                case 7: randColor = DiscordColor.Blurple; break;
                default: randColor = new DiscordColor(); break;
            }
            embedBuilder.Color = randColor;
            embedBuilder.ThumbnailUrl = msg.Author.AvatarUrl;

            if(autoEmbedderMode == 0)
                await msg.ModifyAsync(content: "", embed: embedBuilder.Build());
            else
            {
                await msg.Channel.SendMessageAsync(embed: embedBuilder.Build());
                await msg.Channel.DeleteMessageAsync(msg);
            }
        }

        private void SetAutoEmbedderMode1(object sender, RoutedEventArgs e)
        {
            autoEmbedderMode = 0;
        }

        private void SetAutoEmbedderMode2(object sender, RoutedEventArgs e)
        {
            autoEmbedderMode = 1;
        }
    }
}
