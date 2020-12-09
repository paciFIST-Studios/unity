using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoistureConsenserController : MonoBehaviour
{
    [SerializeField] Transform waterLevel;

    // how much the water level will move, from its current position to its 100% full position
    [SerializeField] private Vector3 waterLevelOffsetVector = Vector3.up;

    [Range(0, 1)][SerializeField]float currentWaterFillPercent;

    private Vector3 waterLevelStartPosition;


    [SerializeField] private Light lightBuldLightComponent;
    [SerializeField] private MeshRenderer bulb;
    [SerializeField] private Material  bulbOff;
    [SerializeField] private Material  bulbOn;

    // - Unity Methods --------------------------------------------------------------------

    private void Start()
    {
        waterLevelStartPosition = waterLevel.position;
    }

    private void Update()
    {
        setWaterLevel(currentWaterFillPercent);
    }

    // - Utility --------------------------------------------------------------------------


    void setWaterLevel(float percent)
    {
        Vector3 offset = waterLevelOffsetVector * percent;        
        waterLevel.position = waterLevelStartPosition + offset;

        if(percent == 1.0f)
        {
            setLightOn();
        }
        else
        {
            setLightOff();
        }
    }

    void setLightOn()
    {
        bulb.material = bulbOn;
        lightBuldLightComponent.enabled = true;
    }

    void setLightOff()
    {
        bulb.material = bulbOff;
        lightBuldLightComponent.enabled = false;
    }


}
