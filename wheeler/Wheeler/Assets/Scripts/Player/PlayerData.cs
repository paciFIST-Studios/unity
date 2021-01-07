
using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class PlayerData : SerializedScriptableObject
{
    public FloatReference WheelerHoverForce;
    public FloatReference WheelerHoverHeight;
    public FloatReference WheelerMoveForce;
    public PIDController WheelerPIDController;

    public Vector3 WheelerPosition;

    public WheelerPlayerController.ScannerType WheelerCurrentScanner;

    public ParticleSystem WheelerForwardScan;
    public ParticleSystem WheelerRadialScan;
    public ParticleSystem WheelerSphericalScan;

    public InventoryItem[] WheelerInventory;
}
