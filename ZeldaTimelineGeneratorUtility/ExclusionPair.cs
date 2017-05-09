using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    public class Exclusion : INotifyPropertyChanged
    {
        private GameEnum sourceGame, targetGame;

        private string reason;

        private ExclusionOrder order;

        private bool enabled;

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

        public ExclusionOrder Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
                NotifyPropertyChanged("Order");
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

        public string Reason
        {
            get
            {
                return reason;
            }
            set
            {
                reason = value;
                NotifyPropertyChanged("Reason");
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                NotifyPropertyChanged("Enabled");
            }
        }

        public Exclusion()
        {
            Reason = "<Reason>";
            SourceGame = GameEnum.NoData;
            Order = ExclusionOrder.NoData;
            TargetGame = GameEnum.NoData;
            Enabled = true;
        }

        public Exclusion(string reason)
        {
            Reason = reason;
            SourceGame = GameEnum.NoData;
            Order = ExclusionOrder.NoData;
            TargetGame = GameEnum.NoData;
            Enabled = true;
        }

        public Exclusion(GameEnum source)
        {
            Reason = "<Reason>";
            SourceGame = source;
            Order = ExclusionOrder.NoData;
            TargetGame = GameEnum.NoData;
            Enabled = true;
        }

        public Exclusion(GameEnum source, GameEnum target)
        {
            Reason = "<Reason>";
            SourceGame = source;
            Order = ExclusionOrder.NoData;
            TargetGame = target;
            Enabled = true;
        }

        public Exclusion(GameEnum source, GameEnum target, string reason)
        {
            Reason = reason;
            SourceGame = source;
            Order = ExclusionOrder.NoData;
            TargetGame = target;
            Enabled = true;
        }

        public Exclusion(GameEnum source, ExclusionOrder order, GameEnum target, string reason)
        {
            Reason = reason;
            SourceGame = source;
            Order = order;
            TargetGame = target;
            Enabled = true;
        }

        public Exclusion(GameEnum source, ExclusionOrder o, GameEnum target, string r, bool e)
        {
            Reason = r;
            SourceGame = source;
            Order = o;
            TargetGame = target;
            Enabled = e;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
