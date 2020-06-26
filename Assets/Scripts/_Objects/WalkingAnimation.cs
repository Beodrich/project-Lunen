using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Walking Animation", menuName = "GameElements/Animation/Walking")]
public class WalkingAnimation : ScriptableObject
{
    public Texture2D CharacterSpritesheet;
    [HideInInspector] public Sprite[] CharacterSpriteList;

    public float CharacterIdleTime;
    public float CharacterWalkTime;

    public void RepopulateSprites()
    {
        CharacterSpriteList = Resources.LoadAll<Sprite>(CharacterSpritesheet.name);
    }

    public Sprite GetAnimationSprite(int index)
    {
        if (index >= CharacterSpriteList.Length) RepopulateSprites();
        if (index < CharacterSpriteList.Length) return CharacterSpriteList[index];
        else return null;
    }

    public int GetAnimationIndex(MoveScripts.Direction direction, bool moving, float time)
    {
        int result = 0;
        float modulo = 0;
        if (moving)
        {
            switch(direction)
            {
                case MoveScripts.Direction.South:
                    result = 16;
                    modulo = getWalkModulo(time);
                    result += (int)(modulo / CharacterWalkTime);
                break;
                case MoveScripts.Direction.West:
                    result = 24;
                    modulo = getWalkModulo(time);
                    result += (int)(modulo / CharacterWalkTime);
                break;
                case MoveScripts.Direction.East:
                    result = 32;
                    modulo = getWalkModulo(time);
                    result += (int)(modulo / CharacterWalkTime);
                break;
                case MoveScripts.Direction.North:
                    result = 40;
                    modulo = getWalkModulo(time);
                    result += (int)(modulo / CharacterWalkTime);
                break;
            }
        }
        else
        {
            switch(direction)
            {
                case MoveScripts.Direction.South:
                    result = 0;
                    modulo = getIdleModulo(time);
                    result += (int)(modulo / CharacterIdleTime);
                break;
                case MoveScripts.Direction.West:
                    result = 4;
                    modulo = getIdleModulo(time);
                    result += (int)(modulo / CharacterIdleTime);
                break;
                case MoveScripts.Direction.East:
                    result = 8;
                    modulo = getIdleModulo(time);
                    result += (int)(modulo / CharacterIdleTime);
                break;
                case MoveScripts.Direction.North:
                    result = 12;
                    modulo = getIdleModulo(time);
                    result += (int)(modulo / CharacterIdleTime);
                break;
            }
        }
        return result;
    }

    public float getIdleModulo(float time)
    {
        return (time % (CharacterIdleTime*4));
    }

    public float getWalkModulo(float time)
    {
        return (time % (CharacterWalkTime*8));
    }
}
