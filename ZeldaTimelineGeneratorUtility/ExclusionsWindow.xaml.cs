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
using System.Windows.Shapes;

namespace ZeldaTimelineGeneratorUtility
{
    /// <summary>
    /// Interaction logic for ExclusionsWindowxaml.xaml
    /// </summary>
    public partial class ExclusionsWindow : Window
    {
        private Game selectedGame;

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

        public ExclusionsWindow()
        {
            InitializeComponent();
        }

        public ExclusionsWindow(Game game)
        {
            SelectedGame = game;
            InitializeComponent();

            dataGrid.ItemsSource = SelectedGame.Exclusions;

            dataGrid.Items.Refresh();
        }

        private void dataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new Exclusion(SelectedGame.GameTitle);
        }
    }
}
