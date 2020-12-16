using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DebugFontSwitcher : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private List<Font> fonts;

    private int idx = 0;

    private void Start()
    {
        titleText.font = fonts[idx];    
    }

    public void OnPlayerMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled ) { return; }
        if (ctx.performed) { return; }


        var xAxis = ctx.ReadValue<Vector2>().x;
        if (xAxis < 0.0f)
        {
            --idx;
            idx = clampIdx(idx, 0, fonts.Count - 1);
        }
        else if (xAxis > 0.0f)
        {
            ++idx;
            idx = clampIdx(idx, 0, fonts.Count - 1);
        }
        else if (xAxis == 0)
        {
            return;
        }

        print("idx=" + idx);
        titleText.font = fonts[idx];
    }

    private int clampIdx(int var, int min, int max)
    {
        if(var < min)
        {
            return min;
        }
        else if (var > max)
        {
            return max;
        }

        return var;
    }



}
