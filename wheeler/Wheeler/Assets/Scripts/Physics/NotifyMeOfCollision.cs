using UnityEngine;
using UnityEngine.Events;

public class NotifyMeOfCollision : MonoBehaviour
{
    UnityEvent callback = null;

    [SerializeField]
    private bool triggerOnce = true;
    [SerializeField]
    private bool isTriggered = false;

    public void Register(UnityAction cb)
    {
        if(callback == null)
        {
            callback = new UnityEvent();
        }

        callback.AddListener(cb);
    }

    public void Unregister(UnityAction cb)
    {
        callback.RemoveListener(cb);
    }

    public void OnParticleCollision(GameObject other)
    {
        if(triggerOnce && isTriggered) { return; }
        if(callback == null) { return; }

        callback.Invoke();
        isTriggered = true;
    }
}
