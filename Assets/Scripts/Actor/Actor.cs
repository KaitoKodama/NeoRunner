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
    // �O�����L�֐�
    //------------------------------------------
    public float HP => hp;
    public float MaxHP => maxHP;
    public float ClampHP { get { return hp / maxHP; } }

    //------------------------------------------
    // �p���拤�L�֐�
    //------------------------------------------
    protected bool IsDeath => isDeath;
    protected void ExcuteBullet(Bullet bullet, Vector2 pos, Quaternion rot, Vector2 force, float power)
    {
        bullet.OnExcute(pos, rot, force, power);
    }


    //------------------------------------------
    // �O�����L���ۊ֐�
    //------------------------------------------
    public virtual void OnStateMachineExit() { }


    //------------------------------------------
    // �p���拤�L���ۊ֐�
    //------------------------------------------
    protected virtual void OnDamage() { }
    protected virtual void OnDeath() { }


    //------------------------------------------
    // �C���^�[�t�F�C�X
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
