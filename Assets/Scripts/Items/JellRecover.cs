using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellRecover : Jell
{
    [SerializeField] float value = 5f;

    protected override void OnCollideToIItemReciever(IItemReciever reciever)
    {
        reciever.ApplyRecover(value);
    }
}
