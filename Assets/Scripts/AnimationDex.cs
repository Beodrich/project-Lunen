using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class AnimationDex : MonoBehaviour
{
    public enum CharacterSpriteEnum
    {
        Template,
        MaleMainCharacter,
        FemaleMainCharacter,
        Character000,
        Character001,
        Character002,
        Character003,
        Character004,
        Character005,
        Character006,
        Character007,
        Character008,
        Character009,
        Character010,
        Character011,
        Character012,
        Character013,
        Character014,
        Character015,
        Character016,
        Character017,
        Character018,
        Character019,
        Character020,
        Character021,
        Character022,
        Character023,
        Character024,
        Character025,
        Character026,
        Character027,
        Character028,
        Character029,
        Trainer000,
        Trainer001,
        Trainer002,
        Trainer003,
        Trainer004,
        Trainer005,
        Trainer006,
        Trainer007,
        Trainer008,
        Trainer009,
        Trainer010,
        Trainer011,
        Trainer012,
        Trainer013,
        Trainer014,
        Trainer015,
        Trainer016,
        Trainer017,
        Trainer018,
        Trainer019,
        Trainer020,
        Trainer021,
        Trainer022,
        Trainer023,
        Trainer024,
        Trainer025,
        Trainer026,
        Trainer027,
        Trainer028,
        Trainer029,
    }

    [NamedArray(typeof(CharacterSpriteEnum))]
    public List<Texture2D> CharacterSpritesheetList = new List<Texture2D>();
    private List<Sprite[]> CharacterSpriteList;

    public float CharacterIdleTime;
    public float CharacterWalkTime;

    void Awake()
    {
        SetupSprites();
    }

    public void SetupSprites()
    {
        if (CharacterSpriteList == null)
        {
            CharacterSpriteList = new List<Sprite[]>();
            for (int i = 0; i < CharacterSpritesheetList.Count; i++)
            {
                Sprite[] a = Resources.LoadAll<Sprite>(CharacterSpritesheetList[i].name);
                CharacterSpriteList.Add(a);
            }
        }
        
    }

    public Sprite GetAnimationSprite(CharacterSpriteEnum charType, int index)
    {
        SetupSprites();
        return CharacterSpriteList[(int)charType][index];
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
