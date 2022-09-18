using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IApplyDamage
{
    [SerializeField]
    protected float maxHP;
    protected float hp;

    private bool isDeath = false;


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public float HP => hp;
    public float MaxHP => maxHP;
    public float ClampHP { get { return hp / maxHP; } }

    //------------------------------------------
    // 継承先共有関数
    //------------------------------------------
    protected bool IsDeath => isDeath;
    protected void ExcuteBullet(Bullet bullet, Vector2 pos, Quaternion rot, Vector2 force, float power)
    {
        bullet.OnExcute(pos, rot, force, power);
    }


    //------------------------------------------
    // 外部共有抽象関数
    //------------------------------------------
    public virtual void OnStateMachineExit() { }


    //------------------------------------------
    // 継承先共有抽象関数
    //------------------------------------------
    protected virtual void OnDamage() { }
    protected virtual void OnDeath() { }


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public virtual void ApplyDamage(float damage)
    {
        if (!isDeath)
        {
            hp -= damage;
            if (hp <= 0)
            {
                hp = 0;
                isDeath = true;
                OnDeath();
            }
            else
            {
                OnDamage();
            }
        }
    }
}
