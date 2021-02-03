using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class NodeBase : ScriptableObject
{
    [SerializeField]
    private List<NodeBase> neighbors;

    public List<NodeBase> Neighbors
    {
        get
        {
            if (neighbors == null)
            {
                neighbors = new List<NodeBase>();
            }

            return neighbors;
        }
    }

    public static T Create<T>(string name)
        where T : NodeBase
    {
        var node = CreateInstance<T>();
        node.name = name;
        return node;
    }

}
