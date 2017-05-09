using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    public class ZeldaTreeNode
    {
        private bool rootNode = false;
        private bool leafNode = false;
        public bool Visited = false;

        private ZeldaTreeNode parent;
        private List<ZeldaTreeNode> PossibleParents = new List<ZeldaTreeNode>();

        private List<ZeldaTreeNode> children = new List<ZeldaTreeNode>();
        private List<List<ZeldaTreeNode>> PossibleChildren = new List<List<ZeldaTreeNode>>(); 

        // The exclusions in this list will be possible games that could follow this one
        public List<KeyValuePair<ZeldaTreeNode, Exclusion>> nodesBeforeByExclusions = new List<KeyValuePair<ZeldaTreeNode, Exclusion>>();
        // The exclusions in this list will be possible games that could follow this one
        public List<KeyValuePair<ZeldaTreeNode, Exclusion>> nodesAfterByExclusions = new List<KeyValuePair<ZeldaTreeNode, Exclusion>>();

        public GameEnum GameTitle;
        public Game SourceGame;

        public List<TreeDirectConnectionString> Reasons = new List<TreeDirectConnectionString>();

        public bool IsRootNode
        {
            get
            {
                return rootNode;
            }
            set
            {
                rootNode = value;
            }
        }

        public bool LeafNode
        {
            get
            {
                return leafNode;
            }
            set
            {
                leafNode = value;
            }
        }

        public ZeldaTreeNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public List<ZeldaTreeNode> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
            }
        }

        public ZeldaTreeNode()
        {
            
        }

        public ZeldaTreeNode(Game game)
        {
            foreach (var reason in game.DirectConnections)
            {
                Reasons.Add(new TreeDirectConnectionString(reason));
            }
            SourceGame = game;
            GameTitle = game.GameTitle;
        }

        public ZeldaTreeNode(Game game, ZeldaTreeNode parent)
        {
            Parent = parent;
            foreach (var reason in game.DirectConnections)
            {
                Reasons.Add(new TreeDirectConnectionString(reason));
            }
            SourceGame = game;
            GameTitle = game.GameTitle;
        }
    }
}
