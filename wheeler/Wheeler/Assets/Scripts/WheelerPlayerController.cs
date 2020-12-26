using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class PIDController
{
    // patron saint
    // http://luminaryapps.com/blog/use-a-pid-loop-to-control-unity-game-objects/

    [Tooltip("Proportional constant (counters error)")]
    public float Kp = 0.2f;

    [Tooltip("Integral constant (counters accumulated error)")]
    public float Ki = 0.05f;

    [Tooltip("Derivative constant (fights oscillation)")]
    public float Kd = 1f;

    [Tooltip("current control value")]
    public float value = 0f;

    private float lastError;
    private float integral;


    public float Update(float error)
    {
        return Update(error, Time.deltaTime);
    }

    public float Update(float error, float dt)
    {
        float derivative = (error - lastError) / dt;
        integral += error * dt;
        lastError = error;

        value = Kp * error + Ki * integral + Kd * derivative;
        return value;
    }
}


public class WheelerPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] PIDController pid;

    [SerializeField] float hoverForce = 10f;
    [SerializeField] float targetAltitude = 1f;

    [SerializeField] float groundMovementForce = 100f;

    [Header("Firing")]
    [SerializeField] float fireCooldownTime = 0.1f;
    float lastShotFiredAt;

    [SerializeField] ParticleSystem forwardScanParticleSystemPrefab;
    [SerializeField] ParticleSystem explosionScanParticleSystemPrefab;
    [SerializeField] Transform particleSystemCarrier;


    private ParticleSystem forwardScanParticleSystem;
    private ParticleSystem explosionScanParticleSystem;

    bool particleSystemRotationIsLocked;


    enum ScannerType
    {
          ForwardScan
        , ExplosionScan
    }
    private ScannerType currentScanner;

    private Rigidbody rb;



    private bool isMoving = false;
    private bool isRotating = false;
    private bool isFiring = false;

    private bool isRotationLocked = false;



    private Vector2 movementInputThisTick;
    private Vector2 rotateInputThisTick;

    private float zAxisRotation = 0f;

    private float screenHalfWidth = Screen.width * 0.5f;
    private float screenHalfHeight = Screen.height * 0.5f;


    // Debug ---------------------------------------------------------

    private void OnGUI()
    {
        // scanner switcher
        {
            GUI.Box(new Rect(10, 10, 200, 60), "Scanner Equipped:");

            if (currentScanner == ScannerType.ForwardScan)
            {
                if (GUI.Button(new Rect(30, 35, 160, 30), "ForwardScanner"))
                {
                    ActivateScanner(ScannerType.ExplosionScan);
                }
            }
            else if (currentScanner == ScannerType.ExplosionScan)
            {
                if (GUI.Button(new Rect(30, 35, 160, 30), "ExplosionScanner"))
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
    }



    // ---------------------------------------------------------------

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        forwardScanParticleSystem = Instantiate(forwardScanParticleSystemPrefab, particleSystemCarrier);
        forwardScanParticleSystem.Stop();

        var data = forwardScanParticleSystem.customData;
        data.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        data.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve(0));

        explosionScanParticleSystem = Instantiate(explosionScanParticleSystemPrefab, particleSystemCarrier);
        explosionScanParticleSystem.Stop();

        data = explosionScanParticleSystem.customData;
        data.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
        data.SetVector(ParticleSystemCustomData.Custom1, 0, new ParticleSystem.MinMaxCurve(1));


        currentScanner = ScannerType.ForwardScan;
    }

    void FixedUpdate()
    {
        float currentAltitude = transform.position.y;

        float error = targetAltitude - currentAltitude;
        var correction = Vector3.up;
        correction *= pid.Update(error);
        correction *= hoverForce;
        //correction *= Time.deltaTime;
        
        rb.AddForce(correction);

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
        ActivateScanner(ScannerType.ExplosionScan);
    }

    // ---------------------------------------------------------------

    void MovePlayerCharacter(Vector2 input)
    {
        var movement = Vector3.right;
        movement.x *= input.x * groundMovementForce;
        rb.AddForce(movement);
    }


    void RotatePlayerCharacter(Vector2 input)
    {
        var mouseControl = Mouse.current.position;

        Vector2 screenCoordinate;
        screenCoordinate.x = mouseControl.x.ReadValue();
        screenCoordinate.y = mouseControl.y.ReadValue();

        // recenter to middle of screen
        screenCoordinate.x -= screenHalfWidth;
        screenCoordinate.y -= screenHalfHeight;
        
        float angle = Mathf.Atan2(screenCoordinate.y, screenCoordinate.x) * Mathf.Rad2Deg;

        var rotation = rb.rotation.eulerAngles;
        rotation.z = angle;
        rb.rotation = Quaternion.Euler(rotation);
        zAxisRotation = angle;
    }


    private void FireProjectile()
    {
        if(currentScanner == ScannerType.ForwardScan)
        {
            forwardScanParticleSystem.Play();
        }
        else if (currentScanner == ScannerType.ExplosionScan)
        {
            explosionScanParticleSystem.Play();
        }
    }


    private void ActivateScanner(ScannerType activate)
    {
        if (activate == ScannerType.ForwardScan)
        {
            explosionScanParticleSystem.Stop();
        }
        else if (activate == ScannerType.ExplosionScan)
        {
            forwardScanParticleSystem.Stop();            
        }

        currentScanner = activate;
    }

    private void SetParticleSystemRotation(Quaternion rotation)
    {
        forwardScanParticleSystem.transform.localRotation = rotation;
        explosionScanParticleSystem.transform.localRotation = rotation;
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


    // ---------------------------------------------------------------
    // ---------------------------------------------------------------
    // ---------------------------------------------------------------





}
