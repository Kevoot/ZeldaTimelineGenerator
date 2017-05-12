using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaTimelineGeneratorUtility
{
    public partial class GraphGenerator
    {
        List<ZeldaTreeNode> NodeCollection = new List<ZeldaTreeNode>();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "GraphGenerator";
        }

        #endregion

        public GraphGenerator(List<ZeldaTreeNode> nodeCollection)
        {
            NodeCollection = nodeCollection;
        }

        public List<ZeldaTreeNode> ShowGraphs()
        {
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");

            graph.UserData = NodeCollection;


            foreach (var node in graph.UserData as List<ZeldaTreeNode>)
            {
                foreach (var directConnection in node.SourceGame.DirectConnections)
                {
                    graph.AddEdge(GameEnumDescription.GetEnumDescription(directConnection.SourceGame),
                        (directConnection.Description + "\nR: " + directConnection.Rating),
                        GameEnumDescription.GetEnumDescription(directConnection.TargetGame));
                }
                foreach(var exclusion in node.SourceGame.Exclusions)
                {
                    Edge edge;
                    if (exclusion.Order == ExclusionOrder.CantBeBefore || exclusion.Order == ExclusionOrder.MustBeAfter)
                    {
                        edge = new Edge(GameEnumDescription.GetEnumDescription(exclusion.TargetGame),
                        exclusion.Reason,
                        GameEnumDescription.GetEnumDescription(exclusion.SourceGame));
                    }
                    else
                    {
                        edge = new Edge(GameEnumDescription.GetEnumDescription(exclusion.SourceGame),
                        exclusion.Reason,
                        GameEnumDescription.GetEnumDescription(exclusion.TargetGame));
                    }
                    try
                    {
                        var ed = graph.Edges;
                        var duplicateEdge = graph.Edges.FirstOrDefault(innerNode => (innerNode.Source.Equals(edge.Source)
                        && innerNode.Target.Equals(edge.Target))
                        || (innerNode.Source.Equals(edge.Target)
                        && innerNode.Target.Equals(edge.Source)));
                        // SetEdgeColor(edge, exclusion.Rating);
                        if(duplicateEdge != null)
                        {
                            duplicateEdge.LabelText += ("\n" + edge.LabelText);
                        }
                        else
                        {
                            graph.AddEdge(edge.Source, edge.LabelText, edge.Target);
                        }
                    }
                    catch(Exception ex)
                    {
                        graph.AddPrecalculatedEdge(edge);
                        continue;
                    }                  
                }
            }

            SetFormat(graph);

            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(viewer);
            ResumeLayout();
            ShowDialog();

            return graph.UserData as List<ZeldaTreeNode>;
        }

        private void SetEdgeColor(Edge edge, int rating)
        {
            switch(rating)
            {
                // Add some color to the lines once the Rating system is complete
                
            }
        }

        private void SetFormat(Graph graph)
        {
            foreach(var edge in graph.Edges)
            {
                edge.Label.FontSize *= .5;
            }
            foreach(var node in graph.Nodes)
            {
                node.Attr.XRadius *= .6;
                node.Attr.YRadius *= .6;
                node.Label.FontSize *= .5;
                node.Attr.Color = Color.ForestGreen;
                node.Attr.FillColor = Color.DarkSeaGreen;
            }
        }
    }
}