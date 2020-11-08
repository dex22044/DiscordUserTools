using DiscordUserTools.Forms;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DiscordUserTools
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DiscordClient userClient;

        public static event AsyncEventHandler<GuildCreateEventArgs> DiscordGuildAvailable;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ToggleConnection(object sender, RoutedEventArgs e)
        {
            if (userClient == null)
            {
                userClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = TokenBox.Text,
                    TokenType = TokenType.User
                });

                try
                {
                    await userClient.ConnectAsync();
                    ((Button)sender).Content = "Отключиться";
                    StatusEllipse.Fill = Brushes.Green;
                    StatusLable.Content = "Подключен";

                    userClient.MessageCreated += CustomTools.onMsgCreated;
                }
                catch (Exception) {
                    StatusEllipse.Fill = Brushes.Blue;
                    StatusLable.Content = "Ошибка подключения";
                    userClient = null;
                }
            }
            else
            {
                try
                {
                    await userClient.DisconnectAsync();
                    ((Button)sender).Content = "Подключиться";
                    StatusEllipse.Fill = Brushes.Red;
                    StatusLable.Content = "Не подключен";

                    userClient.GuildAvailable -= DiscordGuildAvailable;
                    userClient = null;
                }
                catch (Exception)
                {
                    StatusEllipse.Fill = Brushes.Blue;
                    StatusLable.Content = "Ошибка отключения";
                }
            }
        }

        
    }
}
