using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Types : MonoBehaviour
{
    [System.Serializable]
    public enum Element
    {
        Balanced,
        Earth,
        Water,
        Floral,
        Fire,
        Plasma,
        Sky,
        Ice,
        Psychic,
        Cosmic,
        Corrupted
    }

    const float HE = 2.0f; //High Effect
    const float NE = 1.0f; //Normal Effect
    const float LE = 0.5f; //Low Effect

    public float TypeMatch(Element sender, Element[] reciever)
    {
        float multiplier = 1f;
        for (int i = 0; i < reciever.Length; i++)
        {
            multiplier *= TypeMatch(sender, reciever[i]);
        }
        return multiplier;
    }

    public float TypeMatch(Element sender, Element reciever)
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
}
