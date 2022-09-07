using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera2D : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] float speed = 5f;
    [SerializeField] float depth = -10f;
    private Transform targetform;
    private Transform _transform;

    void Start()
    {
        _transform = this.gameObject.transform;
        targetform = GameObject.FindWithTag("Player").transform;
        _transform.position = targetform.position;
    }

    void FixedUpdate()
    {
        if (targetform != null)
        {
            float x = Mathf.Lerp(_transform.position.x, targetform.position.x + offset.x, speed * Time.deltaTime);
            float y = Mathf.Lerp(_transform.position.y, targetform.position.y + offset.y, speed * Time.deltaTime);
            _transform.position = new Vector3(x, y, depth);
        }
    }
}
