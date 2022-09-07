using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollider : MonoBehaviour
{
    private bool isGround = false;
    private bool isThrowGround = false;


    public bool IsThrowGround => isThrowGround;
    public bool IsGround => isGround;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isGround)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGround = true;
            }
        }
        if (!isGround || !isThrowGround)
        {
            if (collision.gameObject.CompareTag("ThrowGround"))
            {
                isGround = true;
                isThrowGround = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
        if (collision.gameObject.CompareTag("ThrowGround"))
        {
            isGround = false;
            isThrowGround = false;
        }
    }
}
