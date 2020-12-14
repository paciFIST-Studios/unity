using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoadManager : MonoBehaviour
{
    [SerializeField] private int loadSceneAtIdx = 1;
    [SerializeField] private float minimumWaitBeforeNextScene = 3f;
    [SerializeField] private float automaticAdvanceSceneAfterSeconds = 91f;

    float startTime = 0f;
    bool advanceScene = false;
    
    private bool isMinimumTimeoutElapsed()
    {
        return startTime + minimumWaitBeforeNextScene <= Time.time;
    }

    private bool isMaximumTimeoutElapsed()
    {
        return startTime + automaticAdvanceSceneAfterSeconds <= Time.time;
    }

    private void Start()
    {
        startTime = Time.time;
        StartCoroutine(LoadNextSceneAsync());
    }
    
    public void PlayerRequestsLoadNextScene(InputAction.CallbackContext ctx)
    {
        if(isMinimumTimeoutElapsed())
        {
            advanceScene = true;
        }
    }

    private IEnumerator LoadNextSceneAsync()
    {
        yield return null;

        var asyncLoad = SceneManager.LoadSceneAsync(loadSceneAtIdx);

        // we have to set this to true, before the load can happen
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // progress stalls out at 90%, if you've disallowed activation
            if (asyncLoad.progress >= 0.9f)
            {
                if (advanceScene || isMaximumTimeoutElapsed() )
                {
                    // setting this to true causes the load to happen next frame
                    asyncLoad.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

}
