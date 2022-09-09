using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.gameObject.GetComponent<IApplyDamage>();
        if (target != null)
        {
            if (!target.GetType().IsSubclassOf(typeof(Enemy)))
            {
                target.ApplyDamage(Power);
                gameObject.SetActive(false);
            }
        }
    }
}
