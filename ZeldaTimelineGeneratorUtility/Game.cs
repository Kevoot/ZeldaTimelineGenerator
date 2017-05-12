using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    public class Game : INotifyPropertyChanged
    {
        private int gameId;
        private GameEnum sourceGame;
        private ObservableCollection<DirectConnection> directConnections;
        private ObservableCollection<Exclusion> exclusions;

        public ObservableCollection<Exclusion> Exclusions
        {
            get
            {
                return exclusions;
            }
            set
            {
                exclusions = value;
                NotifyPropertyChanged("Exclusions");
            }
        }

        public int GameId
        {
            get
            {
                return gameId;
            }
            set
            {
                gameId = value;
                NotifyPropertyChanged("GameId");
            }
        }

        public GameEnum GameTitle
        {
            get
            {
                return sourceGame;
            }
            set
            {
                sourceGame = value;
                NotifyPropertyChanged("SourceGame");
            }
        }


        public ObservableCollection<DirectConnection> DirectConnections
        {
            get
            {
                return directConnections;
            }
            set
            {
                directConnections = value;
                NotifyPropertyChanged("Connections");
            }
        }


        public Game()
        {
            GameId = 0;
            GameTitle = GameEnum.NoData;
            Exclusions = new ObservableCollection<Exclusion>();
            DirectConnections = new ObservableCollection<DirectConnection>();
        }

        // Copy constructor
        public Game(Game game)
        {
            GameTitle = game.GameTitle;
            GameId = game.GameId;
            foreach (var con in game.DirectConnections.ToList())
            {
                game.DirectConnections.Add(con);
            }
            foreach (var exc in game.Exclusions.ToList())
            {
                game.Exclusions.Add(exc);
            }
        }

        public Game(ZeldaTreeNode node)
        {
            gameId = node.SourceGame.GameId;
            GameTitle = node.SourceGame.GameTitle;
            Exclusions = node.SourceGame.Exclusions;
            DirectConnections = node.SourceGame.DirectConnections;
            foreach(var child in node.Children)
            {
                // Have to figure out the added graph data
            }
        }

        public Game(GameEnum title)
        {
            GameId = 0;
            GameTitle = title;
            Exclusions = new ObservableCollection<Exclusion>();
            DirectConnections = new ObservableCollection<DirectConnection>();
        }

        public Game(GameEnum sourceGame, ObservableCollection<DirectConnection> directConnections)
        {
            GameTitle = sourceGame;
            DirectConnections = directConnections;
            Exclusions = new ObservableCollection<Exclusion>();
        }

        public Game(GameEnum sourceGame, ObservableCollection<DirectConnection> directConnections, ObservableCollection<Exclusion> exclusions)
        {
            GameTitle = sourceGame;
            DirectConnections = directConnections;
            Exclusions = exclusions;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
