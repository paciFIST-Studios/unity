using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Vector3 heading;
    [SerializeField] private float speed;
    [SerializeField] private float despawnAfterSeconds;

    private float firedAtTime;

    private bool isInFlight;

    void Start()
    {
        Despawn();
    }

    void FixedUpdate()
    {
        if(firedAtTime + despawnAfterSeconds < Time.time)
        {
            Despawn();
            //Destroy(this);
        }

        var newPos = transform.forward * speed;
        transform.localPosition += newPos;

        var scale = transform.localScale;
        scale.y += 0.01f;
        transform.localScale = scale;
    }

    public void Fire(Vector3 direction, Vector3 position)
    {
        ResetScale();

        isInFlight = true;
        firedAtTime = Time.time;
        transform.forward = direction;
        transform.localPosition = position;
        toggleComponents(true);
    }

    public void Despawn()
    {
        isInFlight = false;
        toggleComponents(false);
        transform.position = Vector3.zero;

        ResetScale();
    }

    private void ResetScale()
    {
        var scale = transform.localScale;
        scale.y = 0.1f;
        transform.localScale = scale;
    }

    private void toggleComponents(bool on)
    {
        if(on)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    
    public bool IsInFlight()
    {
        return isInFlight;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Despawn();
    }
}
