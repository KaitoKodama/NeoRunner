using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IApplyDamage
{
    [SerializeField] GameObject bullet = default;
    private bool isDeath = false;
    protected float hp;


    //------------------------------------------
    // 継承先共有関数
    //------------------------------------------
    protected bool IsDeath => isDeath;
    protected void ExcuteBullet(Transform tips, Quaternion rotate, Vector2 force, float power)
    {
        var obj = Instantiate(bullet, tips.position, rotate);
        obj.GetComponent<Bullet>().OnExcute(this, force, power);
    }


    //------------------------------------------
    // 外部共有抽象関数
    //------------------------------------------
    public virtual void OnStateMachineExit() { }


    //------------------------------------------
    // 継承先共有抽象関数
    //------------------------------------------
    protected virtual void OnDeath() { }


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void ApplyDamage(float damage)
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
        }
    }
}
