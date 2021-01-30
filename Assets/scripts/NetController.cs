using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Dictionary<string, EnemyController> enemys = new Dictionary<string, EnemyController>();

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
        NetManager.Connect("127.0.0.1", 1234);

        player.desc = NetManager.GetDesc();
        player.transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

        sendStr = "Enter|" + player.desc + "," +
            player.transform.position.x + "," +
            player.transform.position.y + "," +
            player.moveDirection.x + "," +
            player.moveDirection.y + ",";

        NetManager.Send(sendStr);

        StartCoroutine("Waitfor");
        //NetManager.Send("List|");
    }

    IEnumerator Waitfor()
    {
        yield return new WaitForSeconds(1f);
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

        if (player.getFireFlag())
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
                + player.hitdesc + ","
                );
        }
    }

    void OnList(string msgArgs)
    {
        Debug.Log("OnList " + msgArgs);
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 5;
        for(int i = 0; i < count; i++)
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
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.transform.position = new Vector3(posX, posY, 0);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.moveDirection = new Vector2(dirX, dirY);
        enemyController.desc = desc;
        enemys.Add(desc, enemyController);
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
        enemys[desc].Move();
    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        if (!enemys.ContainsKey(desc)) return;
        Destroy(enemys[desc].gameObject);
        enemys.Remove(desc);
    }

    void OnFire(string msg)
    {
        Debug.Log("OnFire " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float posx = float.Parse(split[1]);
        float posy = float.Parse(split[2]);
        short dirx = short.Parse(split[3]);
        short diry = short.Parse(split[4]);

        if (!enemys.ContainsKey(desc)) return;
        //enemys[desc].transform.position = new Vector3(posx, posx, 0);
        enemys[desc].moveDirection = new Vector2(dirx, diry);
        enemys[desc].Fire();
    }
    void OnHit(string msg)
    {
        Debug.Log("OnHit " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        string hitdesc = split[1];
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
    }
}
