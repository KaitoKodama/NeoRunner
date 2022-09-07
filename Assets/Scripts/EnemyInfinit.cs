using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyInfinit : MonoBehaviour
{
	[SerializeField] Enemy enemyPrefab = default;
	[SerializeField] int fieldEnemyMax = 10;
	[SerializeField] int createEach = 1;
	[SerializeField] int maxCreateEach = 1;
	[SerializeField] bool createIncrement = false;

	[SerializeField] float offsetY = 0;
	[SerializeField] OffsetType offsetType = OffsetType.RandomY;

	[SerializeField] float delay = 0f;
	[SerializeField] float currGap = 10f;
	[SerializeField] float minGap = 2f;
	[SerializeField] float decreaseGapSpeed = 0.1f;

	private List<Enemy> fieldEnemy;
	private bool enable = false;
	private float time = 0f;

	private enum OffsetType
    {
		[InspectorName("画面の右上下座標からランダムに")] RandomY,
		[InspectorName("画面の右下指定のY座標から画面の右上座標まで")] FromYToRandomY,
	}

    private void Start()
    {
		fieldEnemy = new List<Enemy>(fieldEnemyMax);
		time = currGap;
		DOVirtual.DelayedCall(delay, () =>
		{
			enable = true;
		});
	}
    private void Update()
    {
		if (enable)
        {
			if (fieldEnemy.Count < fieldEnemyMax)
            {
				time += Time.deltaTime;
				if (time >= currGap)
				{
					if (currGap >= minGap)
					{
						currGap -= decreaseGapSpeed;
					}
					CreateEnemy();
					time = 0;
				}
            }
        }
    }


    private void CreateEnemy()
	{
        for (int i = 0; i < createEach; i++)
        {
			var rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
			float enemyX = rightTop.x;
			float enemyY = default;

			if (offsetType == OffsetType.FromYToRandomY)
            {
				enemyY = Random.Range(offsetY, rightTop.y);
			}
			if (offsetType == OffsetType.RandomY)
            {
				enemyY = Random.Range(0, rightTop.y);
			}

			var obj = Instantiate(enemyPrefab.gameObject, transform.position, Quaternion.identity);
			obj.transform.position = new Vector3(enemyX, enemyY, 0);

			var enemy = obj.GetComponent<Enemy>();
			enemy.OnDeathNotifyerHandler = OnDeathNotifyerReciever;
			fieldEnemy.Add(enemy);
		}

		if (createIncrement && createEach < maxCreateEach)
        {
			createEach += 1;
        }
	}

	private void OnDeathNotifyerReciever(Enemy enemy)
    {
		fieldEnemy.Remove(enemy);
	}
}
