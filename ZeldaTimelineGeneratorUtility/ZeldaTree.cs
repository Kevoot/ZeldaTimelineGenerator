using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZeldaTimelineGeneratorUtility
{
    public class ZeldaTree
    {
        public List<ZeldaTreeNode> NodeCollection = new List<ZeldaTreeNode>();

        public int numTablesCreated = 0;

        public ZeldaTree()
        {

        }

        public ZeldaTree(ObservableCollection<Game> games)
        {
            try
            {
                GenerateCollectionFromGames(games.ToList());
                GenerateGraphWindow();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void GenerateGraphWindow()
        {
            try
            {
                GenerateMsaglGraph();
            }
            catch(Exception ex)
            {
                // Need to do better exception handling
                throw ex;
            }
        }

        public void GenerateMsaglGraph()
        {
            var graphWindow = new GraphGenerator(NodeCollection);
            NodeCollection = graphWindow.ShowGraphs();
        }

        private void GenerateCollectionFromGames(List<Game> games)
        {
            NodeCollection = new List<ZeldaTreeNode>();
            foreach(var game in games)
            {
                NodeCollection.Add(new ZeldaTreeNode(game));
            }
            return;
        }

        // Once editing is complete, put data back into tables
        private void GenerateGamesFromCollection()
        {
            var gameCollection = new ObservableCollection<Game>();
            foreach(var node in NodeCollection)
            {
                gameCollection.Add(new Game(node));
            }
        }
    }

    public class TreeDirectConnectionString
    {
        public string Reason = string.Empty;
        public string Rating = string.Empty;
        public string Comment = string.Empty;
        private DirectConnection Conn = null;

        public TreeDirectConnectionString()
        {
            Reason = "No Data";
            Rating = "No Data";
            Comment = "No Data";
        }

        public TreeDirectConnectionString(DirectConnection reason)
        {
            this.Conn = reason;
            Reason = Conn.Description;
            Rating = Conn.Rating.ToString();
            Comment = Conn.Comment;
        }
    }

    public class Link
    {
        public GameEnum ParentLink, ChildLink;
        // 0 = Exclusion, 1 = Direct Connection,
        public bool Type;

        public Link(GameEnum t1, GameEnum t2, bool type)
        {
            ParentLink = t1;
            ChildLink = t2;
            Type = type;
        }
    }
}