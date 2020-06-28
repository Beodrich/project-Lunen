using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Type", menuName = "GameElements/Type")]
public class Type : ScriptableObject
{
    static float HighEffect = 2.0f;
    static float NormalEffect = 1.0f;
    static float LowEffect = 0.5f;

    public int indexValue;
    
    public List<Type> StrongAgainst;
    public List<Type> WeakAgainst;

    public bool IsStrongAgainst(Type type)
    {
        foreach (Type t in StrongAgainst) if (t == type) return true;
        return false;
    }

    public bool IsWeakAgainst(Type type)
    {
        foreach (Type t in StrongAgainst) if (t == type) return true;
        return false;
    }

    public static float SameTypeAttackBonus(List<Type> lunenTypes, Type attackType)
    {
        bool foundSTAB = false;
        for (int i = 0; i < lunenTypes.Count; i++)
        {
            if (lunenTypes[i] == attackType) foundSTAB = true;
        }
        if (foundSTAB) return 1.5f; else return 1f;
    }

    public static float TypeMatch(Type sender, List<Type> receiver)
    {
        float multiplier = 1f;
        for (int i = 0; i < receiver.Count; i++)
        {
            multiplier *= TypeMatch(sender, receiver[i]);
        }
        return multiplier;
    }

    public static float TypeMatch(Type sender, Type receiver)
    {
        if (sender.IsStrongAgainst(receiver)) return HighEffect;
        if (sender.IsWeakAgainst(receiver)) return LowEffect;
        return NormalEffect;
    }
}
