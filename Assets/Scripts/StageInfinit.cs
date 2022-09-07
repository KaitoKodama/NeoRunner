using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfinit : MonoBehaviour
{
	[SerializeField] GameObject[] stagePrefabs = default;
	[SerializeField] float width = 61f;
	private Transform playerform;
	private float offset_x = 0f;


	private void Start()
	{
		transform.position = new Vector3(0, 0, 0);
		playerform = GameObject.FindWithTag("Player").transform;
		CreateNewStage(0);
	}
	private void Update()
	{
		if (playerform.position.x + width > offset_x)
		{
			int rndNum = Random.Range(0, stagePrefabs.Length);
			CreateNewStage(rndNum);
		}
	}
	private void CreateNewStage(int index)
	{
		Instantiate(stagePrefabs[index], new Vector2(offset_x, 0), Quaternion.identity);
		offset_x += width;
	}
}
