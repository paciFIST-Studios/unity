using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CreateNodesExample
{
    // https://www.pelican7.com/blog/2018/2/9/creating-a-graph-data-structure-with-support-for-polymorphism-in-unity

    [MenuItem("Window/NodeGraph/Create Example Objects")]
    public static void CreateExample()
    {
        var graph = Graph.Create("Example Graph");

        var nodeA = NodeBase.Create<NodeBase>("ExampleA");
        var nodeB = NodeBase.Create<SpriteNode>("ExampleB");
        nodeA.Neighbors.Add(nodeB);

        graph.AddNode(nodeA);
        graph.AddNode(nodeB);
    }


}
