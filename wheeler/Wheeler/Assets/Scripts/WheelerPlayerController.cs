﻿using UnityEngine;
using UnityEngine.InputSystem;

public class WheelerPlayerController : MonoBehaviour
{
    // Editor facing vars --------------------------------------------

    [Header("Movement")]
    [SerializeField] PIDController pid;

    [SerializeField] float hoverForce = 10f;
    [SerializeField] float targetAltitude = 1f;

    [SerializeField] float groundMovementForce = 100f;


    [Header("Firing")]
    [SerializeField] float fireCooldownTime = 0.1f;
    float lastShotFiredAt;

    [SerializeField] ParticleSystem forwardScanParticleSystemPrefab;
    [SerializeField] ParticleSystem radialScanParticleSystemPrefab;
    [SerializeField] ParticleSystem sphericalScanParticleSystemPrefab;

    [SerializeField] Transform particleSystemCarrier;


    // Particle system vars ------------------------------------------

    private ParticleSystem forwardScanParticleSystem;
    private ParticleSystem radialScanParticleSystem;
    private ParticleSystem sphericalScanParticleSystem;

    bool particleSystemRotationIsLocked;

    enum ScannerType
    {
          ForwardScan
        , RadialScan
        , SphericalScan
    }
    private ScannerType currentScanner;

    public enum ElementType
    {
          Berry     = 0
        , Orange    = 1
        , Lime      = 2
        , Grape     = 3
    }


    // Input vars ----------------------------------------------------

    public enum InputSource
    {
          Unknown
        , MouseAndKeyboard
        , XboxController
    }
    private InputSource inputSource;
    private string InputSourceString = "Mouse+Keyboard";
    private string MKInput = "Mouse+Keyboard";
    private string XBInput = "Xbox Controller";


    // Movement vars -------------------------------------------------

    private Rigidbody rb;
    
    private bool isMoving   = false;
    private bool isRotating = false;
    private bool isFiring   = false;

    private bool isRotationLocked = false;

    private Vector2 movementInputThisTick;
    private Vector2 rotateInputThisTick;

    private float zAxisRotation = 0f;


    // Misc vars -------------------------------------------------

    private float screenHalfWidth = Screen.width * 0.5f;
    private float screenHalfHeight = Screen.height * 0.5f;


    // Debug fns -----------------------------------------------------

    private void OnGUI()
    {
        // scanner switcher
        {
            GUI.Box(new Rect(10, 10, 200, 60), "Scanner Equipped:");

            if (currentScanner == ScannerType.ForwardScan)
            {
                if (GUI.Button(new Rect(30, 35, 160, 30), "ForwardScanner"))
                {
                    ActivateScanner(ScannerType.RadialScan);
                }
            }
            else if (currentScanner == ScannerType.RadialScan)
            {
                if (GUI.Button(new Rect(30, 35, 160, 30), "RadialScanner"))
                {
                    ActivateScanner(ScannerType.SphericalScan);
                }
            }
            else if (currentScanner == ScannerType.SphericalScan)
            {
                if (GUI.Button(new Rect(30, 35, 160, 30), "SphericalScanner"))
                {
                    ActivateScanner(ScannerType.ForwardScan);
                }
            }
        }

        // player stats
        {
            GUI.Box(new Rect(10, 80, 200, 100), "Stats");
            GUI.Label(new Rect(30, 100, 180, 30), string.Format("XYZ: {0}", transform.position));
            GUI.Label(new Rect(30, 120, 180, 30), string.Format("Rotation: {0}", transform.localEulerAngles.z.ToString("#.00")));
            GUI.Label(new Rect(30, 140, 180, 30), "");
            GUI.Label(new Rect(30, 160, 180, 30), "");
        }

        // particle system
        {
            GUI.Box(new Rect(10, 180, 200, 100), "Particle System");
            GUI.Label(new Rect(30, 200, 180, 30), string.Format("Name: {0}", transform.position));
            GUI.Label(new Rect(30, 220, 180, 30), string.Format("Rotation: {0}", transform.localEulerAngles.z.ToString("#.00")));
            GUI.Label(new Rect(30, 240, 180, 30), "");
            GUI.Label(new Rect(30, 260, 180, 30), "");
        }

        // input system
        {
            GUI.Box(new Rect(10, 280, 200, 100), "Input system");
            GUI.Label(new Rect(30, 300, 180, 30), string.Format("Source: {0}", InputSourceString));
            GUI.Label(new Rect(30, 320, 180, 30), string.Format("Look: {0}", rotateInputThisTick.ToString()));
            GUI.Label(new Rect(30, 340, 180, 30), string.Format("Move: {0}", movementInputThisTick.ToString()));
        }
    }


