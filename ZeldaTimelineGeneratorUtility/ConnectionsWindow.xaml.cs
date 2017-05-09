using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZeldaTimelineGeneratorUtility
{
    /// <summary>
    /// Interaction logic for ConnectionsWindow.xaml
    /// </summary>
    public partial class ConnectionsWindow : Window
    {
        private Game selectedGame;
        private Game backupGame;

        public Game SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                selectedGame = value;
            }
        }

        public Game BackupGame
        {
            get
            {
                return backupGame;
            }
            set
            {
                backupGame = value;
            }
        }

        public ConnectionsWindow()
        {
            InitializeComponent();
        }

        public ConnectionsWindow(Game game)
        {
            SelectedGame = game;
            CreateBackupForReversion();

            InitializeComponent();
            dataGrid.ItemsSource = SelectedGame.DirectConnections;

            dataGrid.Items.Refresh();
        }

        private void CreateBackupForReversion()
        {
            backupGame = new Game();
            foreach (var con in SelectedGame.DirectConnections)
            {
                BackupGame.DirectConnections.Add(con);
            }
            foreach (var exc in SelectedGame.Exclusions)
            {
                BackupGame.Exclusions.Add(exc);
            }
        }

        private void dataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new DirectConnection(SelectedGame.GameId, SelectedGame.GameTitle);
        }
    }
}
