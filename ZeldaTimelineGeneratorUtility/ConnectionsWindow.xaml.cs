using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class ConnectionsWindow : Window, INotifyPropertyChanged
    {
        private Game selectedGame;
        private List<DirectConnection> backupConnections;
        public event PropertyChangedEventHandler PropertyChanged;
        private string gameTitle;
        List<DirectConnection> ValidationFailures = new List<DirectConnection>();

        public Game SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                if(selectedGame != value)
                {
                    selectedGame = value;
                    NotifyPropertyChanged("SelectedGame");
                }
            }
        }

        public List<DirectConnection> BackupConnections
        {
            get
            {
                return backupConnections;
            }
            set
            {
                if(backupConnections != value)
                {
                    backupConnections = value;
                    NotifyPropertyChanged("BackupGame");
                }
            }
        }

        public string GameTitle
        {
            get
            {
                return gameTitle;
            }
            set
            {
                if (gameTitle != value)
                {
                    gameTitle = value;
                    NotifyPropertyChanged("GameTitle");
                }
            }
        }

        public ConnectionsWindow()
        {
            InitializeComponent();
        }

        public ConnectionsWindow(Game game)
        {
            InitializeComponent();
            SelectedGame = game;
            GameTitle = GameEnumDescription.GetEnumDescription(SelectedGame.GameTitle);
            CreateBackupForReversion();
            dataGrid.ItemsSource = SelectedGame.DirectConnections;
            dataGrid.DataContext = this;
            dataGrid.Items.Refresh();
        }

        private void CreateBackupForReversion()
        {
            BackupConnections = SelectedGame.DirectConnections.ToList();
        }

        private void dataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new DirectConnection(SelectedGame.GameId, SelectedGame.GameTitle);
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var connection in SelectedGame.DirectConnections)
            {
                connection.SourceGame = SelectedGame.GameTitle;
            }
            Close();
        }

        private void revertButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedGame.DirectConnections = new ObservableCollection<DirectConnection>();
            foreach(var conn in BackupConnections)
            {
                SelectedGame.DirectConnections.Add(conn);
            }
            Close();
        }

        private void addNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedGame.DirectConnections.Add(new DirectConnection());
        }

        private void deleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem);

            if (row == null || row.Item == null) return;
            else if (row.Item.GetType() == typeof(DirectConnection))
            {
                var selectedConnection = SelectedGame.DirectConnections.FirstOrDefault(conn => conn == row.Item as DirectConnection);
                var faultyConnections = ValidationFailures.Where(connection => connection == selectedConnection);
                if (faultyConnections != null)
                {
                    foreach(var connection in faultyConnections.ToList())
                    {
                        ValidationFailures.Remove(connection);
                    }
                    if (ValidationFailures.Count < 1)
                    {
                        SetButtonStatus(true);
                    }
                    else
                    {
                        SetButtonStatus(false);
                    }
                }
                if (selectedConnection != null)
                {
                    SelectedGame.DirectConnections.Remove(selectedConnection);
                }
            }
            
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            DirectConnection connectionFault = new DirectConnection();

            if(sender.GetType() == typeof(TextBox))
            {
                connectionFault = ((DirectConnection)(((TextBox)sender).BindingGroup.Items[0]));
            }
            else if(sender.GetType() == typeof(ComboBox))
            {
                connectionFault = ((DirectConnection)(((ComboBox)sender).BindingGroup.Items[0]));
            }

            if (e.Action.Equals(ValidationErrorEventAction.Added))
            {
                ValidationFailures.Add(connectionFault);
            }
            else if (e.Action.Equals(ValidationErrorEventAction.Removed))
            {
                ValidationFailures.Remove(connectionFault);
            }

            if (ValidationFailures.Count() > 0) SetButtonStatus(false);
            else SetButtonStatus(true);
        }

        private void SetButtonStatus(bool status)
        {
            this.addNewRowButton.IsEnabled = status;
            this.saveButton.IsEnabled = status;
        }
    }
}
