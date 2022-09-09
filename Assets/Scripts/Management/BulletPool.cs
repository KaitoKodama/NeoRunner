using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] Bullet enemyBulletPrefab = default;
    [SerializeField] Bullet playerBulletPrefab = default;
    [SerializeField] int enemyBulletPoolNum = 10;
    [SerializeField] int playerBulletPoolNum = 10;

    private List<Bullet> enemyBulletList;
    private List<Bullet> playerBulletList;


    private void Awake()
    {
        Locator<BulletPool>.Bind(this);

        SetPool(ref enemyBulletList, enemyBulletPrefab.gameObject, enemyBulletPoolNum);
        SetPool(ref playerBulletList, playerBulletPrefab.gameObject, playerBulletPoolNum);
    }
    private void OnDestroy()
    {
        Locator<BulletPool>.Unbind(this);
    }


    public BulletEnemy GetEnemyBullet()
    {
        foreach (var bullet in enemyBulletList)
        {
            if (!bullet.gameObject.activeSelf)
            {
                return (BulletEnemy)bullet;
            }
        }
        return null;
    }
    public BulletPlayer GetPlayerBullet()
    {
        foreach (var bullet in playerBulletList)
        {
            if (!bullet.gameObject.activeSelf)
            {
                return (BulletPlayer)bullet;
            }
        }
        return null;
    }


    private void SetPool(ref List<Bullet> bulletList, GameObject prefab, int poolNum)
    {
        bulletList = new List<Bullet>(poolNum);
        for (int i = 0; i < poolNum; i++)
        {
            var obj = Instantiate(prefab, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            var bullet = obj.GetComponent<Bullet>();
            bullet.OnGenerate();
            bulletList.Add(bullet);
        }
    }
}
