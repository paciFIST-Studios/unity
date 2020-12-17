using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct CameraSmoothFollowPosition
{ 
    [SerializeField] public Vector3 followTargetOffsetPosition;
    [SerializeField] public Vector3 cameraOffsetPosition;
}

public class CameraSmoothFollowController : MonoBehaviour
{

    [Header("Follow Target Settings")]
    // a reference to the target, which the camera is meant to follow
    [SerializeField] public Transform followTarget;
    // an offset, which changes the "lead" the camera has on the follow target
    [SerializeField] private Vector3 followTargetOffsetPosition;
    [SerializeField] private bool markFollowTargetPosition;

    [SerializeField] private GameObject followTargetMarkerPrefab;
    private GameObject followTargetMarker;

    [Header("Camera Settings")]
    // an offset, which changes the camera's position
    [SerializeField] private Vector3 cameraOffsetPosition;
    // translation speed is how fast the camera moves
    [SerializeField] private float cameraTranslationSpeed = 10f;
    // rotation speed is how fast the camera turns
    [SerializeField] private float cameraRotationSpeed = 10f;

    // the camera preset values we should base our position off of
    [SerializeField] private int useCameraPresetIdx = 0;
    // named, preset positions the camera may be placed at, around the follow target
    [SerializeField] private List<CameraSmoothFollowPosition> presetPositions;

    [Header("Smoothing")]
    // lerp smoothing is applied to translational movement
    [SerializeField] private float lerpSmoothing = 0.1f;
    // slerp smoothing is applied to rotational movement
    [SerializeField] private float slerpSmoothing = 0.1f;

    private void Start()
    {
        followTargetMarker = Instantiate(followTargetMarkerPrefab);
        followTargetMarker.transform.SetParent(transform);
    }

    // camera is done in late update, so it takes place after all of the user input
    // is collected, and after all of the movement has been applied by physics
    private void LateUpdate()
    {
        if (followTarget)
        {
            cameraOffsetPosition       = presetPositions[useCameraPresetIdx].cameraOffsetPosition;
            followTargetOffsetPosition = presetPositions[useCameraPresetIdx].followTargetOffsetPosition;

            // branchless code, which takes care of the follow marker.  It's childed to the camera
            {
                followTargetMarker.GetComponent<MeshRenderer>().enabled = markFollowTargetPosition;
                followTargetMarker.transform.position = followTarget.position + followTargetOffsetPosition;
            }

            // this step needs to be performed, but I'm just not sure how to do it yet.  Don't worry, it will come. <3
            //{
            //    // pre-rotate vectors so they fit with the current local space of the target
            //    var targetRotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, followTargetOffsetPosition);
            //    var cameraRotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, cameraOffsetPosition);
            //}


            Vector3 lookPosition = followTarget.position + followTargetOffsetPosition;
            Vector3 relativePos = lookPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            transform.rotation = Quaternion.Slerp(
                  transform.rotation
                , rotation
                , Time.deltaTime * cameraRotationSpeed * slerpSmoothing
                );

            Vector3 cameraDesiredPosition = 
                  followTarget.transform.position 
                  // we use a minus here for convenience sake in-editor.  
                  // A positive value represents "distance away from" the target.
                  // we *could* cange this to a plus, and then provide a -dist, to show
                  // how far behind the camera should follow, but this saves a keystroke
                - cameraOffsetPosition.x * followTarget.transform.forward
                + cameraOffsetPosition.y * followTarget.transform.up
                + cameraOffsetPosition.z * followTarget.transform.right
                ;

            transform.position = Vector3.Lerp(
                  transform.position
                , cameraDesiredPosition
                , Time.deltaTime * cameraTranslationSpeed * lerpSmoothing
                );
        }
    }
}
