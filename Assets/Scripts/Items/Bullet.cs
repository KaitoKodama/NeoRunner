using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rigid;
    private float power;
    private float time = 0f;
    private float lifeTime = 5f;


    protected void Update()
    {
        if (gameObject.activeSelf)
        {
            time += Time.deltaTime;
            if (time >= lifeTime)
            {
                time = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    protected float Power => power;
    public void OnGenerate()
    {
        rigid = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }
    public void OnExcute(Vector2 pos, Quaternion rot, Vector2 force, float power)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = rot;
        rigid.velocity = force;
        this.power = power;
    }
}
