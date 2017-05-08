using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ZeldaTimelineGeneratorUtility
{
    /// <summary>
    /// Interaction logic for RootNodeChoser.xaml
    /// </summary>
    public partial class RootNodeChoser : Window
    {
        public ZeldaTreeNode targetGame;

        public ObservableCollection<string> Titles
        {
            get; set;
        } = new ObservableCollection<string>();

        public ObservableCollection<ZeldaTreeNode> Games
        {
            get; set;
        }

        public RootNodeChoser()
        {
            InitializeComponent();
        }

        public RootNodeChoser(ObservableCollection<ZeldaTreeNode> gameCollection)
        {
            InitializeComponent();

            Games = gameCollection;
            select_button.IsEnabled = false;

            foreach(var game in Games)
            {
                Titles.Add(game.GameTitle.ToString());
            }

            DataContext = this;
            dataGrid.ItemsSource = Titles;
            dataGrid.Items.Refresh();
        }

        private void select_button_Click(object sender, RoutedEventArgs e)
        {
            targetGame = Games.FirstOrDefault(game => game.GameTitle.ToString() == ((string)dataGrid.SelectedItem));
            targetGame.IsRootNode = true;
            this.Close();
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dataGrid.SelectedItems.Count == 1 && dataGrid.SelectedItem.GetType().Equals(typeof(string)))
            {
                select_button.IsEnabled = true;
            }
            else
            {
                select_button.IsEnabled = false;
            }
        }
    }
}
