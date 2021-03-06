﻿using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lunen", menuName = "GameElements/Lunen")]
public class Lunen : ScriptableObject
{
    [System.Serializable]
    public struct LearnedAction
    {
        public Action action;
        public int level;
    }

    [Header("Species Info")]

    public AnimationSet animationSet;
    public List<Type> Elements;
    //public List<LearnedAction> ActionsToLearn;
    public List<LearnedAction> LearnedActions;

    public bool Evolves;
    [ConditionalField(nameof(Evolves))] public Lunen EvolutionLunen;
    [ConditionalField(nameof(Evolves))] public int EvolutionLevel;

    [Header("Stats")]

    [VectorLabels("Start"," PerLevel")]
    public Vector2Int Health;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Attack;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Defense;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Speed;

    public int AffinityCost;
    public float CatchRate;
    public float CooldownTime;

    public bool HasType(Type type)
    {
        foreach (Type t in Elements)
        {
            if (t == type) return true;
        }
        return false;
    }
}
