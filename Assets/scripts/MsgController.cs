using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class MsgController : MonoBehaviour
{
    public static MsgController instance;
    public Text msgTextPrefab;
    public GameObject msgRoot;
    private MsgBox msgBox;
    public ScrollRect msgHistory;

    private Queue<Text> msgQueue = new Queue<Text>();
    public static int MAXMSGCOUNT = 665;

    private void Start()
    {
        instance = this;
        msgBox = MsgBox.instance;
    }

    public void UpdateMsg(string msg)
    {
        Text newMsg = Instantiate(msgTextPrefab, msgRoot.transform);
        newMsg.text = msg;
        msgQueue.Enqueue(newMsg);
        // show last msg
        if (MsgBox.updatable) { StartCoroutine(UpdateScroll(0f)); }
        if (msgQueue.Count >= MAXMSGCOUNT)
        {
            Destroy(msgQueue.First().gameObject);
            msgQueue.Dequeue();
        }
    }

    IEnumerator UpdateScroll(float pos)
    {
        yield return new WaitForEndOfFrame();
        msgHistory.verticalNormalizedPosition = pos;
    }
}
