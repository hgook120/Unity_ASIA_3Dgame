﻿using UnityEngine;
using UnityEngine.AI; //引用 人工智慧 API

public class Enemy : MonoBehaviour
{
    [Header("移動速度"), Range(0, 50)]
    public float speed = 3;
    [Header("停止距離"), Range(0, 50)]
    public float stopDistance = 2.5f;
    [Header("攻擊冷卻時間"), Range(0, 50)]
    public float cd = 2f;
    [Header("攻擊中心點")]
    public Transform atkPoint;
    [Header("攻擊長度"), Range(0f, 5f)]
    public float atkLength;

    private Transform player;
    private NavMeshAgent nav;
    private Animator ani;
    /// <summary>
    /// 計時器
    /// </summary>
    private float timer;

    private void Awake()
    {
        //取得身上的物件<代理器>
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        //尋找其他遊戲物件("物件名稱").變形元件
        player = GameObject.Find("A1").transform;
        //代理器 的 速度 與 停止距離
        nav.speed = speed;
        nav.stoppingDistance = stopDistance;
    }

    private void Update()
    {
        Track();
        Attack();
    }

    /// <summary>
    /// 繪製圖示事件 : 僅在 Unity 內顯示
    /// </summary>
    private void OnDrawGizmos()
    {
        //圖示.顏色 = 紅色
        Gizmos.color = Color.red;
        //圖示.繪製射線(中心點，方向)
        //(攻擊中心點的座標，攻擊中心點的前方 * 攻擊長度)
        Gizmos.DrawRay(atkPoint.position, atkPoint.forward * atkLength);
    }

    /// <summary>
    /// 射線擊中的物件
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        if (nav.remainingDistance < stopDistance)
        {
            //時間 累加 (一幀的時間)
            timer += Time.deltaTime;

            //取得玩家的座標
            Vector3 pos = player.position;
            //將玩家的座標Y軸 指定為 本物件的Y軸
            pos.y = transform.position.y;
            //看向(玩家的座標)
            transform.LookAt(pos);

            //如果 計時器 >= 冷卻時間 就攻擊 並且計時器歸零
            if (timer >= cd)
            {
                ani.SetTrigger("attack_switch");
                timer = 0;

                //物理.射線碰撞(攻擊中心點的座標，攻擊中心點的前方，射線擊中的物件，攻擊長度，圖層)
                //圖層:1 << 圖層編號
                //如果 射線 打到物件 就 執行 {   }
                if (Physics.Raycast(atkPoint.position, atkPoint.forward, out hit, atkLength, 1 << 8))
                {
                    //print(hit.collider.name);   ((測試))

                    //碰撞物件.取得元件<玩家>().受傷()
                    hit.collider.GetComponent<Player>().Damage();
                }
            }
        }
    }

    /// <summary>
    /// 追蹤
    /// </summary>
    private void Track()
    {
        //代理器.設定目的地(玩家的座標)
        nav.SetDestination(player.position);

        //print("剩餘的距離:" + nav.remainingDistance);

        //動畫控制器.設定布林值("參數名稱"， 剩餘距離 > 停止距離)
        ani.SetBool("run_switch", nav.remainingDistance > stopDistance);
    }
}
