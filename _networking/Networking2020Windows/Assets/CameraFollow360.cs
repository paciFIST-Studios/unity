using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow360 : MonoBehaviour
{
    [SerializeField] static public Transform player;

    [SerializeField] private float distance = 10f;
    [SerializeField] private float height = 5f;
    [SerializeField] private Vector3 lookOffset;
    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    private void LateUpdate()
    {
        if(player)
        {
            Vector3 lookPosition = player.position + lookOffset;
            Vector3 relativePos = lookPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed * 0.1f);
            Vector3 targetPos = player.transform.position + player.transform.up * height - player.transform.forward * distance;

            this.transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed * 0.1f);
        }
    }


}
