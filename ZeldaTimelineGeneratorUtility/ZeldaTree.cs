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
using TreeGenerator;

namespace ZeldaTimelineGeneratorUtility
{
    public class ZeldaTree
    {
        public ZeldaTreeNode RootNode = null;
        public List<ZeldaTreeNode> LeafNodes = new List<ZeldaTreeNode>();
        public List<ZeldaTreeNode> OrphanNodes = new List<ZeldaTreeNode>();
        public List<ZeldaTreeNode> NodeCollection = new List<ZeldaTreeNode>();
        public List<Link> ConnectionLinks = new List<Link>();
        public List<TreeData.TreeDataTableDataTable> CompletedTables = new List<TreeData.TreeDataTableDataTable>();
        public TreeData.TreeDataTableDataTable ZeldaDataTableTree = new TreeData.TreeDataTableDataTable();

        public int numTablesCreated = 0;

        public ZeldaTree()
        {

        }

        public ZeldaTree(ZeldaTreeNode rootNode)
        {
            RootNode = rootNode;
        }

        public ZeldaTree(ObservableCollection<Game> games)
        {
            GenerateCollectionFromGames(games.ToList());
            AddReferencedGames();
            RemoveOrphanNodes();
            SetRootNode();
            SetLeafNodes();          
            GenerateTreeFromCollection();
            GenerateGraphImage();
        }

