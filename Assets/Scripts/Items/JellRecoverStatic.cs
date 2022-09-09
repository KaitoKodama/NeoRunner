using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellRecoverStatic : MonoBehaviour
{
    [SerializeField] float value = 1;
    [SerializeField] AudioClip sound = default;
    [SerializeField] SpriteRenderer render = default;

    private bool isSuppply = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSuppply)
        {
            var reciever = collision.gameObject.GetComponent<IItemReciever>();
            if (reciever != null)
            {
                isSuppply = true;
                reciever.ApplyRecover(value);
                render.color = new Color(1, 1, 1, 0);
                gameObject.GetComponent<AudioSource>().PlayOneShot(sound);

                Destroy(gameObject, 1f);
            }
        }
    }
}
