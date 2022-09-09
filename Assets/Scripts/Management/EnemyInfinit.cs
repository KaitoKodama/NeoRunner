using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyInfinit : MonoBehaviour
{
	[SerializeField] Enemy enemyPrefab = default;
	[SerializeField] float enemyScore = 10;
	[SerializeField] int fieldEnemyMax = 10;
	[SerializeField] int createEach = 1;
	[SerializeField] int maxCreateEach = 1;
	[SerializeField] bool createIncrement = false;

	[SerializeField] float offset = 0;
	[SerializeField] OffsetBegin offsetBegin = OffsetBegin.ScreenRightSide;
	[SerializeField] OffsetType offsetType = OffsetType.RandomBoth;

	[SerializeField] float delay = 0f;
	[SerializeField] float currGap = 10f;
	[SerializeField] float minGap = 2f;
	[SerializeField] float decreaseGapSpeed = 0.1f;

	[SerializeField] private List<Enemy> fieldEnemy;
	private bool enable = false;
	private float time = 0f;
	private float score = 0;

	private enum OffsetBegin
    {
		[InspectorName("画面右側")] ScreenRightSide,
		[InspectorName("画面左側")] ScreenLeftSide,
	}
	private enum OffsetType
    {
		[InspectorName("上下ランダム")] RandomBoth,
		[InspectorName("上ランダム、下固定")] RandomTopFixedBottom,
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
			time += Time.deltaTime;
			if (time >= currGap)
			{
				if (fieldEnemy.Count < fieldEnemyMax)
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


	public float Score => score;


    private void CreateEnemy()
	{
        for (int i = 0; i < createEach; i++)
        {
			float enemyX = default;
			float enemyY = default;

			if (offsetBegin == OffsetBegin.ScreenRightSide)
            {
				var rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
				enemyX = rightTop.x;
				if (offsetType == OffsetType.RandomBoth)
                {
					enemyY = Random.Range(0, rightTop.y);
				}
				if (offsetType == OffsetType.RandomTopFixedBottom)
                {
					enemyY = Random.Range(offset, rightTop.y);
				}
            }
			if (offsetBegin == OffsetBegin.ScreenLeftSide)
			{
				var leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
				enemyX = leftTop.x;
				if (offsetType == OffsetType.RandomBoth)
				{
					enemyY = Random.Range(0, leftTop.y);
				}
				if (offsetType == OffsetType.RandomTopFixedBottom)
				{
					enemyY = Random.Range(offset, leftTop.y);
				}
			}

			var obj = Instantiate(enemyPrefab.gameObject, transform.position, Quaternion.identity);
			obj.transform.position = new Vector3(enemyX, enemyY, 0);

			var enemy = obj.GetComponent<Enemy>();
			enemy.OnDeathNotifyerHandler = OnDeathNotifyerReciever;
			enemy.OnDestroyNotifyerHandler = OnDestroyNotifyerReciever;
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
		score += enemyScore;
	}
	private void OnDestroyNotifyerReciever(Enemy enemy)
	{
		fieldEnemy.Remove(enemy);
	}
}
