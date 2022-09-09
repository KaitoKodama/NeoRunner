using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera2D : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] float speedX = 3f;
    [SerializeField] float speedY = 25f;
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
            float x = Mathf.Lerp(_transform.position.x, _transform.position.x + 1, speedX * Time.fixedDeltaTime);
            float y = Mathf.Lerp(_transform.position.y, targetform.position.y + offset.y, speedY * Time.fixedDeltaTime);
            _transform.position = new Vector3(x, y, depth);
        }
    }
}