    // Unity Engine fns ----------------------------------------------

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        // Forward system, Berry
        forwardScanParticleSystem = Instantiate(forwardScanParticleSystemPrefab, particleSystemCarrier);
        forwardScanParticleSystem.Stop();
        SetParticleSystemElementType(forwardScanParticleSystem, ElementType.Berry);

        // Radial system, Orange
        radialScanParticleSystem = Instantiate(radialScanParticleSystemPrefab, particleSystemCarrier);
        radialScanParticleSystem.Stop();
        SetParticleSystemElementType(radialScanParticleSystem, ElementType.Orange);

        // spherical system, Lime
        sphericalScanParticleSystem = Instantiate(sphericalScanParticleSystemPrefab, particleSystemCarrier);
        sphericalScanParticleSystem.Stop();
        SetParticleSystemElementType(sphericalScanParticleSystem, ElementType.Lime);

        currentScanner = ScannerType.ForwardScan;
    }

    void FixedUpdate()
    {
        float currentAltitude = transform.position.y;

        float error = targetAltitude - currentAltitude;
        var hoveCorrection = Vector3.up;
        hoveCorrection *= pid.Update(error);
        hoveCorrection *= hoverForce;
        //correction *= Time.deltaTime;
        
        rb.AddForce(hoveCorrection);

        if(isMoving)
        {
            MovePlayerCharacter(movementInputThisTick);
        }

        if(isRotating && !isRotationLocked)
        {
            RotatePlayerCharacter(rotateInputThisTick);
        }

        if (isFiring)
        {
            FireProjectile();
        }

        if(!particleSystemRotationIsLocked)
        {
            var euler = transform.localEulerAngles;
            euler.z = zAxisRotation;
            SetParticleSystemRotation(Quaternion.Euler(euler));
        }
    }


    // Input System Callbacks ----------------------------------------

    public void OnLook(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            isRotating = false;
            return;
        }

        isRotating = true;
        rotateInputThisTick = ctx.ReadValue<Vector2>();

        UpdateInputSource(ctx);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            isMoving = false;
            return;
        }

        isMoving = true;
        movementInputThisTick = ctx.ReadValue<Vector2>();

        UpdateInputSource(ctx);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            isFiring = false;
            LockParticleSystemRotation();
            //UnlockPlayerRotation();
            return;
        }

        if(lastShotFiredAt + fireCooldownTime < Time.time)
        {
            isFiring = true;
            UnlockParticleSystemRotation();
            //LockPlayerRotation();
        }
    }

    public void OnSetScanner1(InputAction.CallbackContext ctx)
    {
        ActivateScanner(ScannerType.ForwardScan);
    }

    public void OnSetScanner2(InputAction.CallbackContext ctx)
    {
        ActivateScanner(ScannerType.RadialScan);
    }

    public void OnSetScanner3(InputAction.CallbackContext ctx)
    {
        ActivateScanner(ScannerType.SphericalScan);
    }

    public void OnToggleScanner(InputAction.CallbackContext ctx)
    {
        if(currentScanner == ScannerType.ForwardScan)
        {
            ActivateScanner(ScannerType.RadialScan);
        }
        else if (currentScanner == ScannerType.RadialScan)
        {
            ActivateScanner(ScannerType.SphericalScan);
        }
        else if (currentScanner == ScannerType.SphericalScan)
        {
            ActivateScanner(ScannerType.ForwardScan);
        }
    }


    // Player Character Management fns -------------------------------

    void MovePlayerCharacter(Vector2 input)
    {
        var movement = Vector3.right;
        movement.x *= input.x * groundMovementForce;
        rb.AddForce(movement);
    }

    void RotatePlayerCharacter(Vector2 input)
    {
        if (inputSource == InputSource.Unknown)
        {
            return;
        }
        else if (inputSource == InputSource.MouseAndKeyboard)
        {
            // recenter to middle of screen
            input.x -= screenHalfWidth;
            input.y -= screenHalfHeight;
        }
        
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        var rotation = rb.rotation.eulerAngles;
        rotation.z = angle;
        rb.rotation = Quaternion.Euler(rotation);
        zAxisRotation = angle; // cache value
    }

    private void FireProjectile()
    {
        if(currentScanner == ScannerType.ForwardScan)
        {
            forwardScanParticleSystem.Play();
        }
        else if (currentScanner == ScannerType.RadialScan)
        {
            radialScanParticleSystem.Play();
        }
        else if (currentScanner == ScannerType.SphericalScan)
        {
            sphericalScanParticleSystem.Play();
        }
    }

    private void ActivateScanner(ScannerType activate)
    {
        if (activate == ScannerType.ForwardScan)
        {
            radialScanParticleSystem.Stop();
            sphericalScanParticleSystem.Stop();
        }
        else if (activate == ScannerType.RadialScan)
        {
            forwardScanParticleSystem.Stop();            
            sphericalScanParticleSystem.Stop();
        }
        else if (activate == ScannerType.SphericalScan)
        {
            forwardScanParticleSystem.Stop();
            radialScanParticleSystem.Stop();
        }

        currentScanner = activate;
    }

    private void SetParticleSystemRotation(Quaternion rotation)
    {
        forwardScanParticleSystem.transform.localRotation   = rotation;
        radialScanParticleSystem.transform.localRotation    = rotation;
        sphericalScanParticleSystem.transform.localRotation = rotation;
    }

    private void LockParticleSystemRotation()
    {
        particleSystemRotationIsLocked = true;
    }

    private void UnlockParticleSystemRotation()
    {
        particleSystemRotationIsLocked = false;
    }

    private void LockPlayerRotation()
    {
        isRotationLocked = true;
    }

    private void UnlockPlayerRotation()
    {
        isRotationLocked = false;
    }

    private void UpdateInputSource(InputAction.CallbackContext ctx)
    {
        inputSource = DetermineInputStyle(ctx);
        if (inputSource == InputSource.MouseAndKeyboard)
        {
            InputSourceString = MKInput;
        }
        else if (inputSource == InputSource.XboxController)
        {
            InputSourceString = XBInput;
        }
        else
        {
            InputSourceString = "";
        }
    }


    // Utility fns ---------------------------------------------------

    InputSource DetermineInputStyle(InputAction.CallbackContext ctx)
    {
        // ellie:todo: someday, change this, it's fragile
        var str = ctx.action.GetBindingDisplayString();

        if(str.Contains("LS") || str.Contains("RS"))
        {
            return InputSource.XboxController;
        }
        else if (str.Contains("Position"))
        {
            return InputSource.MouseAndKeyboard;
        }
        else if (str.Contains("W | Up | S | Down | A | Left | D | Right"))
        {
            return InputSource.MouseAndKeyboard;
        }
        else
        {
            return InputSource.Unknown;
        }
    }
    
    void SetParticleSystemElementType(ParticleSystem ps, ElementType type)
    {       
        var data = ps.customData;
        data.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        data.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve((float)type));
    }


    // ---------------------------------------------------------------
    // ---------------------------------------------------------------





}
