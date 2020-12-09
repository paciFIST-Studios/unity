using UnityEngine;

public class DestructWithRemains : MonoBehaviour
{
    [SerializeField] GameObject remains;

    public void Do()
    {
        Instantiate(remains, transform.position, transform.rotation);
        transform.SetPositionAndRotation(new Vector3(0f, -1000f, 0f), Quaternion.identity);
        DestroyObject(this, 1f);
    }
}