        private void AddReferencedGames()
        {
            foreach (GameEnum gameTitle in Enum.GetValues(typeof(GameEnum)))
            {
                if(gameTitle == GameEnum.NoData)
                {
                    continue;
                }
                var game = new Game(gameTitle);
                if (NodeCollection.Where(outerNode => outerNode.SourceGame.DirectConnections
                .Any(innerNode => innerNode.TargetGame == gameTitle)).Count() > 0)
                {
                    var references = NodeCollection.Where(outerNode => outerNode.SourceGame.DirectConnections
                    .Any(innerNode => innerNode.TargetGame == gameTitle));
                    CreateExclusionsFromReferences(game, references);
                }
                if(NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                .Any(innerNode => innerNode.SourceGame == gameTitle)).Count() > 0)
                {
                    var references = NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                    .Any(innerNode => innerNode.SourceGame == gameTitle));
                    CreateExclusionsFromReferences(game, references);
                }
                if(NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                .Any(innerNode => innerNode.TargetGame == gameTitle)).Count() > 0)
                {
                     var references = NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                    .Any(innerNode => innerNode.TargetGame == gameTitle));
                    CreateExclusionsFromReferences(game, references);
                }
                if(game.Exclusions.Count > 0 && !NodeCollection.Any(node => node.GameTitle == gameTitle))
                {
                    game.GameId = 0;
                    NodeCollection.Add(new ZeldaTreeNode(game));
                }
            }
        }

        private static void CreateExclusionsFromReferences(Game game, IEnumerable<ZeldaTreeNode> references)
        {
            foreach (var reference in references)
            {
                foreach (var ex in reference.SourceGame.Exclusions)
                {
                    if (ex.Order == ExclusionOrder.CantBeAfter)
                    {
                        game.Exclusions.Add(
                            new Exclusion(ex.TargetGame, ExclusionOrder.CantBeBefore, ex.SourceGame, "From parent"));
                    }
                    else if (ex.Order == ExclusionOrder.CantBeBefore)
                    {
                        game.Exclusions.Add(
                            new Exclusion(ex.TargetGame, ExclusionOrder.CantBeAfter, ex.SourceGame, "From parent"));
                    }
                    else if (ex.Order == ExclusionOrder.MustBeAfter)
                    {
                        game.Exclusions.Add(
                            new Exclusion(ex.TargetGame, ExclusionOrder.MustBeBefore, ex.SourceGame, "From parent"));
                    }
                    else if (ex.Order == ExclusionOrder.MustBeBefore)
                    {
                        game.Exclusions.Add(
                            new Exclusion(ex.TargetGame, ExclusionOrder.MustBeAfter, ex.SourceGame, "From parent"));
                    }
                }
            }
        }

        private void RemoveOrphanNodes()
        {
            var pendingDeletions = new List<ZeldaTreeNode>();
            // Find all nodes that are not referenced and do not reference any others
            foreach (var node in NodeCollection)
            {
                if(node.SourceGame.DirectConnections.Count() < 1 && node.SourceGame.Exclusions.Count() < 1 &&
                    NodeCollection.Where(outerNode => outerNode.SourceGame.DirectConnections.Any(innerNode => innerNode.TargetGame == node.GameTitle)).Count() < 1
                    && NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions.Any(innerNode => innerNode.SourceGame == node.GameTitle)).Count() < 1
                    && NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions.Any(innerNode => innerNode.TargetGame == node.GameTitle)).Count() < 1)
                {
                    pendingDeletions.Add(node);
                }
            }
            OrphanNodes = pendingDeletions;
            foreach(var node in pendingDeletions)
            {
                NodeCollection.Remove(node);
            }
        }

        private int SetRootNode()
        {
            if (NodeCollection.ToList().Where(node => node.IsRootNode).Count() < 1)
            {
                var possibleRoots = new List<ZeldaTreeNode>();

                possibleRoots = FindPossibleRoots();

                // If too many roots are found
                if (possibleRoots.Count > 1)
                {
                    return UserPromptForRootChoice(1);
                }
                // If no roots are found
                else if (possibleRoots.Count < 1)
                {
                    return UserPromptForRootChoice(0);
                }
                // If only one root is found
                else
                {
                    possibleRoots.First().IsRootNode = true;
                    RootNode = possibleRoots.First();
                    return 1;
                }
            }
            else if (NodeCollection.ToList().Where(node => node.IsRootNode).Count() > 1)
            {
                return UserPromptForRootChoice(1);
            }
            else
            {
                RootNode = NodeCollection.First(node => node.IsRootNode);
                return 1;
            }
        }

        private void SetLeafNodes()
        {
            var possibleLeafNodes = NodeCollection.Where(node => node.LeafNode && (node.SourceGame.DirectConnections.Count == 0)).ToList();
            var pendingDeletions = new List<ZeldaTreeNode>();
            foreach(var node in possibleLeafNodes)
            {
                if (NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                     .Any(innerNode => innerNode.SourceGame == node.GameTitle && innerNode.Order == ExclusionOrder.MustBeAfter)).Count() > 0)
                {
                    if (pendingDeletions.Contains(node))
                    {
                        pendingDeletions.Remove(node);
                    }
                }
                else if (NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                    .Any(innerNode => innerNode.SourceGame == node.GameTitle && innerNode.Order == ExclusionOrder.CantBeBefore)).Count() > 0)
                {
                    if (pendingDeletions.Contains(node))
                    {
                        pendingDeletions.Remove(node);
                    }
                }
                else if (NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                    .Any(innerNode => innerNode.TargetGame == node.GameTitle && innerNode.Order == ExclusionOrder.CantBeAfter)).Count() > 0)
                {
                    if (pendingDeletions.Contains(node))
                    {
                        pendingDeletions.Remove(node);
                    }
                }
                else if (NodeCollection.Where(outerNode => outerNode.SourceGame.Exclusions
                    .Any(innerNode => innerNode.TargetGame == node.GameTitle && innerNode.Order == ExclusionOrder.CantBeAfter)).Count() > 0)
                {
                    if (pendingDeletions.Contains(node))
                    {
                        pendingDeletions.Remove(node);
                    }
                }
                else continue;
            }
            // Remove all non-leaf nodes, and add all leafs to leaf list.
            foreach(var node in possibleLeafNodes)
            {
                if(!pendingDeletions.Contains(node))
                {
                    node.LeafNode = true;
                    LeafNodes.Add(node);
                }
            }
            LeafNodes = possibleLeafNodes;
        }

        private int UserPromptForRootChoice(int mode)
        {
            MessageBoxResult result;
            if(mode == 0)
            {
                result = MessageBox.Show("No root node was able to be defined, please check your data. \n\n" +
                "A root node cannot be the target of a directConnection, can't be the source in a \n" +
                "\"Must be after\" or \"Can't be before\" exclusion, and can't be the target in a \n"
                + "\"Must be before\" or \"Can't be after\" exclusion (Logically it makes sense!)" +
                "\n\n Select root node manually now? \n Yes to select, No to proceed anyways (not recommended) \n" + 
                "Cancel to go back to editing", "No root node defined!",
                MessageBoxButton.YesNoCancel);
            }
            else
            {
                result = MessageBox.Show("Too many roots are possible, please select one from the following menu \n" + 
                    "or hit no to proceed anyways (not recommended), or cancel to go back to editing", "Too many roots!", 
                    MessageBoxButton.YesNoCancel);
            }
            if (result == MessageBoxResult.Yes)
            {
                var rootChoser = new RootNodeChoser(new ObservableCollection<ZeldaTreeNode>(NodeCollection));
                rootChoser.ShowDialog();
                // After done, check for root status
                if (NodeCollection.Any(node => node.IsRootNode))
                {
                    RootNode = NodeCollection.First(node => node.IsRootNode);
                    return 1;
                }
                else
                {
                    MessageBox.Show("No root chosen, this may get a bit goofy");
                    return 0;
                }
            }
            else
            {
                MessageBox.Show("No root chosen, this may get a bit goofy");
                return 0;
            }
        }

        private List<ZeldaTreeNode> FindPossibleRoots()
        {
            var directConnectionResults = new List<ZeldaTreeNode>();
            var cbbResults = new List<ZeldaTreeNode>();
            var cbaResults = new List<ZeldaTreeNode>();
            var mbbResults = new List<ZeldaTreeNode>();
            var mbaResults = new List<ZeldaTreeNode>();

            // Find any nodes that are not referenced as targets in directConnections
            DirectConnectionTest(directConnectionResults);
            CbbExclusionTest(cbbResults);
            CbaExclusionTest(cbaResults);
            MbaExclusionTest(mbaResults);
            MbbExclusionTest(mbbResults);

            return directConnectionResults.Where(results => cbbResults.Contains(results)
                       && cbaResults.Contains(results)
                       && mbbResults.Contains(results)
                       && mbaResults.Contains(results))
                       .ToList();
        }

        private void DirectConnectionTest(List<ZeldaTreeNode> directConnectionResults)
        {
            foreach (var testNode in NodeCollection)
            {
                var testFlag = false;
                foreach (var node in NodeCollection)
                {
                    if (node.SourceGame.DirectConnections.Any(directConnection => directConnection.TargetGame == testNode.GameTitle))
                    {
                        testFlag = true;
                        break;
                    }

                }
                if (!testFlag)
                {
                    directConnectionResults.Add(testNode);
                }
            }
        }

        private void MbbExclusionTest(List<ZeldaTreeNode> mbbResults)
        {
            // Exclude any nodes that are the target "Must be before" exclusion
            foreach (var testNode in NodeCollection)
            {
                var testFlag = false;
                foreach (var node in NodeCollection)
                {
                    if (node.SourceGame.Exclusions.Any(exclusion => exclusion.TargetGame == testNode.GameTitle
                         && exclusion.Order == ExclusionOrder.MustBeBefore))
                    {
                        testFlag = true;
                        break;
                    }

                }
                if (!testFlag)
                {
                    mbbResults.Add(testNode);
                }
            }
        }

        private void MbaExclusionTest(List<ZeldaTreeNode> mbaResults)
        {
            // Exclude any nodes that are the source in "Must be after" exclusion
            foreach (var testNode in NodeCollection)
            {
                var testFlag = false;
                foreach (var node in NodeCollection)
                {
                    if (node.SourceGame.Exclusions.Any(exclusion => exclusion.SourceGame == testNode.GameTitle
                         && exclusion.Order == ExclusionOrder.MustBeAfter))
                    {
                        testFlag = true;
                        break;
                    }

                }
                if (!testFlag)
                {
                    mbaResults.Add(testNode);
                }
            }
        }

        private void CbaExclusionTest(List<ZeldaTreeNode> cbaResults)
        {
            // Exclude any nodes that are the target in "Can't be after" exclusion
            foreach (var testNode in NodeCollection)
            {
                var testFlag = false;
                foreach (var node in NodeCollection)
                {
                    if (node.SourceGame.Exclusions.Any(exclusion => exclusion.TargetGame == testNode.GameTitle
                         && exclusion.Order == ExclusionOrder.CantBeAfter))
                    {
                        testFlag = true;
                        break;
                    }

                }
                if (!testFlag)
                {
                    cbaResults.Add(testNode);
                }
            }
        }

        private void CbbExclusionTest(List<ZeldaTreeNode> cbbResults)
        {
            // Exclude any nodes that aren the source in "Can't be before" exclusion
            foreach (var testNode in NodeCollection)
            {
                var testFlag = false;
                foreach (var node in NodeCollection)
                {
                    if (node.SourceGame.Exclusions.Any(exclusion => exclusion.SourceGame == testNode.GameTitle
                         && exclusion.Order == ExclusionOrder.CantBeBefore))
                    {
                        testFlag = true;
                        break;
                    }

                }
                if (!testFlag)
                {
                    cbbResults.Add(testNode);
                }
            }
        }

        private void GenerateTreeFromCollection()
        {
            AddDirectConnectionsToTree();
            foreach(var node in NodeCollection)
            {
                node.Visited = false;
            }
            AddExclusionsToNodes(RootNode);
            AssignChildren(RootNode.Children);
            AssignChildren(NodeCollection.Where(node => !node.Visited).ToList());
        }

        private void AddExclusionsToNodes(ZeldaTreeNode zNode)
        {
            if(zNode.Visited)
            {
                return;
            }
            else
            {
                zNode.Visited = true;
            }
            foreach (var ex in zNode.SourceGame.Exclusions)
            {
                // Exclusion dictates node must be before target
                if(ex.SourceGame == zNode.GameTitle && (ex.Order == ExclusionOrder.CantBeAfter || ex.Order == ExclusionOrder.MustBeBefore))
                {
                    var connectedNode = NodeCollection.FirstOrDefault(innerNode => innerNode.SourceGame.GameTitle == ex.TargetGame);
                    if(connectedNode != null)
                    {
                        if (!zNode.nodesAfterByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex)))
                        {
                            NodeCollection.FirstOrDefault(node => node.GameTitle == zNode.GameTitle)
                            .nodesAfterByExclusions
                            .Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex));
                        }
                        if (!connectedNode.nodesBeforeByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex)))
                        {
                            connectedNode.nodesBeforeByExclusions.Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex));
                        }
                    }
                }
                else if(ex.SourceGame == zNode.GameTitle && (ex.Order == ExclusionOrder.CantBeBefore || ex.Order == ExclusionOrder.MustBeAfter))
                {
                    var connectedNode = NodeCollection.FirstOrDefault(innerNode => innerNode.SourceGame.GameTitle == ex.TargetGame);
                    if (connectedNode != null)
                    {
                        if (!zNode.nodesBeforeByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex)))
                        {
                            NodeCollection.FirstOrDefault(node => node.GameTitle == zNode.GameTitle)
                            .nodesBeforeByExclusions
                            .Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex));
                        }
                        if (!connectedNode.nodesAfterByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex)))
                        {
                            connectedNode.nodesAfterByExclusions.Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex));
                        }
                    }
                }
                else if (ex.TargetGame == zNode.GameTitle && (ex.Order == ExclusionOrder.CantBeAfter || ex.Order == ExclusionOrder.MustBeBefore))
                {
                    var connectedNode = NodeCollection.FirstOrDefault(innerNode => innerNode.SourceGame.GameTitle == ex.SourceGame);
                    if (connectedNode != null)
                    {
                        if (!zNode.nodesBeforeByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex)))
                        {
                            NodeCollection.FirstOrDefault(node => node.GameTitle == zNode.GameTitle)
                            .nodesBeforeByExclusions
                            .Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex));
                        }
                        if (!connectedNode.nodesAfterByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex)))
                        {
                            connectedNode.nodesAfterByExclusions.Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex));
                        }
                    }
                }
                else if (ex.TargetGame == zNode.GameTitle && (ex.Order == ExclusionOrder.CantBeBefore || ex.Order == ExclusionOrder.MustBeAfter))
                {
                    var connectedNode = NodeCollection.FirstOrDefault(innerNode => innerNode.SourceGame.GameTitle == ex.SourceGame);
                    if (connectedNode != null)
                    {
                        if (!zNode.nodesAfterByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex)))
                        {
                            NodeCollection.FirstOrDefault(node => node.GameTitle == zNode.GameTitle)
                            .nodesAfterByExclusions
                            .Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(connectedNode, ex));
                        }
                        if (!connectedNode.nodesBeforeByExclusions.Contains(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex)))
                        {
                            connectedNode.nodesBeforeByExclusions.Add(new KeyValuePair<ZeldaTreeNode, Exclusion>(zNode, ex));
                        }
                    }
                }
            }
            foreach(var node in zNode.nodesAfterByExclusions.ToList())
            {
                AddExclusionsToNodes(node.Key);
            }
            foreach(var node in zNode.nodesBeforeByExclusions.ToList())
            {
                AddExclusionsToNodes(node.Key);
            }
            foreach (var node in zNode.Children.ToList())
            {
                AddExclusionsToNodes(node);
            }
            if(zNode.Parent != null)
            {
                AddExclusionsToNodes(zNode.Parent);
            }
        }

        private void AddDirectConnectionsToTree()
        {
            List<ZeldaTreeNode> pendingRootAdditions = new List<ZeldaTreeNode>();
            // Now that we know the root, we can begin setting up the data structure
            // Which the graphics will mirror
            // Start with the Root's children
            foreach (var child in RootNode.SourceGame.DirectConnections)
            {
                var pendingChild = (NodeCollection.FirstOrDefault(node => node.GameTitle == child.TargetGame));
                if (pendingChild != null)
                {
                    if (CheckForDuplicateLinkAndAdd(new Link(RootNode.GameTitle, pendingChild.GameTitle, true)))
                    {
                        pendingChild.Parent = RootNode;
                        RootNode.Children.Add(pendingChild);
                    }
                }
            }
            foreach (var child in RootNode.Children)
            {
                ConnectionLinks.Add(new Link(RootNode.GameTitle, child.GameTitle, true));
            }
        }

        private bool CheckForDuplicateLinkAndAdd(Link resultLink)
        {
            if (!ConnectionLinks.Contains(new Link(resultLink.ParentLink, resultLink.ChildLink, resultLink.Type))
                && !ConnectionLinks.Contains(new Link(resultLink.ChildLink, resultLink.ParentLink, resultLink.Type)))
            {
                ConnectionLinks.Add(resultLink);
                return true;
            }
            else return false;
        }

        private void AssignChildren(List<ZeldaTreeNode> nodes)
        {
            if(nodes.Count < 1)
            {
                return;
            }
            var pendingChildren = new List<ZeldaTreeNode>();
            /*if(nodes.All(node => node.SourceGame.DirectConnections.Count() == 0))
            {
                return;
            }*/
            foreach(var node in nodes)
            {
                //if (node.SourceGame.DirectConnections.Count() == 0) continue;
                //else
                //{
                foreach (var child in node.SourceGame.DirectConnections)
                {
                    var pendingChild = NodeCollection.FirstOrDefault(innerNode => innerNode.GameTitle == child.TargetGame);
                    if(pendingChild != null)
                    {
                        if(CheckForDuplicateLinkAndAdd(new Link(node.SourceGame.GameTitle, pendingChild.GameTitle, true)))
                        {
                            pendingChild.Parent = node;
                            node.Children.Add(pendingChild);
                            pendingChildren.Add(pendingChild);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                //}
                // Currently overrides Connections, next step is to add rating selection
                foreach (var child in node.SourceGame.Exclusions)
                {
                    
                    var pendingChild = NodeCollection.FirstOrDefault(innerNode => (innerNode.GameTitle == child.TargetGame 
                                        && innerNode.GameTitle != node.GameTitle) || (innerNode.GameTitle == child.SourceGame
                                        && innerNode.GameTitle != node.GameTitle));
                    if (pendingChild != null)
                    {
                        if (!ConnectionLinks.Contains(new Link(child.SourceGame, child.TargetGame, false)))
                        {
                            ConnectionLinks.Add(new Link(child.SourceGame, child.TargetGame, false));
                            if((child.SourceGame == pendingChild.GameTitle
                                && (child.Order == ExclusionOrder.CantBeAfter || child.Order == ExclusionOrder.MustBeBefore)) ||
                                (child.TargetGame == pendingChild.GameTitle
                                && (child.Order == ExclusionOrder.CantBeBefore || child.Order == ExclusionOrder.MustBeAfter)))
                            {
                                pendingChild.Children.Add(node);
                                node.Parent = pendingChild;
                                if(pendingChild.Visited == false)
                                {
                                    pendingChild.Visited = true;
                                    pendingChildren.Add(pendingChild);
                                }
                            }
                            else
                            {
                                node.Children.Add(pendingChild);
                                pendingChild.Parent = node;
                                if (pendingChild.Visited == false)
                                {
                                    pendingChild.Visited = true;
                                    pendingChildren.Add(pendingChild);
                                }
                            }
                        }
                    }
                }
            }
            if(pendingChildren.Count > 0)
            {
                AssignChildren(pendingChildren);
            }
        }

        private void GenerateGraphImage()
        {
            GenerateConnections();
            GenerateExclusions();
            // Check to see if table is already a created one
            if(!CompletedTables.Contains(ZeldaDataTableTree))
            {
                numTablesCreated += 1;
                var ZeldaTree = new TreeBuilder(ZeldaDataTableTree);
                ZeldaTree.BoxHeight = ZeldaTree.BoxHeight * 2;
                ZeldaTree.BoxWidth = ZeldaTree.BoxWidth * 2;
                ZeldaTree.FontSize = 4;
                var ZeldaTreeImage = Image.FromStream(ZeldaTree.GenerateTree(RootNode.GameTitle.ToString(), System.Drawing.Imaging.ImageFormat.Bmp));
                ZeldaTreeImage.Save("Graph" + numTablesCreated + ".bmp");
            }
        }

        private void GenerateExclusions()
        {
            // To be added
        }

        private void GenerateConnections()
        {
            foreach (var node in NodeCollection)
            {
                var sbDetails = new StringBuilder();
                sbDetails.Append("*" + node.GameTitle + "*\n");
                foreach (var reason in node.Reasons)
                {
                    sbDetails.Append("+" + reason.Reason + "\n+Strength of Evidence: " + reason.Rating + "\n+Category: " + reason.Category);
                }
                var sbComments = new StringBuilder();
                foreach (var reason in node.Reasons)
                {
                    sbComments.Append("Comment: " + reason.Comment + "\n");
                }
                ZeldaDataTableTree.AddTreeDataTableRow(node.GameTitle.ToString(),
                    ((node.Parent == null) ? "" : node.Parent.GameTitle.ToString()),
                    sbDetails.ToString(), sbComments.ToString());
            }
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
    }

    public class TreeDirectConnectionString
    {
        public string Reason = string.Empty;
        public string Category = string.Empty;
        public string Rating = string.Empty;
        public string Comment = string.Empty;
        private DirectConnection Conn = null;

        public TreeDirectConnectionString()
        {
            Reason = "No Data";
            Category = "No Data";
            Rating = "No Data";
            Comment = "No Data";
        }

        public TreeDirectConnectionString(DirectConnection reason)
        {
            this.Conn = reason;
            Reason = Conn.Description;
            Category = Conn.Category;
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