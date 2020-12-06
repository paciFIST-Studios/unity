using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RocketController : MonoBehaviour
{
    private     Rigidbody    rigidBody;
    private     AudioSource  audioSource;
    private     MeshRenderer coneMeshRenderer;

    // used to track start and stop of audio
    private     bool        playAudio       = false;

    // booster applies this force, per frame, when active (scaled against time.deltaTime)
    private     float       boostForce      = 500f;
    // rotation applies this force, per frame, when active (scaled against time.deltaTime)
    private     float       rotationForce   = 100f;


    [System.Serializable]
    struct RocketSettings
    {
        [SerializeField]
        public float mass;
        [SerializeField]
        public float boostForce;
        [SerializeField]
        public float rotationForce;
        [SerializeField]
        public Material material;
    }

    [SerializeField]
    List<RocketSettings> rocketSettings = new List<RocketSettings>();

    private int currentRocketIdx = 0;

    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();

        // the mesh render of the "cone" object is the specific one that holds color
        Transform cone = this.transform.Find("cone");
        coneMeshRenderer = cone.GetComponent<MeshRenderer>();

        SetCurrentRocketIndex(0);
    }

    void Update ()
    {
        ProcessDebugInput();

        HandleThrustInput();
        HandleRotationInput();
        HandleAudioChanges();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;

            default:
                print("boom");
                break;
        }
    }

    private void SetCurrentRocketIndex(int idx)
    {
        Assert.IsTrue(idx < rocketSettings.Count);
        Assert.IsTrue(idx >= 0);

        boostForce = rocketSettings[idx].boostForce;
        rotationForce = rocketSettings[idx].rotationForce;
        coneMeshRenderer.material = rocketSettings[idx].material;
    }

    private void ProcessDebugInput()
    {
        // update boost forces
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            boostForce += 50f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            boostForce -= 50f;
        }

        // switch between different rocket configurations
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentRocketIdx--;
            if (currentRocketIdx < 0)
            {
                currentRocketIdx = 0;
            }
            SetCurrentRocketIndex(currentRocketIdx);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRocketIdx++;
            if (currentRocketIdx > rocketSettings.Count - 1)
            {
                currentRocketIdx = rocketSettings.Count - 1;
            }
            SetCurrentRocketIndex(currentRocketIdx);
        }
    }

    private void HandleAudioChanges()
    {
        if (playAudio && !audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.mute = false;
        }

        if (!playAudio && audioSource.isPlaying)
        {
            audioSource.mute = true;
            audioSource.Stop();
        }
    }

    private void HandleThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // using rocket's coordinate system
            rigidBody.AddRelativeForce(Vector3.up * boostForce * Time.deltaTime);
            playAudio = true;
        }
        else
        {
            playAudio = false;
        }
    }

    private void HandleRotationInput()
    {
        rigidBody.freezeRotation = true; // start manual rotation control

        // left rotate has precedence 
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationForce * Time.deltaTime);
        }

        rigidBody.freezeRotation = false; // resume physics rotation control
    }


}
