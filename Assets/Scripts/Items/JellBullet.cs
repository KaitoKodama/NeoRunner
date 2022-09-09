using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JellBullet : Jell
{
    [SerializeField] float value = 1;

    protected override void OnCollideToIItemReciever(IItemReciever reciever)
    {
        reciever.ApplyBullet(value);
    }
}
