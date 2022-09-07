using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Actor
{
    [Space(20)]
    [Header("TagとLayerの指定を確認してください")]
    [Space(20)]
    [SerializeField] GameObject[] explodes = default;
    private Rigidbody2D rigid;
    private Transform playerform;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    protected void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        playerform = GameObject.FindWithTag("Player").transform;
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public delegate void OnDeathNotifyer(Enemy enemy);
    public OnDeathNotifyer OnDeathNotifyerHandler;


    //------------------------------------------
    // 継承先共有関数
    //------------------------------------------
    protected Rigidbody2D Rigid => rigid;
    protected void ForceToPlayer(float speed, float gravity = 0)
    {
        if (playerform != null)
        {
            var force = GetDirection();
            if (gravity != 0)
            {
                force.y = gravity;
            }
            rigid.velocity = force * speed;
        }
    }
    protected void RotateFwd()
    {
        if (playerform != null)
        {
            var force = GetDirection();
            float side = Mathf.Clamp(force.x, -1, 1);
            if (side < 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (side > 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
    protected void GapUpdateOnFire(ref float time, float gap, float speed, float power)
    {
        if(time >= gap)
        {
            var dir = GetDirection();
            var rotate = Quaternion.FromToRotation(Vector2.right, dir);
            ExcuteBullet(transform, rotate, dir * speed, power);
            time = 0;
        }
        time += Time.deltaTime;
    }
    protected Vector2 GetDirection()
    {
        if (playerform != null)
        {
            var targetform = playerform.transform.position;
            var selfform = transform.position;
            var direction = (targetform - selfform).normalized;
            return direction;
        }
        return default;
    }
    protected bool IsInDistance(float maxDistance)
    {
        if (playerform != null)
        {
            float distance = Vector2.Distance(transform.position, playerform.position);
            if (distance <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }
    protected void RequestDeathEffect()
    {
        StartCoroutine(IDeathEffect());
    }


    //------------------------------------------
    // 内部共有関数
    //------------------------------------------
    private IEnumerator IDeathEffect()
    {
        if(explodes != null)
        {
            float gap = 0.3f;
            var wait = new WaitForSeconds(gap);
            float duration = explodes.Length * gap;
            Camera.main.DOShakePosition(duration / 2, 0.5f);
            gameObject.GetComponent<SpriteRenderer>().DOFade(0, duration / 2);

            foreach (var obj in explodes)
            {
                obj.SetActive(true);
                yield return wait;
            }

            yield return new WaitForSeconds(duration - gap);
            OnDeathNotifyerHandler?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}
