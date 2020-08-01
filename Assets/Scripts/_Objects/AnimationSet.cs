using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Animation Set", menuName = "GameElements/AnimationSet")]
public class AnimationSet : ScriptableObject
{
    [System.Serializable]
    public class Animation
    {
        public string name;
        public float iterateTime;
        public bool loops;
        public bool fourDirectional;
        public bool twoDirectional;
        public List<Sprite> spriteArray;
        
        public int spritesPerDirection
        {
            get
            {
                if (fourDirectional)
                {
                    return spriteArray.Count/4;
                }
                else if (twoDirectional)
                {
                    return spriteArray.Count/2;
                }
                else
                {
                    return spriteArray.Count;
                }
            }
        }

        public float loopTime
        {
            get
            {
                return iterateTime*spritesPerDirection;
            }
        }

        public int getDirectionStartIndex(MoveScripts.Direction direction)
        {
            int directionValue = 0;
            if (fourDirectional)
            {
                switch (direction)
                {
                    case MoveScripts.Direction.North: directionValue = 1; break;
                    case MoveScripts.Direction.South: directionValue = 0; break;
                    case MoveScripts.Direction.East: directionValue = 2; break;
                    case MoveScripts.Direction.West: directionValue = 3; break;
                }
            }
            else if (twoDirectional)
            {
                switch (direction)
                {
                    case MoveScripts.Direction.North: directionValue = 1; break;
                    case MoveScripts.Direction.South: directionValue = 0; break;
                    case MoveScripts.Direction.East: directionValue = 0; break;
                    case MoveScripts.Direction.West: directionValue = 1; break;
                }
            }
            
            return (directionValue*spritesPerDirection);
        }

        public float getModulo(float time)
        {
            if (!loops && time > loopTime)
            {
                return 0;
            }
            else
            {
                return (time % (iterateTime*spritesPerDirection));
            }
            
        }
    }

    public bool isLunen;
    public List<Animation> animations;

    public Animation GetAnimation(string _name)
    {
        foreach (Animation a in animations) if (a.name == _name) return a;
        Debug.Log("Unable to find animation: " + _name + " in " + name);
        return animations[0];
    }

    public string GetAnimationName(int index)
    {
        return animations[index].name;
    }

    public Sprite GetAnimationSprite(string type, int index)
    {
        return GetAnimation(type).spriteArray[index];
    }

    public int GetAnimationIndex(string type, MoveScripts.Direction direction, float time)
    {
        Animation animation = GetAnimation(type);
        int result = animation.getDirectionStartIndex(direction);
        float modulo = animation.getModulo(time);

        result += (int)(modulo/animation.iterateTime);

        return result;
    }

    public string[] GetAnimList()
    {
        List<string> getList = new List<string>();
        foreach (Animation a in animations) getList.Add(a.name);
        return getList.ToArray();
    }
}
