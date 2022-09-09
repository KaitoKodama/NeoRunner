using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Actor
{
    [SerializeField] 
    protected SOEnemy data = default;

    [SerializeField] GameObject[] explodes = default;
    [SerializeField] AudioClip shootSound = default;
    [SerializeField] AudioClip explodeSound = default;


    private Rigidbody2D rigid;
    private AudioSource audioSource;
    private Transform playerform;
    private float isVisibleTime = 5;
    private float isVisibleCount = 0;
    private bool isVisible = false;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    protected void Start()
    {
        maxHP = data.MaxHP;
        hp = maxHP;

        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerform = GameObject.FindWithTag("Player").transform;
    }
    protected void Update()
    {
        if (!isVisible)
        {
            isVisibleCount += Time.deltaTime;
            if (isVisibleCount >= isVisibleTime)
            {
                OnDestroyNotifyerHandler?.Invoke(this);
                Destroy(this.gameObject);
            }
        }
        if (transform.position.y <= -50f)
        {
            OnDestroyNotifyerHandler?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
    private void OnBecameVisible()
    {
        isVisibleCount = 0;
        isVisible = true;
    }
    private void OnBecameInvisible()
    {
        isVisibleCount = 0;
        isVisible = false;
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public delegate void OnDeathNotifyer(Enemy enemy);
    public delegate void OnDestroyNotifyer(Enemy enemy);
    public OnDeathNotifyer OnDeathNotifyerHandler;
    public OnDestroyNotifyer OnDestroyNotifyerHandler;


    //------------------------------------------
    // 継承先共有関数
    //------------------------------------------
    protected Rigidbody2D Rigid => rigid;
    protected Transform Playerform => playerform;
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
    protected void Force(Vector2 expect, float speed, float gravity = 0)
    {
        if (playerform != null)
        {
            if (gravity != 0)
            {
                expect.y = gravity;
            }
            rigid.velocity = expect * speed;
        }
    }
    protected void ForceGravity(float gravity)
    {
        rigid.velocity = Vector2.up * gravity;
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

            var bullet = Locator<BulletPool>.I.GetEnemyBullet();
            if (bullet != null)
            {
                ExcuteBullet(bullet, transform.position, rotate, dir * speed, power);
                audioSource.PlayOneShot(shootSound);
            }
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
        audioSource.PlayOneShot(explodeSound);
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
            gameObject.GetComponent<SpriteRenderer>().DOFade(0, duration / 2);

            for (int i = 0; i < data.SupplyItemNum; i++)
            {
                var bullet = Locator<ItemPool>.I.GetJell();
                bullet?.OnExcute(transform.position);
            }

            foreach (var obj in explodes)
            {
                obj.SetActive(true);
                yield return wait;
            }

            yield return new WaitForSeconds(duration - gap);
            OnDeathNotifyerHandler?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
