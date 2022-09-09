using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Data", menuName = "ScriptableObject/SOEnemy")]
public class SOEnemy : ScriptableObject
{
    [SerializeField] GameObject bulletPrefab = default;
    [SerializeField] int supplyItemNum = 5;
    [SerializeField] float maxHP = 0f;
    [SerializeField] float speed = 5f;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] float bulletPower = 5f;
    [SerializeField] float bulletDuration = 0.5f;
    [SerializeField] float attackDistance = 10f;
    [SerializeField] float chaseDistance = 15f;


    public GameObject BulletPrefab => bulletPrefab;
    public int SupplyItemNum => supplyItemNum;
    public float MaxHP => maxHP;
    public float Speed => speed;
    public float BulletSpeed => bulletSpeed;
    public float BulletPower => bulletPower;
    public float BulletDuration => bulletDuration;
    public float AttackDistance => attackDistance;
    public float ChaseDistance => chaseDistance;
}
