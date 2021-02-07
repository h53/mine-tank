using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MsgBox : Button
{
    public static MsgBox instance;
    public static bool updatable;
    private Image mask;
    private Color toColor;
    private Color originColor;
    protected override void Awake()
    {
        base.Start();
        instance = this;
        updatable = true;
        mask = this.gameObject.GetComponent<Image>();
        originColor = mask.color;
        toColor = new Color(mask.color.r, mask.color.g, mask.color.b, 0.1f);
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
                mask.color = toColor;
                updatable = false;
                break;
            case SelectionState.Normal:
                Debug.Log("MsgHistory leave");
                mask.color = originColor;
                updatable = true;
                break;
            case SelectionState.Pressed:
                break;
            default:
                break;
        }
    }
}
