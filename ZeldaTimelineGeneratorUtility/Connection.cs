using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    public class DirectConnection : INotifyPropertyChanged
    {
        private int gameId;
        private string description;
        private int rating;
        private string category;
        private string comment;
        private GameEnum sourceGame, targetGame;

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
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }
        public int Rating
        {
            get
            {
                return rating;
            }
            set
            {
                rating = value;
                NotifyPropertyChanged("Rating");
            }
        }
        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                NotifyPropertyChanged("category");
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

        public GameEnum TargetGame
        {
            get
            {
                return targetGame;
            }
            set
            {
                targetGame = value;
                NotifyPropertyChanged("TargetGame");
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
            }
        }

        public DirectConnection()
        {
            GameId = 0;
            Description = "<Description>";
            Rating = 0;
            Category = "<Category>";
        }

        public DirectConnection(int gameId)
        {
            GameId = gameId;
            Description = "<Description>";
            Rating = 0;
            Category = "<Category>";
        }

        public DirectConnection(int gameId, GameEnum sourceGame)
        {
            GameId = gameId;
            Description = "<Description>";
            Rating = 0;
            Category = "<Category>";
            SourceGame = sourceGame;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
