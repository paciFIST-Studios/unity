using UnityEngine;
using UnityEngine.InputSystem;

using Sirenix.OdinInspector;

public class WheelerPlayerController : MonoBehaviour
{
    // Editor facing vars --------------------------------------------
    [FoldoutGroup("Movement")][HideLabel][SerializeField]
    PIDController pid;

    [FoldoutGroup("Movement")][HideLabel][SerializeField]
    FloatReference hoverForce;
    [FoldoutGroup("Movement")][HideLabel][SerializeField]
    FloatReference hoverHeight;
    [FoldoutGroup("Movement")][HideLabel][SerializeField]
    FloatReference moveForce;


    [FoldoutGroup("Scanner")][HideLabel][SerializeField]
    FloatReference emitCooldown;
    private float lastShotFiredAt;

    [FoldoutGroup("Scanner")][SerializeField]
    ParticleSystem forwardScanParticleSystemPrefab;
    [FoldoutGroup("Scanner")][SerializeField]
    ParticleSystem radialScanParticleSystemPrefab;
    [FoldoutGroup("Scanner")][SerializeField]
    ParticleSystem sphericalScanParticleSystemPrefab;

    [FoldoutGroup("Scanner")][SerializeField]
    Transform particleSystemCarrier;


    // Particle system vars ------------------------------------------

    private ParticleSystem forwardScanParticleSystem;
    private ParticleSystem radialScanParticleSystem;
    private ParticleSystem sphericalScanParticleSystem;

    bool particleSystemRotationIsLocked;

    public enum ScannerType
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

    private bool isScannerActivationLocked = false;
    private bool isRotationLocked = false;

    private Vector2 movementInputThisTick;
    private Vector2 rotateInputThisTick;

    private float zAxisRotation = 0f;


    // Misc vars -------------------------------------------------

    private float screenHalfWidth  = Screen.width  * 0.5f;
    private float screenHalfHeight = Screen.height * 0.5f;


    // Debug fns -----------------------------------------------------

    private void OnGUI()
    {
        // save load
        {
            GUI.Box(new Rect(10, 10, 200, 60), "State Management");
            if(GUI.Button(new Rect(35, 35, 80, 20), "Save"))
            {
                var data = this.BuildSaveData();
                SaveSystem.SavePlayer(data);
            } 
            if(GUI.Button(new Rect(120, 35, 60, 20), "Load"))
            {
                var data = SaveSystem.LoadPlayer();
                this.ApplySaveData(data);
            }
        }

        // scanner switcher
        //{
        //    GUI.Box(new Rect(10, 10, 200, 60), "Scanner Equipped:");
        //
        //    if (currentScanner == ScannerType.ForwardScan)
        //    {
        //        if (GUI.Button(new Rect(30, 35, 160, 30), "ForwardScanner"))
        //        {
        //            SetActiveScanner(ScannerType.RadialScan);
        //        }
        //    }
        //    else if (currentScanner == ScannerType.RadialScan)
        //    {
        //        if (GUI.Button(new Rect(30, 35, 160, 30), "RadialScanner"))
        //        {
        //            SetActiveScanner(ScannerType.SphericalScan);
        //        }
        //    }
        //    else if (currentScanner == ScannerType.SphericalScan)
        //    {
        //        if (GUI.Button(new Rect(30, 35, 160, 30), "SphericalScanner"))
        //        {
        //            SetActiveScanner(ScannerType.ForwardScan);
        //        }
        //    }
        //}

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
        RaycastHit hitInfo;
        Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo, hoverHeight);
        float error = (hitInfo.point.y + hoverHeight) - transform.position.y;

        // Clamp negative error values to zero.  A negative error would snap Wheeler downwards,
        // that that would feel like a sudden gravity spike, which isn't as pleasant as freefall
        error = (error < 0) ? 0.0f : error;

        var hoverCorrection = Vector3.up;
        hoverCorrection *= pid.Update(error);
        hoverCorrection *= hoverForce;
        //correction *= Time.deltaTime;        

        rb.AddForce(hoverCorrection);

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

        if(lastShotFiredAt + emitCooldown < Time.time)
        {
            isFiring = true;
            UnlockParticleSystemRotation();
            //LockPlayerRotation();
        }
    }

    public void OnSetScanner1(InputAction.CallbackContext ctx)
    {
        SetActiveScanner(ScannerType.ForwardScan);
    }

    public void OnSetScanner2(InputAction.CallbackContext ctx)
    {
        SetActiveScanner(ScannerType.RadialScan);
    }

    public void OnSetScanner3(InputAction.CallbackContext ctx)
    {
        SetActiveScanner(ScannerType.SphericalScan);
    }

    public void OnToggleScanner(InputAction.CallbackContext ctx)
    {
        if(currentScanner == ScannerType.ForwardScan)
        {
            SetActiveScanner(ScannerType.RadialScan);
        }
        else if (currentScanner == ScannerType.RadialScan)
        {
            SetActiveScanner(ScannerType.SphericalScan);
        }
        else if (currentScanner == ScannerType.SphericalScan)
        {
            SetActiveScanner(ScannerType.ForwardScan);
        }
    }


    public void OnInitiateDialogue(InputAction.CallbackContext ctx)
    {
    }

    // Player Character Management fns -------------------------------

    void MovePlayerCharacter(Vector2 input)
    {
        var movement = Vector3.right;
        movement.x *= input.x * moveForce;
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
        if(isScannerActivationLocked) { return; }

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

    private void SetActiveScanner(ScannerType scanner)
    {
        if (scanner == ScannerType.ForwardScan)
        {
            radialScanParticleSystem.Stop();
            sphericalScanParticleSystem.Stop();
        }
        else if (scanner == ScannerType.RadialScan)
        {
            forwardScanParticleSystem.Stop();            
            sphericalScanParticleSystem.Stop();
        }
        else if (scanner == ScannerType.SphericalScan)
        {
            forwardScanParticleSystem.Stop();
            radialScanParticleSystem.Stop();
        }

        currentScanner = scanner;
    }

    private void LockScannerActivation()
    {
        isScannerActivationLocked = true;
    }

    private void UnlockScannerActivation()
    {
        isScannerActivationLocked = false;
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


    public void SetDialogueData(DialogueData data)
    {
    }

    public void RemoveDialogueData(DialogueData data)
    {
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


    public PlayerData BuildSaveData()
    {
        var data = new PlayerData
        {
              WheelerPIDController  = this.pid
            , WheelerHoverForce     = this.hoverForce
            , WheelerHoverHeight    = this.hoverHeight
            , WheelerMoveForce      = this.moveForce
            , WheelerPosition       = this.transform.position
            , WheelerCurrentScanner = this.currentScanner
            //, WheelerForwardScan    = this.forwardScanParticleSystem
            //, WheelerRadialScan     = this.radialScanParticleSystem
            //, WheelerSphericalScan  = this.sphericalScanParticleSystem
        };
        return data;
    }

    public void ApplySaveData(PlayerData data)
    {
        this.pid = data.WheelerPIDController;
        this.hoverForce  = data.WheelerHoverForce;
        this.hoverHeight = data.WheelerHoverHeight;
        this.moveForce   = data.WheelerMoveForce;
        this.transform.position = data.WheelerPosition;
        this.currentScanner     = data.WheelerCurrentScanner;
        //this.forwardScanParticleSystem   = data.WheelerForwardScan;
        //this.radialScanParticleSystem    = data.WheelerRadialScan;
        //this.sphericalScanParticleSystem = data.WheelerSphericalScan;
    }

    // ---------------------------------------------------------------
    // ---------------------------------------------------------------





}
