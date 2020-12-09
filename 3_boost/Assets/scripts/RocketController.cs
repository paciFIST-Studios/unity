using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour
{
    private     Rigidbody    rigidBody;
    private     AudioSource  audioSource;
    private     MeshRenderer coneMeshRenderer;

    private bool debugCollisionIsOn = true;

    // used to track start and stop of audio
    private     bool        playAudio       = false;

    // booster applies this force, per frame, when active (scaled against time.deltaTime)
    private     float       boostForce      = 500f;
    // rotation applies this force, per frame, when active (scaled against time.deltaTime)
    private     float       rotationForce   = 100f;
    

    [System.Serializable]
    struct RocketSettings
    {
        [SerializeField] public float mass;
        [SerializeField] public float boostForce;
        [SerializeField] public float rotationForce;
        [SerializeField] public Material material;
    }

    private int currentRocketIdx = 0;
    [SerializeField] List<RocketSettings> rocketSettings = new List<RocketSettings>();
    [SerializeField] private Material brokenRocketMaterial;

    [System.Serializable]
    struct AudioForRocket
    {
        [SerializeField] public AudioClip thrust;
        [SerializeField] public AudioClip success;
        [SerializeField] public AudioClip explode;
    }

    [SerializeField]
    AudioForRocket audioClips;

    [System.Serializable]
    struct ParticleSystems
    {
        [SerializeField] public ParticleSystem[] engineParticles;
        [SerializeField] public ParticleSystem   successParticles;
        [SerializeField] public ParticleSystem   deathParticles;
    }

    [SerializeField] ParticleSystems particleSystems;



    enum PlayerState { Alive, Dying, LevelTransition }
    private PlayerState playerState;

    private int currentSceneIdx = 0;

    // - Unity Methods --------------------------------------------------------------------

    private void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();

        // the mesh render of the "cone" object is the specific one that holds color
        Transform cone = this.transform.Find("cone");
        coneMeshRenderer = cone.GetComponent<MeshRenderer>();

        SetCurrentRocketIndex(0);
        playerState = PlayerState.Alive;

        // the load-next-scene fn automatically increments the sceneIdx, so
        // preparing for that, means to point at the current scene
        currentSceneIdx = SceneManager.GetActiveScene().buildIndex;
    }

    void Update ()
    {
        ProcessDebugInput();

        if(playerState == PlayerState.Alive)
        {
            HandleThrustInput();
            HandleRotationInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerState != PlayerState.Alive) { return; }
        if(!debugCollisionIsOn) { return; }

        print("Collision");


        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Start":
                break;

            case "End":
                PerformEndLevelSequence();
                break;

            case "Destruct":
                print("Destructible Object Hit!");
                collision.gameObject.GetComponent<CallDestructOnRootParent>().Destruct();
                PerformPlayerDeathSequence();
                break;

            default:
                PerformPlayerDeathSequence();
                break;
        }
    }

    private void OnGUI()
    {
        if(!debugCollisionIsOn)
        {
            GUI.Label(new Rect(0, 0, 200, 100), new GUIContent("debugCollisionOn=" + debugCollisionIsOn.ToString()));
        }
    }

    // - Utility --------------------------------------------------------------------------

    private void SetCurrentRocketIndex(int idx)
    {
        Assert.IsTrue(idx < rocketSettings.Count);
        Assert.IsTrue(idx >= 0);

        boostForce = rocketSettings[idx].boostForce;
        rotationForce = rocketSettings[idx].rotationForce;
        coneMeshRenderer.material = rocketSettings[idx].material;
    }
      
    private void SetRocketToDead()
    {
        foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = brokenRocketMaterial;
        }
    }

    private void LoadSceneAfterSeconds(int sceneIdx, float seconds = 1.0f)
    {
        if (sceneIdx == SceneManager.sceneCountInBuildSettings)
        {
            currentSceneIdx = 0;
        }
        else
        {
            currentSceneIdx = sceneIdx;
        }

        Invoke("LoadNextScene", seconds);
    }

    private void LoadNextScene()
    {
        print("Loading Scene idx=" + currentSceneIdx);
        SceneManager.LoadScene(currentSceneIdx);
    }

    private void PlayAudioClip(AudioClip clip, bool loop = false, bool allowLayering = false)
    {
        if (audioSource.isPlaying && !allowLayering)
        {
            return;
        }

        audioSource.mute = false;

        if(loop)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void StopAudio()
    {
        audioSource.mute = true;
        audioSource.Stop();
    }

    private void PlayEngineParticles()
    {
        particleSystems.engineParticles[0].Play();
        particleSystems.engineParticles[1].Play();
    }

    private void StopEngineParticles()
    {
        particleSystems.engineParticles[0].Stop();
        particleSystems.engineParticles[1].Stop();
    }

    // - Rocket Control -------------------------------------------------------------------

    private void ProcessDebugInput()
    {
        if(!Debug.isDebugBuild) { return; }


        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadSceneAfterSeconds(0);
        }
        else  if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadSceneAfterSeconds(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadSceneAfterSeconds(2);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            debugCollisionIsOn = !debugCollisionIsOn;
        }

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
   

    private void HandleThrustInput()
    {
        if (Input.GetButton("Boost"))
        {
            // using rocket's coordinate system
            rigidBody.AddRelativeForce(Vector3.up * boostForce * Time.deltaTime);
            PlayAudioClip(audioClips.thrust);
            PlayEngineParticles();
        }
        else
        {
            StopAudio();
            StopEngineParticles();
        }
    }

    private void HandleRotationInput()
    {
        rigidBody.angularVelocity = Vector3.zero;

        if (Input.GetButton("RotateLeft"))
        {
            transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
        }
        else if (Input.GetButton("RotateRight"))
        {
            transform.Rotate(-Vector3.forward * rotationForce * Time.deltaTime);
        }
    }


    private void PerformEndLevelSequence()
    {
        StopAudio();
        StopEngineParticles();
        playerState = PlayerState.LevelTransition;
        PlayAudioClip(audioClips.success);
        particleSystems.successParticles.Play();
        LoadSceneAfterSeconds(++currentSceneIdx, 1f);
    }

    private void PerformPlayerDeathSequence()
    {
        StopAudio();
        StopEngineParticles();
        playerState = PlayerState.Dying;
        particleSystems.deathParticles.Play();
        PlayAudioClip(audioClips.explode);
        SetRocketToDead();
        LoadSceneAfterSeconds(currentSceneIdx, 3f);
    }

}
