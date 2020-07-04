using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Animation Set", menuName = "GameElements/AnimationSet")]
public class AnimationSet : ScriptableObject
{
    public enum AnimationType
    {
        Idle,
        Walk,
        Run,
        Attack,
    }

    [System.Serializable]
    public class Animation
    {
        public string name;
        public float iterateTime;
        public List<Sprite> spriteArray;
        

        public int spritesPerDirection
        {
            get
            {
                return spriteArray.Count/4;
            }
        }

        public int getDirectionStartIndex(MoveScripts.Direction direction)
        {
            int directionValue = 0;
            switch (direction)
            {
                case MoveScripts.Direction.North: directionValue = 1; break;
                case MoveScripts.Direction.South: directionValue = 0; break;
                case MoveScripts.Direction.East: directionValue = 2; break;
                case MoveScripts.Direction.West: directionValue = 3; break;
            }
            return (directionValue*spritesPerDirection);
        }

        public float getModulo(float time)
        {
            return (time % (iterateTime*spritesPerDirection));
        }
    }

    public List<Animation> animations;

    public Sprite GetAnimationSprite(AnimationType type, int index)
    {
        return animations[(int)type].spriteArray[index];
    }

    public int GetAnimationIndex(AnimationType type, MoveScripts.Direction direction, float time)
    {
        Animation animation = animations[(int)type];
        int result = animation.getDirectionStartIndex(direction);
        float modulo = animation.getModulo(time);

        result += (int)(modulo/animation.iterateTime);

        return result;
    }
}
