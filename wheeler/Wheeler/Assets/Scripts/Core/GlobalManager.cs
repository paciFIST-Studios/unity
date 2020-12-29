using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    // https://wiki.unity3d.com/index.php/Toolbox


    // Runtime components
    private Dictionary<string, Component> _components = new Dictionary<string, Component>();


    // no one is allowed to use this
    protected GlobalManager() { }

    private void Awake()
    {
        InitializePlayerData();
        InitializeDialogueManager();
    }


    private void InitializePlayerData()
    { }

    private void InitializeDialogueManager()
    {
        this.AddGlobalComponent("dialogue_maanger", typeof(DialogueManager));
        print("Added Dialogue Manager");
    }


    public Component AddGlobalComponent(string id, System.Type type)
    {
        if(_components.ContainsKey(id))
        {
            Debug.LogWarning(string.Format($"GlobalManager component id: {0}  already exists! Returning extant", id));
            return GetGlobalComponent(id);
        }

        var component = this.gameObject.AddComponent(type);
        _components[id] = component;
        return component;
    }

    public void RemoveGlobalComponent(string id)
    {
        Component component;

        if(_components.TryGetValue(id, out component))
        {
            Destroy(component);
            _components.Remove(id);
        }
        else
        {
            Debug.LogWarning(string.Format($"GlobalManager trying to remove component id={0}; COMPONENT NOT FOUND!", id));
        }
    }


    public Component GetGlobalComponent(string id)
    {
        Component component;

        if(_components.TryGetValue(id, out component))
        {
            return component;
        }

        Debug.LogWarning(string.Format($"GlobalManager try to return component id={0}; COMPONENT NOT FOUND!", id));

        return null;
    }
}
