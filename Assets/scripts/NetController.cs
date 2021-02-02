using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Text msgTextPrefab;
    public GameObject msgRoot;
    public ScrollRect msgHistory;
    public Dictionary<string, EnemyController> enemys = new Dictionary<string, EnemyController>();
    private Queue<Text> msgQueue = new Queue<Text>();
    public const int MAXMSGCOUNT = 665;

    private PlayerController player;
    private string sendStr;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;

        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Fire", OnFire);
        NetManager.AddListener("Hit", OnHit);
        NetManager.AddListener("Tip", OnTip);
        NetManager.AddListener("Text", OnText);
        NetManager.Connect(GlobalVars.serverip, GlobalVars.port);

        player.desc = NetManager.GetDesc();
        player.transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

        sendStr = "Enter|" + player.desc + "," +
            player.transform.position.x + "," +
            player.transform.position.y + "," +
            player.moveDirection.x + "," +
            player.moveDirection.y + ",";

        NetManager.Send(sendStr);

        NetManager.Send("List|");
    }

    private void Update()
    {
        NetManager.Update();

        if (player.moveDirection.x != 0 || player.moveDirection.y != 0)
        {
            NetManager.Send("Move|" + player.desc + ","
                + player.transform.position.x + ","
                + player.transform.position.y + ","
                + player.moveDirection.x + ","
                + player.moveDirection.y + ","
                );
        }

        if (player.fireFlag)
        {
            NetManager.Send("Fire|" + player.desc + ","
                + player.transform.position.x + ","
                + player.transform.position.y + ","
                + player.moveDirection.x + ","
                + player.moveDirection.y + ","
                );
        }

        if (!player.hitdesc.Equals(""))
        {
            NetManager.Send("Hit|" + player.desc + ","
                + player.hitdesc
                );
        }
    }
    
    void OnList(string msgArgs)
    {
        Debug.Log("OnList " + msgArgs);
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 5;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 5 + 0];
            float posX = float.Parse(split[i * 5 + 1]);
            float posY = float.Parse(split[i * 5 + 2]);
            short dirX = short.Parse(split[i * 5 + 3]);
            short dirY = short.Parse(split[i * 5 + 4]);

            if (desc == player.desc) continue;

            AddEnemy(posX, posY, dirX, dirY, desc);
        }
    }
    void OnEnter(string msg)
    {
        Debug.Log("OnEnter " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float posx = float.Parse(split[1]);
        float posy = float.Parse(split[2]);
        short dirx = short.Parse(split[3]);
        short diry = short.Parse(split[4]);

        if (desc == player.desc) return;

        // add enemy
        AddEnemy(posx, posy, dirx, diry, desc);
    }

    void AddEnemy(float posX, float posY,short dirX,short dirY, string desc)
    {
        Vector3 angle = new Vector3(0, 0, -(dirX + (dirY == 0 ? 0 : dirY - 1)) * NUM.DIRECTION_ANGLE);
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(posX, posY, 0), Quaternion.Euler(angle));
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.desc = desc;
        enemys.Add(desc, enemyController);
        GlobalVars.onLineNum++;
    }

    void OnMove(string msg)
    {
        Debug.Log("OnMove " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float posx = float.Parse(split[1]);
        float posy = float.Parse(split[2]);
        short dirx = short.Parse(split[3]);
        short diry = short.Parse(split[4]);

        if (!enemys.ContainsKey(desc)) return;
        //enemys[desc].transform.position = new Vector3(posx, posx, 0);
        enemys[desc].moveDirection = new Vector2(dirx, diry);
        enemys[desc].position = new Vector2(posx, posy);
        //enemys[desc].Move();
    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        if (!enemys.ContainsKey(desc)) return;
        Destroy(enemys[desc].gameObject);
        enemys.Remove(desc);
        GlobalVars.onLineNum--;
    }

    void OnFire(string msg)
    {
        Debug.Log("OnFire " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        //float posx = float.Parse(split[1]);
        //float posy = float.Parse(split[2]);
        short dirx = short.Parse(split[3]);
        short diry = short.Parse(split[4]);

        if (!enemys.ContainsKey(desc)) return;
        //enemys[desc].transform.position = new Vector3(posx, posx, 0);
        enemys[desc].moveDirection = new Vector2(dirx, diry);
        enemys[desc].fireFlag = true;
        //enemys[desc].Fire();
    }
    void OnHit(string msg)
    {
        Debug.Log("OnHit " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        //string hitdesc = split[1];
        if (desc == player.desc)
        {
            Debug.LogWarning("you fail");
            Destroy(player.gameObject);
            Destroy(this.gameObject);
            GameController.instance.isOver = true;
        }
        if (!enemys.ContainsKey(desc)) return;
        Destroy(enemys[desc].gameObject);
        enemys.Remove(desc);
        GlobalVars.onLineNum--;
    }

    void OnTip(string msg)
    {
        Debug.Log("OnTip " + msg);
        string[] split = msg.Split(',');
        string tip = split[0];
        GameController.instance.ShowTip(tip, 2f);
    }

    void OnText(string msg)
    {
        Debug.Log("OnText " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        string info = split[1];
        Text newMsg = Instantiate(msgTextPrefab, msgRoot.transform);
        newMsg.text = info;
        msgQueue.Enqueue(newMsg);
        // show last msg
        StartCoroutine(UpdateScroll(0f));
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
