using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Jell : MonoBehaviour
{
    [SerializeField] float shake = 5f;
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 15f;

    private Rigidbody2D rigid;
    private Transform playerform;
    private float time = 0;
    private bool isEnable = false;


    private void Update()
    {
        if (isEnable)
        {
            time += Time.deltaTime;
            if (time >= lifeTime)
            {
                time = 0;
                gameObject.SetActive(false);
                isEnable = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (isEnable)
        {
            var dir = (playerform.position - transform.position).normalized;
            rigid.velocity = dir * speed;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isEnable)
        {
            var reciever = collision.gameObject.GetComponent<IItemReciever>();
            if (reciever != null)
            {
                OnCollideToIItemReciever(reciever);
                OnCollideToTargetHandler?.Invoke();
                gameObject.SetActive(false);
                isEnable = false;
            }
        }
    }


    public delegate void OnCollideToTarget();
    public OnCollideToTarget OnCollideToTargetHandler;
    public void OnGenerate(Transform playerform)
    {
        this.playerform = playerform;
        rigid = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }
    public void OnExcute(Vector2 origin)
    {
        gameObject.SetActive(true);
        transform.position = origin;
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        rigid.velocity = new Vector2(x, y) * shake;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            isEnable = true;
        });
    }

    protected abstract void OnCollideToIItemReciever(IItemReciever reciever);
}
