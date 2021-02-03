using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MsgBox : Button
{
    public static MsgBox instance;
    public static bool updatable;
    protected override void Start()
    {
        base.Start();
        instance = this;
        updatable = true;
    }
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        switch (state)
        {
            case SelectionState.Disabled:
                break;
            case SelectionState.Highlighted:
                Debug.Log("MsgHistory enter");
                updatable = false;
                break;
            case SelectionState.Normal:
                Debug.Log("MsgHistory leave");
                updatable = true;
                break;
            case SelectionState.Pressed:
                break;
            default:
                break;
        }
    }
}
