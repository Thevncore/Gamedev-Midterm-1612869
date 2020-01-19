using System;
using UnityEngine;

public class ScoreLine : ScriptableObject
{
    public int Amount { get; set; }
    public Action Destroyed;

    private void OnDestroy()
    {
        Destroyed();
    }
}

//delegate void ScoreLineExpiredHandler(ScoreLine sender);