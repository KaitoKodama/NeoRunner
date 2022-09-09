using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFaced : MonoBehaviour
{
    private EnemyInfinit[] infinits;


    void Start()
    {
        infinits = transform.GetComponentsInChildren<EnemyInfinit>();
    }

    public float GetFullScore()
    {
        float score = 0;
        foreach (var infinit in infinits)
        {
            score += infinit.Score;
        }
        return score;
    }
}
