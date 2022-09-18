using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundInfinit : MonoBehaviour
{
	[SerializeField] GameObject[] prefabs = default;
	[SerializeField] float offsetY = default;

	private Transform camform;
	private float prevWidth = 0;
	private float offset_x = 0f;


	private void Start()
	{
		transform.position = new Vector3(0, 0, 0);
		camform = Camera.main.transform;
		CreateNewStage(0);
	}
    private void Update()
    {
		if (camform.position.x + prevWidth > offset_x)
		{
			int rndNum = Random.Range(0, prefabs.Length);
			CreateNewStage(rndNum);
		}
	}
	private void CreateNewStage(int index)
	{
		var obj = Instantiate(prefabs[index], new Vector2(offset_x, offsetY), Quaternion.identity);
		prevWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x;
		offset_x += prevWidth;
	}
}