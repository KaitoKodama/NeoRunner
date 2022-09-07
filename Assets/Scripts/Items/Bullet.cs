using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Actor owner;
    private float power;
    private float time = 0f;
    private float lifeTime = 5f;
    private bool isEnable = false;


    private void Update()
    {
        if (isEnable)
        {
            time += Time.deltaTime;
            if (time >= lifeTime)
            {
                isEnable = false;
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnable)
        {
            var trigger = collision.GetComponent<IApplyDamage>();
            var actor = collision.GetComponent<Actor>();
            if (trigger != null && actor != null)
            {
                if (!IsSameOwner(actor))
                {
                    trigger.ApplyDamage(power);
                    Destroy(gameObject);
                }
            }
        }
    }


    public void OnExcute(Actor owner, Vector2 force, float power)
    {
        GetComponent<Rigidbody2D>().velocity = force;
        this.power = power;
        this.owner = owner;
        isEnable = true;
    }


    private bool IsSameOwner(Actor target)
    {
        var ownerIsEnemy = owner.GetType().IsSubclassOf(typeof(Enemy));
        var targetIsEnemy = target.GetType().IsSubclassOf(typeof(Enemy));
        var ownerType = ownerIsEnemy ? typeof(Enemy) : typeof(Player);
        var targetType = targetIsEnemy ? typeof(Enemy) : typeof(Player);
        if (ownerType == targetType)
        {
            return true;
        }
        return false;
    }
}
