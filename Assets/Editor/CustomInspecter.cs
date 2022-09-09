using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyInfinit))]
public class StageSOEditor : Editor
{
	GUIContent enemyPrefabLabel;
	SerializedProperty enemyPrefabProp;

	GUIContent createEachLabel;
	SerializedProperty createEachProp;

	GUIContent maxCreateEachLabel;
	SerializedProperty maxCreateEachProp;

	GUIContent createIncrementLabel;
	SerializedProperty createIncrementProp;

	GUIContent offsetBeginLabel;
	SerializedProperty offsetBeginProp;

	GUIContent offsetTypeLabel;
	SerializedProperty offsetTypeProp;

	GUIContent offsetLabel;
	SerializedProperty offsetProp;

	GUIContent fieldEnemyMaxLabel;
	SerializedProperty fieldEnemyMaxProp;

	GUIContent delayLabel;
	SerializedProperty delayProp;

	GUIContent currGapLabel;
	SerializedProperty currGapProp;

	GUIContent minGapLabel;
	SerializedProperty minGapProp;

	GUIContent decreaseGapSpeedLabel;
	SerializedProperty decreaseGapSpeedProp;

	private void OnEnable()
	{
		enemyPrefabLabel = new GUIContent("プレハブ");
		enemyPrefabProp = serializedObject.FindProperty("enemyPrefab");

		fieldEnemyMaxLabel = new GUIContent("最大個体数");
		fieldEnemyMaxProp = serializedObject.FindProperty("fieldEnemyMax");

		createEachLabel = new GUIContent("一回辺りの生成個体数");
		createEachProp = serializedObject.FindProperty("createEach");

		maxCreateEachLabel = new GUIContent("最大値");
		maxCreateEachProp = serializedObject.FindProperty("maxCreateEach");

		createIncrementLabel = new GUIContent("増幅");
		createIncrementProp = serializedObject.FindProperty("createIncrement");

		offsetBeginLabel = new GUIContent("画面指定");
		offsetBeginProp = serializedObject.FindProperty("offsetBegin");

		offsetTypeLabel = new GUIContent("タイプ指定");
		offsetTypeProp = serializedObject.FindProperty("offsetType");

		offsetLabel = new GUIContent("座標指定");
		offsetProp = serializedObject.FindProperty("offset");

		delayLabel = new GUIContent("初期遅延時間");
		delayProp = serializedObject.FindProperty("delay");

		currGapLabel = new GUIContent("待機時間");
		currGapProp = serializedObject.FindProperty("currGap");

		minGapLabel = new GUIContent("最小待機時間");
		minGapProp = serializedObject.FindProperty("minGap");

		decreaseGapSpeedLabel = new GUIContent("減少速度");
		decreaseGapSpeedProp = serializedObject.FindProperty("decreaseGapSpeed");
	}
	public override void OnInspectorGUI()
	{
		// 最新データを取得
		serializedObject.Update();

		EditorGUILayout.HelpBox("基本情報", MessageType.None);
		EditorGUILayout.PropertyField(enemyPrefabProp, enemyPrefabLabel);
		EditorGUILayout.PropertyField(delayProp, delayLabel);
		EditorGUILayout.PropertyField(fieldEnemyMaxProp, fieldEnemyMaxLabel);
		EditorGUILayout.Space(20);

		EditorGUILayout.HelpBox("生成情報", MessageType.None);
		EditorGUILayout.PropertyField(createEachProp, createEachLabel);
		if (createEachProp.intValue != 1)
        {
			EditorGUILayout.PropertyField(createIncrementProp, createIncrementLabel);
			if (createIncrementProp.boolValue)
            {
				EditorGUILayout.PropertyField(maxCreateEachProp, maxCreateEachLabel);
            }
        }
		EditorGUILayout.PropertyField(offsetBeginProp, offsetBeginLabel);
		EditorGUILayout.PropertyField(offsetTypeProp, offsetTypeLabel);
		if (offsetTypeProp.intValue == 1)
		{
			EditorGUILayout.PropertyField(offsetProp, offsetLabel);
		}
		EditorGUILayout.Space(20);

		EditorGUILayout.HelpBox("出現時間情報", MessageType.None);
		EditorGUILayout.PropertyField(currGapProp, currGapLabel);
		EditorGUILayout.PropertyField(minGapProp, minGapLabel);
		EditorGUILayout.PropertyField(decreaseGapSpeedProp, decreaseGapSpeedLabel);

		serializedObject.ApplyModifiedProperties();
	}
}
#endif