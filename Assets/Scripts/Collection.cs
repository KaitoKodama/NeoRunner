using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//------------------------------------------
// クラス
//------------------------------------------
[System.Serializable]
public class AnyDictionary<Tkey, Tvalue>
{
	public Tkey key;
	public Tvalue value;

	public AnyDictionary(Tkey key, Tvalue value)
	{
		this.key = key;
		this.value = value;
	}
	public AnyDictionary(KeyValuePair<Tkey, Tvalue> pair)
	{
		this.key = pair.Key;
		this.value = pair.Value;
	}
}

[System.Serializable]
public struct FromTo
{
	public float from;
	public float to;
}


//------------------------------------------
// インターフェイス
//------------------------------------------
public interface IApplyDamage
{
	void ApplyDamage(float damage);
}
public interface IItemReciever
{
	void ApplyRecover(float value);
	void ApplyBullet(float num);
}


//------------------------------------------
// 列挙
//------------------------------------------
public enum BuildScene
{
	Title,
	Battle,
}



//------------------------------------------
// ユーティリティ
//------------------------------------------
namespace CMN
{
	public static class Utility
	{
		public static TValue GetDICVal<TValue, TKey>(TKey component, List<AnyDictionary<TKey, TValue>> dics)
		{
			foreach (var dic in dics)
			{
				if (dic.key.Equals(component))
				{
					return dic.value;
				}
			}
			return default;
		}
		public static T GetNextEnum<T>(int currentEnum)
		{
			int nextIndex = currentEnum + 1;
			T nextEnum = (T)Enum.ToObject(typeof(T), nextIndex);
			int length = Enum.GetValues(typeof(T)).Length;
			if (nextIndex >= length)
			{
				nextEnum = (T)Enum.ToObject(typeof(T), 0);
			}
			return nextEnum;
		}
		public static T GetIntToEnum<T>(int targetInt)
		{
			T targetEnum = (T)Enum.ToObject(typeof(T), targetInt);
			return targetEnum;
		}
		public static bool Probability(float fPercent)
		{
			float fProbabilityRate = UnityEngine.Random.value * 100.0f;

			if (fPercent == 100.0f && fProbabilityRate == fPercent) return true;
			else if (fProbabilityRate < fPercent) return true;
			else return false;
		}
		public static Vector3 CircleHorizon(Vector2 origin, float min, float max)
		{
			var angle = UnityEngine.Random.Range(0, 360);
			var radius = UnityEngine.Random.Range(min, max);
			var rad = angle * Mathf.Deg2Rad;
			var px = Mathf.Cos(rad) * radius;
			var py = Mathf.Sin(rad) * radius;
			return new Vector3(px + origin.x, py + origin.y, 0);
		}
	}
}