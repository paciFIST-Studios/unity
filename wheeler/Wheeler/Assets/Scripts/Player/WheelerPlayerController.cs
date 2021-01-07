using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Sirenix.OdinInspector;

public class WheelerPlayerController : MonoBehaviour
{
    // Editor facing vars --------------------------------------------
    [FoldoutGroup("Movement")][HideLabel][SerializeField]
    private PIDController pid;

    [FoldoutGroup("Movement/Stats")][HideLabel][SerializeField]
    private FloatReference hoverForce;
    [FoldoutGroup("Movement/Stats")][HideLabel][SerializeField]
    private FloatReference hoverHeight;
    [FoldoutGroup("Movement/Stats")][HideLabel][SerializeField]
    private FloatReference moveForce;
    [FoldoutGroup("Movement/Stats")][HideLabel][SerializeField]
    private FloatReference jumpForce;
    [FoldoutGroup("Movement/Stats")][HideLabel][SerializeField]
    private FloatReference jumpChargeRate;
    [FoldoutGroup("Movement/Stats")][SerializeField][Range(0, 1)]
    private float jumpChargePercent;



    [FoldoutGroup("Scanner")][HideLabel][SerializeField]
    private FloatReference emitCooldown;
    private float lastShotFiredAt;

    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private ParticleSystem forwardScanPrefab;
    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private ParticleSystem radialScanPrefab;
    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private ParticleSystem sphericalScanPrefab;
    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private ParticleSystem jumpChargePrefab;
    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private ParticleSystem jumpBlastPrefab;



    [FoldoutGroup("Scanner/Particle System Prefabs")][SerializeField]
    private GameObject particleSystemCarrierPrefab;
    private Transform particleSystemCarrier;


    // Particle system vars ------------------------------------------

    private ParticleSystem forwardScan;
    private ParticleSystem radialScan;
    private ParticleSystem sphericalScan;
    private ParticleSystem jumpCharge;
    private ParticleSystem jumpBlast;
    
    private bool particleSystemRotationIsLocked;

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


    // Inventory System ----------------------------------------------

    List<InventoryItem> inventory = new List<InventoryItem>();


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
    private bool isJumping  = false;
    private bool isChargingJump  = false;

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

