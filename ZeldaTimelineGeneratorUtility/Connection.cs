using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    public class DirectConnection : INotifyPropertyChanged, IDataErrorInfo
    {
        private int gameId;
        private string description;
        private int rating;
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

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (columnName == "Description")
                {
                    // Validate property and return a string if there is an error
                    if (string.IsNullOrEmpty(Description) || Description.Equals("<Description>"))
                        return "A description is required";
                }
                else if (columnName == "Rating")
                {
                    if (Rating < 0 || Rating > 10)
                        return "Rating must be between 1-10";
                }
                else if (columnName == "TargetGame")
                {
                    if(TargetGame == null || TargetGame.Equals(GameEnum.NoData))
                    {
                        return "A target game must be selected";
                    }
                }
                // If there's no error, null gets returned
                return null;
            }
        }

        public DirectConnection()
        {
            GameId = 0;
            Description = "<Description>";
            Rating = 0;
        }

        public DirectConnection(int gameId)
        {
            GameId = gameId;
            Description = "<Description>";
            Rating = 0;
        }

        public DirectConnection(int gameId, GameEnum sourceGame)
        {
            GameId = gameId;
            Description = "<Description>";
            Rating = 0;
            SourceGame = sourceGame;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
