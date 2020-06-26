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
    /*
    [System.Serializable]
    public enum Element
    {
        Balanced = 0,
        Earth = 1,
        Water = 2,
        Floral = 3,
        Fire = 4,
        Plasma = 5,
        Sky = 6,
        Ice = 7,
        Psychic = 8,
        Cosmic = 9,
        Corrupted = 10
    }

    const float HE = 2.0f; //High Effect
    const float NE = 1.0f; //Normal Effect
    const float LE = 0.5f; //Low Effect

    public static string GetTypeString(Element index)
    {
        switch (index)
        {
            case Element.Balanced:
                return "Balanced";
            case Element.Earth:
                return "Earth";
            case Element.Water:
                return "Water";
            case Element.Floral:
                return "Floral";
            case Element.Fire:
                return "Fire";
            case Element.Plasma:
                return "Plasma";
            case Element.Sky:
                return "Sky";
            case Element.Ice:
                return "Ice";
            case Element.Psychic:
                return "Psychic";
            case Element.Cosmic:
                return "Cosmic";
            case Element.Corrupted:
                return "Corrupted";
            default:
                return "Undefined";

        }
    }

    public static float SameTypeAttackBonus(List<Element> lunenTypes, Element attackType)
    {
        bool foundSTAB = false;
        for (int i = 0; i < lunenTypes.Count; i++)
        {
            if (lunenTypes[i] == attackType) foundSTAB = true;
        }
        if (foundSTAB) return 1.5f; else return 1f;
    }

    public static float TypeMatch(Element sender, List<Element> reciever)
    {
        float multiplier = 1f;
        for (int i = 0; i < reciever.Count; i++)
        {
            multiplier *= TypeMatch(sender, reciever[i]);
        }
        return multiplier;
    }

    public static float TypeMatch(Element sender, Element reciever)
    {
        switch (sender)
        {
            case Element.Balanced:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return NE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return HE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return LE;
                    default:
                        return NE;

                }
            case Element.Earth:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return NE;
                    case Element.Water:
                        return LE;
                    case Element.Floral:
                        return LE;
                    case Element.Fire:
                        return HE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return LE;
                    case Element.Ice:
                        return HE;
                    case Element.Psychic:
                        return HE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return LE;
                    default:
                        return NE;
                }
            case Element.Water:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return HE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return HE;
                    case Element.Plasma:
                        return LE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return LE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return LE;
                    default:
                        return NE;
                }
            case Element.Floral:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return HE;
                    case Element.Water:
                        return HE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return LE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return LE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return LE;
                    default:
                        return NE;
                }
            case Element.Fire:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return LE;
                    case Element.Water:
                        return LE;
                    case Element.Floral:
                        return HE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return HE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return LE;
                    default:
                        return NE;
                }
            case Element.Plasma:
                switch (reciever)
                {
                    case Element.Balanced:
                        return LE;
                    case Element.Earth:
                        return LE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return HE;
                    case Element.Cosmic:
                        return HE;
                    case Element.Corrupted:
                        return NE;
                    default:
                        return NE;
                }
            case Element.Sky:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return HE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return LE;
                    case Element.Sky:
                        return LE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return NE;
                    default:
                        return NE;
                }
            case Element.Ice:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return LE;
                    case Element.Water:
                        return HE;
                    case Element.Floral:
                        return HE;
                    case Element.Fire:
                        return LE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return HE;
                    case Element.Ice:
                        return LE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return NE;
                    case Element.Corrupted:
                        return NE;
                    default:
                        return NE;
                }
            case Element.Psychic:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return NE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return LE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return HE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return LE;
                    case Element.Cosmic:
                        return HE;
                    case Element.Corrupted:
                        return NE;
                    default:
                        return NE;
                }
            case Element.Cosmic:
                switch (reciever)
                {
                    case Element.Balanced:
                        return NE;
                    case Element.Earth:
                        return NE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return HE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return LE;
                    case Element.Cosmic:
                        return LE;
                    case Element.Corrupted:
                        return HE;
                    default:
                        return NE;
                }
            case Element.Corrupted:
                switch (reciever)
                {
                    case Element.Balanced:
                        return HE;
                    case Element.Earth:
                        return NE;
                    case Element.Water:
                        return NE;
                    case Element.Floral:
                        return NE;
                    case Element.Fire:
                        return NE;
                    case Element.Plasma:
                        return NE;
                    case Element.Sky:
                        return NE;
                    case Element.Ice:
                        return NE;
                    case Element.Psychic:
                        return NE;
                    case Element.Cosmic:
                        return LE;
                    case Element.Corrupted:
                        return HE;
                    default:
                        return NE;
                }
            default: //Failsafe if bad type
                return NE;
        }
        
    }
    */
}
