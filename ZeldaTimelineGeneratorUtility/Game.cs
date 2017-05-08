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

        public GameEnum SourceGame
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
            SourceGame = GameEnum.NoData;
            Exclusions = new ObservableCollection<Exclusion>();
            DirectConnections = new ObservableCollection<DirectConnection>();
        }

        public Game(GameEnum sourceGame, ObservableCollection<DirectConnection> directConnections)
        {
            SourceGame = sourceGame;
            DirectConnections = directConnections;
            Exclusions = new ObservableCollection<Exclusion>();
        }

        public Game(GameEnum sourceGame, ObservableCollection<DirectConnection> directConnections, ObservableCollection<Exclusion> exclusions)
        {
            SourceGame = sourceGame;
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