        // inventory
        {
            GUI.Box(new Rect(10, 380, 200, 100), "Inventory");
            if(inventory.Count > 0) GUI.Label(new Rect(30, 400, 100, 30),  inventory[0].name);
            if(inventory.Count > 1) GUI.Label(new Rect(30, 420, 100, 60),  inventory[1].name);
            if(inventory.Count > 2) GUI.Label(new Rect(30, 440, 100, 90),  inventory[2].name);
            if(inventory.Count > 3) GUI.Label(new Rect(30, 460, 100, 120), inventory[3].name);
        }
    }


    // Unity Engine fns ----------------------------------------------

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        particleSystemCarrier = Instantiate(particleSystemCarrierPrefab).transform;
        var cc = particleSystemCarrier.GetComponent<WheelerParticleSystemCarrierController>();
        cc.SetTarget(this.transform);

        // Forward system, Berry
        forwardScan = Instantiate(forwardScanPrefab, particleSystemCarrier);
        forwardScan.Stop();
        SetParticleSystemElementType(forwardScan, ElementType.Berry);

        // Radial system, Orange
        radialScan = Instantiate(radialScanPrefab, particleSystemCarrier);
        radialScan.Stop();
        SetParticleSystemElementType(radialScan, ElementType.Orange);

        // spherical system, Lime
        sphericalScan = Instantiate(sphericalScanPrefab, particleSystemCarrier);
        sphericalScan.Stop();
        SetParticleSystemElementType(sphericalScan, ElementType.Lime);

        // charge up particle system
        jumpCharge = Instantiate(jumpChargePrefab, particleSystemCarrier);
        jumpCharge.Stop();
                
        // jump effect system
        jumpBlast = Instantiate(jumpBlastPrefab, particleSystemCarrier);
        jumpBlast.Stop();

        currentScanner = ScannerType.ForwardScan;
    }

    private void FixedUpdate()
    {
        HandlePlayerLevitationUpdate();
        HandlePlayerPhysicsStateUpdate();
    }

    private void Update()
    {
        HandlePlayerSocialStateUpdate();
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

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            if(isChargingJump)
            {
                isJumping = true;
            }

            isChargingJump = false;

            // this kills all particles at once, and it doesn't look
            // great, but it's just kinda what we've got to work with
            jumpCharge.Clear();
            jumpCharge.Stop();
            return;
        }

        isChargingJump = true;
        jumpCharge.Play();
    }

    public void OnScan(InputAction.CallbackContext ctx)
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

    private void HandlePlayerLevitationUpdate()
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
    }

    private void HandlePlayerPhysicsStateUpdate()
    {
        // lateral, rotation, jump
        UpdatePlayerMovementState();
        // scan, carry
        UpdatePlayerInteractionState();
    }

    private void HandlePlayerSocialStateUpdate()
    {
        // talk, hologram,
    }

    private void UpdatePlayerMovementState()
    {
        if (isMoving)
        {
            MovePlayerCharacter(movementInputThisTick);
        }

        if (isRotating && !isRotationLocked)
        {
            RotatePlayerCharacter(rotateInputThisTick);
        }

        if (isChargingJump)
        {
            ChargeJump();
        }

        if (isJumping)
        {
            JumpPlayerCharacter();
        }
    }

    private void UpdatePlayerInteractionState()
    {
        if (isFiring)
        {
            performScan();
        }

        if (!particleSystemRotationIsLocked)
        {
            var euler = transform.localEulerAngles;
            euler.z = zAxisRotation;
            SetParticleSystemRotation(Quaternion.Euler(euler));
        }
    }

    private void MovePlayerCharacter(Vector2 input)
    {
        var movement = Vector3.right;
        movement.x *= input.x * moveForce;
        rb.AddForce(movement);
    }

    private void RotatePlayerCharacter(Vector2 input)
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

        jumpBlast.transform.rotation = rb.rotation;
    }

    private void ChargeJump()
    {
        jumpChargePercent += jumpChargeRate * Time.deltaTime;
        jumpChargePercent = Mathf.Clamp01(jumpChargePercent);
    }

    private void JumpPlayerCharacter()
    {
        var jumpVec = transform.right * -jumpForce * jumpChargePercent;
        rb.AddForce(jumpVec, ForceMode.Impulse);
        jumpBlast.Play();

        jumpChargePercent = 0f;
        isJumping = false;
    }

    private void performScan()
    {
        if(isScannerActivationLocked) { return; }

        if(currentScanner == ScannerType.ForwardScan)
        {
            forwardScan.Play();
        }
        else if (currentScanner == ScannerType.RadialScan)
        {
            radialScan.Play();
        }
        else if (currentScanner == ScannerType.SphericalScan)
        {
            sphericalScan.Play();
        }
    }

    private void SetActiveScanner(ScannerType scanner)
    {
        if (scanner == ScannerType.ForwardScan)
        {
            radialScan.Stop();
            sphericalScan.Stop();
        }
        else if (scanner == ScannerType.RadialScan)
        {
            forwardScan.Stop();            
            sphericalScan.Stop();
        }
        else if (scanner == ScannerType.SphericalScan)
        {
            forwardScan.Stop();
            radialScan.Stop();
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
        forwardScan.transform.localRotation   = rotation;
        radialScan.transform.localRotation    = rotation;
        sphericalScan.transform.localRotation = rotation;
        jumpBlast.transform.localRotation     = rotation;
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

    public void AddInventoryItem(InventoryItem item)
    {
        inventory.Add(item);
    }

    public void RemoveInventoryItem(InventoryItem item)
    {
        if(inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    // Utility fns ---------------------------------------------------

    private InputSource DetermineInputStyle(InputAction.CallbackContext ctx)
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

    private void SetParticleSystemElementType(ParticleSystem ps, ElementType type)
    {       
        var data = ps.customData;
        data.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        data.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve((float)type));
    }


    // Save State ----------------------------------------------------

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





}
