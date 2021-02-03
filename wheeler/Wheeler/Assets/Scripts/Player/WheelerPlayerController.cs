using System.Collections.Generic;

using UnityEngine;

using Rewired;

using Sirenix.OdinInspector;


public class WheelerPlayerController : MonoBehaviour
{
    // Rewired Input ------------------------------------------------
    [System.NonSerialized]
    public int playerID = 0;
    // Rewired Player is the input-container
    private Rewired.Player player;
        

    // Editor facing vars --------------------------------------------
    [ColoredFoldoutGroup("Movement", 1, 0, 0)][HideLabel][SerializeField][Required]
    private PIDController pid;


    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][HideLabel][SerializeField][Required]
    private FloatReference hoverForce;
    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][HideLabel][SerializeField][Required]
    private FloatReference hoverHeight;
    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][HideLabel][SerializeField][Required]
    private FloatReference moveForce;
    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][HideLabel][SerializeField][Required]
    private FloatReference jumpForce;
    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][HideLabel][SerializeField][Required]
    private FloatReference jumpChargeRate;
    [ColoredFoldoutGroup("Movement/Stats", 1, 0, 0)][Range(0, 1)][SerializeField][Required]
    private float jumpChargePercent;

    private float minimumJumpChargePercent = 0.3f;

    
    [ColoredFoldoutGroup("Scanner", 0, 1, 1)][HideLabel][SerializeField][Required]
    private FloatReference emitCooldown;
    private float lastShotFiredAt;

    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
    private ParticleSystem forwardScanPrefab;                  
    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
    private ParticleSystem radialScanPrefab;                   
    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
    private ParticleSystem sphericalScanPrefab;                
    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
    private ParticleSystem jumpChargePrefab;                   
    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
    private ParticleSystem jumpBlastPrefab;                    


    [ColoredFoldoutGroup("Scanner/Particle System Prefabs", 0, 1, 1)][SerializeField][Required]
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


    // Menu System ---------------------------------------------------

    [FoldoutGroup("MenuGUI")][SerializeField][Required]
    private WheelerPlayerCharacterMenu playerMenu;

    private bool playerMenuIsActive = false;


    // Inventory System ----------------------------------------------

    List<InventoryItem> inventory = new List<InventoryItem>();


    // Research system -----------------------------------------------

    [FoldoutGroup("ResearchManager")][SerializeField][Required]
    private InventoryManager researchManager;

    float pieResearch = 0.0f;
    float cannisterResearch = 0.0f;


    // Movement vars -------------------------------------------------

    public struct PhysicalState
    {
        public bool IsMoving;
        public bool IsRotating;
        public bool IsRotationLocked;
        public bool IsScanning;
        public bool IsChargingJump;
        public bool IsDoingJump;
    }
    private PhysicalState physicalState;

    private Rigidbody rb;
   
    // todo: add a bitfield here, instead of all these bools
    public struct FrameInput
    {
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool scan;
    }
    private FrameInput inputThisTick;

    private float zAxisRotation = 0f;


    // Misc vars -------------------------------------------------

    private float screenHalfWidth  = Screen.width  * 0.5f;
    private float screenHalfHeight = Screen.height * 0.5f;


    // Debug fns -----------------------------------------------------

    bool drawOnGUI = false;
    private void OnGUI()
    {
        if (!drawOnGUI) return;

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

        #region IMGUI for scanner type
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
        #endregion

        // player stats
        {
            GUI.Box(new Rect(10, 80, 200, 100), "Stats");
            GUI.Label(new Rect(30, 100, 180, 30), string.Format("XYZ: {0}", transform.position));
            GUI.Label(new Rect(30, 120, 180, 30), string.Format("Rotation: {0}", transform.localEulerAngles.z.ToString("#.00")));
            GUI.Label(new Rect(30, 140, 180, 30), string.Format("Angular Velocity.Z: {0}", rb.angularVelocity.z.ToString("#.00")));
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
        //{
        //    GUI.Box(new Rect(10, 280, 200, 100), "Input system");
        //    GUI.Label(new Rect(30, 300, 180, 30), string.Format("Source: {0}", InputSourceString));
        //    GUI.Label(new Rect(30, 320, 180, 30), string.Format("Look: {0}", rotateInputThisTick.ToString()));
        //    GUI.Label(new Rect(30, 340, 180, 30), string.Format("Move: {0}", movementInputThisTick.ToString()));
        //}

        // inventory
        {
            GUI.Box(new Rect(10, 380, 200, 100), "Inventory");
            if(inventory.Count > 0) GUI.Label(new Rect(30, 400, 100, 30),  inventory[0].name);
            if(inventory.Count > 1) GUI.Label(new Rect(30, 420, 100, 60),  inventory[1].name);
            if(inventory.Count > 2) GUI.Label(new Rect(30, 440, 100, 90),  inventory[2].name);
            if(inventory.Count > 3) GUI.Label(new Rect(30, 460, 100, 120), inventory[3].name);
        }

        // data store
        {
            GUI.Box(new Rect(10, 480, 200, 100), "Data");
            GUI.Label(new Rect(30, 500, 100, 30), string.Format($"Pie: {0}", pieResearch));
            GUI.Label(new Rect(30, 520, 100, 30), string.Format($"Cannister: {0}", cannisterResearch));
            //GUI.Label(new Rect(30, 560, 100, 30), string.Format($"Pie: {0}"));
        }
    }

    // Unity Engine fns ----------------------------------------------

    public void Awake()
    {
        player =  ReInput.players.GetPlayer(playerID);
        player.AddInputEventDelegate(OnInput_FixedUpdate, UpdateLoopType.FixedUpdate);
        player.AddInputEventDelegate(OnInput_Update, UpdateLoopType.Update);
    }

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

    private void OnDestroy()
    {
        player.RemoveInputEventDelegate(OnInput_FixedUpdate);
        player.RemoveInputEventDelegate(OnInput_Update);
    }

    private void FixedUpdate()
    {
        DetermineCurrentPlayerState(inputThisTick);

        LevitatePlayerCharacter();

        MovePlayerCharacter(inputThisTick.move);
        RotatePlayerCharacter(inputThisTick.look);
        RotateParticleSystems();

        ChargeJump();
        ReleaseJump();

        PerformScan();
    }

    private void LateUpdate()
    {
        // maybe todo: put input recording here?
        inputThisTick = new FrameInput();
    }

    // Input System Callbacks ----------------------------------------

    private void OnInput_FixedUpdate(InputActionEventData data)
    {
        bool calculateWithMousePosition = false;

        switch (data.actionId)
        {
            case RewiredConsts.Action.Move_Horizontal:
                if (data.GetAxis() != 0)
                {
                    inputThisTick.move.x = data.GetAxis();
                }
                break;

            case RewiredConsts.Action.Move_Vertical:
                if (data.GetAxis() != 0)
                {
                    inputThisTick.move.y = data.GetAxis();
                }
                break;
            
            case RewiredConsts.Action.Look_Horizontal:
                if (data.GetAxisRaw() != 0)
                {
                    if(data.IsCurrentInputSource(ControllerType.Mouse))
                    {
                        calculateWithMousePosition = true;
                    }
                    else
                    {
                        inputThisTick.look.x = data.GetAxisRaw();
                    }
                }
                break;
            
            case RewiredConsts.Action.Look_Vertical:
                if (data.GetAxisRaw() != 0)
                {
                    if (data.IsCurrentInputSource(ControllerType.Mouse))
                    {
                        calculateWithMousePosition = true;
                    }
                    else
                    {
                        inputThisTick.look.y = data.GetAxisRaw();
                    }
                }
                break;
            
            case RewiredConsts.Action.Scan:
                if (data.GetButton())
                {
                    inputThisTick.scan = true;
                }
                break;
            
            case RewiredConsts.Action.Jump:
                if (data.GetButtonSinglePressHold())
                {
                    inputThisTick.jump = true;
                }
                break;
        }

        if(calculateWithMousePosition)
        {
            inputThisTick.look = player.controllers.Mouse.screenPosition;
            inputThisTick.look.x -= screenHalfWidth;
            inputThisTick.look.y -= screenHalfHeight;
            inputThisTick.look = inputThisTick.look.normalized;
        }
    }

    private void OnInput_Update(InputActionEventData data)
    {
        switch(data.actionId)
        {
            case RewiredConsts.Action.Scanner1:
                if(data.GetButtonDown())
                {
                    SetScanner(1);
                }
                break;
            case RewiredConsts.Action.Scanner2:
                if (data.GetButtonDown())
                {
                    SetScanner(2);
                }
                break;
            case RewiredConsts.Action.Scanner3:
                if (data.GetButtonDown())
                {
                    SetScanner(3);
                }
                break;
            case RewiredConsts.Action.NextScanner:
                if (data.GetButtonDown())
                {
                    SetNextScanner();
                }
                break;
            case RewiredConsts.Action.PlayerMenu:
                if (data.GetButtonDown())
                {
                    TogglePlayerMenu();
                }
                break;
            //case RewiredConsts.Action.StartMenu:
            //    // todo
            //    break;
            //case RewiredConsts.Action.Talk:
            //    if (data.GetButtonDown()) { InitiateDialogue(); }
            //    break;
        }

        //PlayerSocialUpdate();
    }
    

    // Player Character Management fns -------------------------------
    
    private void DetermineCurrentPlayerState(FrameInput frame)
    {
        // movement
        physicalState.IsMoving = (frame.move.sqrMagnitude > 0f) ? true : false;

        // rotation
        physicalState.IsRotating = (frame.look.sqrMagnitude > 0f) ? true : false;

        // jumping
        if (physicalState.IsChargingJump)
        {
            // on release
            if (!frame.jump) { physicalState.IsDoingJump = true; }
        }
        else
        {
            if (frame.jump) { physicalState.IsChargingJump = true; }
        }

        // scanning
        physicalState.IsScanning = (frame.scan) ? true : false;
    }

    private void LevitatePlayerCharacter()
    {
        RaycastHit hitInfo;
        Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo, hoverHeight);
        float error = (hitInfo.point.y + hoverHeight) - transform.position.y;

        // Clamp negative error values to zero.  A negative error would snap Wheeler downwards,
        // that that would feel like a sudden gravity spike, which isn't as pleasant as freefall
        error = (error < 0) ? 0.0f : error;

        // if we haven't struck something with a collider, than we're just in the air
        // if we provide an error value while we're just in the air, then the probe won't fall
        if(hitInfo.collider == null) { error = 0.0f; }

        var hoverCorrection = Vector3.up;
        hoverCorrection *= pid.Update(error);
        hoverCorrection *= hoverForce;
        hoverCorrection *= Time.deltaTime;        

        rb.AddForce(hoverCorrection);
    }

    private void MovePlayerCharacter(Vector2 vec)
    {
        // zero out small numbers
        float threshhold = 0.001f;
        if (Mathf.Abs(vec.x) < threshhold) { vec.x = 0.0f; }
        if (Mathf.Abs(vec.y) < threshhold) { vec.y = 0.0f; }

        var movement = Vector3.right;
        movement.x *= vec.x * moveForce * Time.deltaTime;
        rb.AddForce(movement);
    }

    private void RotatePlayerCharacter(Vector2 vec)
    {
        // don't automatically reset the rotation when there is no input
        if (vec.sqrMagnitude == 0f) return;
        
        zAxisRotation = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

        var rotation = rb.rotation.eulerAngles;
        rotation.z = zAxisRotation;
        rb.rotation = Quaternion.Euler(rotation);
        rb.angularVelocity = Vector3.zero;
    }

    private void RotateParticleSystems()
    {
        if (!particleSystemRotationIsLocked)
        {
            var euler = transform.localEulerAngles;
            euler.z = zAxisRotation;
            SetParticleSystemRotation(Quaternion.Euler(euler));
        }
    }

    private void ChargeJump()
    {
        if (!physicalState.IsChargingJump) return;

        if (!jumpCharge.isPlaying) { jumpCharge.Play(); }
        
        jumpChargePercent += jumpChargeRate * Time.deltaTime;
        jumpChargePercent = Mathf.Clamp01(jumpChargePercent);
    }

    private void ReleaseJump()
    {
        if (!physicalState.IsDoingJump) return;

        var jumpVec = transform.right * -jumpForce * jumpChargePercent;
        rb.AddForce(jumpVec, ForceMode.Impulse);
        jumpBlast.Play();

        jumpCharge.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        jumpChargePercent = minimumJumpChargePercent;

        physicalState.IsDoingJump = false;
        physicalState.IsChargingJump = false;
    }

    private void PerformScan()
    {
        if (!physicalState.IsScanning) return;

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

        lastShotFiredAt = Time.time;
        physicalState.IsScanning = false;
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

    private void SetParticleSystemRotation(Quaternion rotation)
    {
        forwardScan.transform.localRotation   = rotation;
        radialScan.transform.localRotation    = rotation;
        sphericalScan.transform.localRotation = rotation;
        jumpBlast.transform.localRotation     = rotation;
    }

    private void SetScanner(int id)
    {
        if (id == 1)
        {
            SetActiveScanner(ScannerType.ForwardScan);
        }
        else if (id == 2)
        {
            SetActiveScanner(ScannerType.RadialScan);

        }
        else if (id == 3)
        {
            SetActiveScanner(ScannerType.SphericalScan);

        }
    }

    public void SetNextScanner()
    {
        if (currentScanner == ScannerType.ForwardScan)
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

    private void TogglePlayerMenu()
    {
        playerMenuIsActive = !playerMenuIsActive;
        print(string.Format($"menuIsActive: {0}", playerMenuIsActive));

        playerMenu.SetMenuVisibility(playerMenuIsActive);
    }


    public void AddInventoryItem(InventoryItem item)
    {
        if(!ItemExistsInInventory(item))
        {
            inventory.Add(item);
            playerMenu.AddInventoryItem(item);
        }

        AddResearch(item);
    }

    public void RemoveInventoryItem(InventoryItem item)
    {
        if(inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    public void AddResearch(InventoryItem item)
    {
        if(item.name == "PetrifiedPie")
        {
            pieResearch += 1.0f;
            print("pie research:" + pieResearch);
        }
        else if (item.name == "EmptyCannister")
        {
            cannisterResearch += 1.0f;
            print("cannister research: " + cannisterResearch);
        }
    }

    // Utility fns ---------------------------------------------------

    private void SetParticleSystemElementType(ParticleSystem ps, ElementType type)
    {       
        var data = ps.customData;
        data.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        data.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve((float)type));
    }

    private bool ItemExistsInInventory(InventoryItem item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].name == item.name)
            {
                return true;
            }
        }

        return false;
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
            , WheelerInventory      = this.inventory.ToArray()
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

        this.inventory = new List<InventoryItem>(data.WheelerInventory);
    }

    // ---------------------------------------------------------------

}
