using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var actor = collision.gameObject.GetComponent<Actor>();
        if(actor != null)
        {
            Destroy(actor.gameObject);
        }
    }
}
