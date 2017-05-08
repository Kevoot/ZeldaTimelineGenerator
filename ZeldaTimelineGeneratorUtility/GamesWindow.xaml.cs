using System;
using System.Data;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;

namespace ZeldaTimelineGeneratorUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Game> GameObservableCollection
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                GameObservableCollection = 
                    JsonConvert.DeserializeObject<ObservableCollection<Game>>(File.ReadAllText(Environment.CurrentDirectory + "\\LocalData.txt"));
            }
            catch(JsonReaderException)
            {
                MessageBox.Show("Could not read LocalData.txt");
                GameObservableCollection = new ObservableCollection<Game>();
            }
            catch (Exception e)
            {
                MessageBox.Show("Couldn't load local JSON file");
                GameObservableCollection = new ObservableCollection<Game>();
            }

            dataGrid.ItemsSource = GameObservableCollection;
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            var pendingDeletions = new List<Game>();
            if (dataGrid.SelectedItems.Count > 0)
            {
                foreach(var game in dataGrid.SelectedItems)
                {
                    pendingDeletions.Add(((Game)game));
                }
            }
            foreach(var game in pendingDeletions)
            {
                GameObservableCollection.Remove(game);
            }
        }

        private void generate_button_Click(object sender, RoutedEventArgs e)
        {
            var tree = new ZeldaTree(GameObservableCollection);
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if(dataGrid.SelectedItems.Count.Equals(1))
            {
                var selectedEntry = dataGrid.SelectedItem;
                if (selectedEntry is Game && ((Game)selectedEntry).GameId == 0)
                {
                    ((Game)selectedEntry).GameId = GameObservableCollection.Count;
                    
                }
            }
        }

        private void edit_directConnections_button_click(object sender, RoutedEventArgs e)
        {
            BindingExpression expression = ((BindingExpression)((Button)sender).TemplatedParent.ReadLocalValue(Button.ContentProperty));
            var selectedGame = expression.DataItem;
            if(selectedGame != null)
            {
                var directConnectionsWindow = new ConnectionsWindow(((Game)selectedGame));
                directConnectionsWindow.ShowDialog();
            }
        }

        private void edit_exclusions_button_click(object sender, RoutedEventArgs e)
        {
            BindingExpression expression = ((BindingExpression)((Button)sender).TemplatedParent.ReadLocalValue(Button.ContentProperty));
            var selectedGame = expression.DataItem;
            if (selectedGame != null)
            {
                var exclusionsWindow = new ExclusionsWindow(((Game)selectedGame));
                exclusionsWindow.ShowDialog();
            }
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string json = JsonConvert.SerializeObject(GameObservableCollection.ToArray());
                File.WriteAllText(Environment.CurrentDirectory + "\\LocalData.txt", json);
                MessageBox.Show("Saved Data to ../LocalData.txt");
            }
            catch(JsonException je)
            {
                MessageBox.Show("Couldn't save changes to LocalData.txt \n" + je.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Couldn't save changes to LocalData.txt \n" + ex.Message);
            }
        }
    }
}
