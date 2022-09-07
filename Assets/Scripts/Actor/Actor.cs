using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IApplyDamage
{
    [SerializeField] GameObject bullet = default;
    private bool isDeath = false;
    protected float hp;


    //------------------------------------------
    // �p���拤�L�֐�
    //------------------------------------------
    protected bool IsDeath => isDeath;
    protected void ExcuteBullet(Transform tips, Quaternion rotate, Vector2 force, float power)
    {
        var obj = Instantiate(bullet, tips.position, rotate);
        obj.GetComponent<Bullet>().OnExcute(this, force, power);
    }


    //------------------------------------------
    // �O�����L���ۊ֐�
    //------------------------------------------
    public virtual void OnStateMachineExit() { }


    //------------------------------------------
    // �p���拤�L���ۊ֐�
    //------------------------------------------
    protected virtual void OnDeath() { }


    //------------------------------------------
    // �C���^�[�t�F�C�X
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
