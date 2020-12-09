using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPadController : MonoBehaviour
{
    [SerializeField] GameObject targetPrefab;
    [SerializeField] Vector2 dimensions;
    [SerializeField] string TagLaunchpadAs;
    private List<GameObject> landingPadSegments;

    bool drawGizmo = true;

    private void Start()
    {
        drawGizmo = false;

        Vector3 centerOfTopLeft = transform.position;
        float halfSizeOfUnit = targetPrefab.transform.lossyScale.x * 0.5f;
        centerOfTopLeft.x += -(dimensions.x * 0.5f) + halfSizeOfUnit;
        centerOfTopLeft.z += -(dimensions.y * 0.5f) + halfSizeOfUnit;

        int totalBlocks = (int)Mathf.Ceil(dimensions.x * dimensions.y);
        landingPadSegments = new List<GameObject>(totalBlocks);

        for(int i = 0; i < dimensions.x; i++)
        {
            for(int j = 0; j < dimensions.y; j++)
            {
                var obj = Instantiate(targetPrefab);
                obj.tag = TagLaunchpadAs;
                Vector3 newPos = centerOfTopLeft + new Vector3(i, 0, j);
                obj.transform.SetPositionAndRotation(newPos, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                landingPadSegments.Add(obj);
            }
        }

    }

    private void OnDrawGizmos()
    {
        if(!drawGizmo) { return; }

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(
              transform.position
            , new Vector3(dimensions.x, 1, dimensions.y)
            );
    }

}
