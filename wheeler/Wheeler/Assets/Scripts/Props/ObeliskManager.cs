using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

// A lof of the "unused" values are used in editor,
// but only during value modification
#pragma warning disable CS0414


public class ObeliskManager : MonoBehaviour
{
    // Lights -----
    [ColoredFoldoutGroup("Lights/References", 0, 0, 1)][SerializeField][LabelWidth(100)]
    private Transform topLight;
    [ColoredFoldoutGroup("Lights/References", 0, 0, 1)][SerializeField][LabelWidth(100)]
    private Transform topOuterLight;
    [ColoredFoldoutGroup("Lights/References", 0, 0, 1)][SerializeField][LabelWidth(100)]
    private Transform[] sideLights;
    [ColoredFoldoutGroup("Lights", 0, 0, 1)][SerializeField][LabelWidth(100)]
    private Material lightMaterial;

    // Body mesh ----
    [ColoredFoldoutGroup("Body", 1, 0, 1)]
    [ColoredFoldoutGroup("Body/References", 1, 0, 1)][SerializeField][LabelWidth(120)]
    private Material sectionAMaterial;
    [ColoredFoldoutGroup("Body/References", 1, 0, 1)][SerializeField][LabelWidth(120)]
    private Material sectionBMaterial;

    [ColoredFoldoutGroup("Body/References")][SerializeField][LabelWidth(100)]
    private Transform[] sectionAObjects;
    [ColoredFoldoutGroup("Body/References")][SerializeField][LabelWidth(100)]
    private Transform[] sectionBObjects;

    // Light positions ----
    [ColoredFoldoutGroup("Positions", 1, 0, 0)][SerializeField]
    private Vector3[] sideLightTopPositions;
    [ColoredFoldoutGroup("Positions", 1, 0, 0)][SerializeField]
    private Vector3[] sideLightBottomPositions;

    [ColoredFoldoutGroup("Positions/Side Light Positions", 1, 0, 0)][SerializeField]
    private Vector3 topLightTopPosition;
    [ColoredFoldoutGroup("Positions/Side Light Positions", 1, 0, 0)][SerializeField]
    private Vector3 topLightBottomPosition;
    [ColoredFoldoutGroup("Positions/Side Light Positions", 1, 0, 0)][SerializeField]
    private Vector3 topOuterLightTopPosition;
    [ColoredFoldoutGroup("Positions/Side Light Positions", 1, 0, 0)][SerializeField]
    private Vector3 topOuterLightBottomPosition;


    // Gameplay mechanics ----
    [ColoredFoldoutGroup("Gameplay", 0, 1, 0)][SerializeField][LabelWidth(100)]
    [OnValueChanged("SetLightFillPercent")][Range(0,1)]
    private float fillPercent;

    [ColoredFoldoutGroup("Gameplay", 0, 1, 0)][SerializeField][LabelWidth(100)]
    private float fillSpeed;

    [ColoredFoldoutGroup("Gameplay", 0, 1, 0)][SerializeField][LabelWidth(100)]
    [OnValueChanged("TriggerObeliskAnimation")]
    private bool isTriggered;


    private void Start()
    {
        var cb = new UnityAction(TriggerObeliskAnimation);

        var model = this.transform.Find("model");
        for(int i = 0; i < model.childCount; i++)
        {
            var child = model.GetChild(i).gameObject;
            child.GetComponent<NotifyMeOfCollision>().Register(cb);
        }
    }


    [Button("Update Light Materials", ButtonSizes.Large)]
    private void UpdateLightMaterial()
    {
        for (int i = 0; i < sideLights.Length; i++)
        {
            sideLights[i].GetComponent<MeshRenderer>().material = lightMaterial;
        }

        topLight.GetComponent<MeshRenderer>().material = lightMaterial;
        topOuterLight.GetComponent<MeshRenderer>().material = lightMaterial;
    }

    [Button("Update Body Materials", ButtonSizes.Large)]
    private void UpdateBodyMaterials()
    {
        for(int i = 0; i < sectionAObjects.Length; i++)
        {
            sectionAObjects[i].GetComponent<MeshRenderer>().material = sectionAMaterial;
        }

        for(int i = 0; i < sectionBObjects.Length; i++)
        {
            sectionBObjects[i].GetComponent<MeshRenderer>().material = sectionBMaterial;
        }
    }

    private Vector3 Vector3Lerp(Vector3 a, Vector3 b, float percent)
    {
        var result = Vector3.zero;
        result.x = Mathf.Lerp(a.x, b.x, percent);
        result.y = Mathf.Lerp(a.y, b.y, percent);
        result.z = Mathf.Lerp(a.z, b.z, percent);
        return result;
    }

    private void SetLightFillPercent(float percent)
    {
        fillPercent = percent;

        for(int i = 0; i < sideLights.Length; i++)
        {
            var pos = Vector3Lerp(
                  sideLightBottomPositions[i]
                , sideLightTopPositions[i]
                , percent
                );

            sideLights[i].localPosition = pos;
        }

        // center light
        //var topPos = Vector3Lerp(
        //      topLightBottomPosition
        //    , topLightTopPosition
        //    , percent
        //    );
        //
        //topLight.localPosition = topPos;

        // sink in after activation
        //var topOuterPos = Vector3Lerp(
        //      topOuterLightTopPosition
        //    , topOuterLightBottomPosition
        //    , percent
        //    );

        var topOuterPos = Vector3Lerp(
              topOuterLightBottomPosition
            , topOuterLightTopPosition
            , percent
            );
        topOuterLight.localPosition = topOuterPos;
    }

    private void ResetLights()
    {
        isTriggered = false;
        SetLightFillPercent(0.0f);
    }

    private void TriggerObeliskAnimation()
    {
        StartCoroutine(AnimateSideLightsOn());
    }

    IEnumerator AnimateSideLightsOn()
    {
        yield return new WaitForSeconds(0.65f);

        while(fillPercent < 1f)
        {
            fillPercent += fillSpeed;
            SetLightFillPercent(fillPercent);

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

}
