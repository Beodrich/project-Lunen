using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveScripts
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static bool CheckForTag(GameObject self, Collider2D hit, string tag)
    {
        if (hit != null && hit.gameObject != self)
        {
            return (hit.gameObject.tag == tag);
        }
        else return false;
    }

    public static bool CheckForTag(GameObject self, Collider2D hit, List<string> tag)
    {
        for (int i = 0; i < tag.Count; i++)
        {
            if (CheckForTag(self, hit, tag[i])) return true;
        }
        return false;
    }

    public static bool CheckForTag(GameObject self, Collider2D[] hit, string tag)
    {
        if (hit.Length > 0)
        {
            int found = 0;
            for (int i = 0; i < hit.Length; i++)
            {
                found += CheckForTag(self, hit[i], tag) ? 1 : 0;
            }
            return (found > 0);
        }
        else return false;
    }

    public static bool CheckForTag(GameObject self, Collider2D[] hit, List<string> tag)
    {
        if (hit.Length > 0)
        {
            int found = 0;
            for (int i = 0; i < hit.Length; i++)
            {
                found += CheckForTag(self, hit[i], tag) ? 1 : 0;
            }
            return (found > 0);
        }
        else return false;
    }

    public static Vector2 DigitizeInput(Vector2 input)
    {
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
        {
            input.y = 0;
            if (input.x > 0) input.x = 1; else if (input.x < 0) input.x = -1;
        }
        else
        {
            input.x = 0;
            if (input.y > 0) input.y = 1; else if (input.y < 0) input.y = -1;
        }
        return input;
    }

    public static Vector2 DigitizeInput(Vector2 input, Vector2 lastInput, Vector2 lastMove)
    {
        Vector2 newInput = new Vector2(0,0);
        if (input.x != lastInput.x)
        {
            if (input.x > 0) newInput.x = 1; else if (input.x < 0) newInput.x = -1;
            else if (input.y > 0) newInput.y = 1; else if (input.y < 0) newInput.y = -1;
        }
        else if (input.y != lastInput.y)
        {
            if (input.y > 0) newInput.y = 1; else if (input.y < 0) newInput.y = -1;
            else if (input.x > 0) newInput.x = 1; else if (input.x < 0) newInput.x = -1;
        }
        else
        {
            if (input.x == 0 && input.y == 0) return newInput;
            else return lastMove;
        }
        return newInput;
    }

    public static Vector2 GetVector2FromDirection(Direction direction)
    {
        switch(direction)
        {
            default: return new Vector2(1,0);
            case Direction.North: return new Vector2(0,1);
            case Direction.South: return new Vector2(0,-1);
            case Direction.East: return new Vector2(1,0);
            case Direction.West: return new Vector2(-1,0);
        }
    }

    public static Direction GetDirectionFromVector2(Vector2 direction)
    {
        if (direction.x == 0)
        {
            if (direction.y == 1) return Direction.North; else return Direction.South;
        }
        else
        {
            if (direction.x == 1) return Direction.East; else return Direction.West;
        }
    }
    
    public static float GetAngleFromDirection(Direction direction)
    {
        switch(direction)
        {
            default: return 0f;
            case Direction.North: return 180f;
            case Direction.South: return 0f;
            case Direction.East: return 270f;
            case Direction.West: return 90f;
        }
    }

    public static Direction GetOppositeDirection(Direction direction)
    {
        switch(direction)
        {
            default: return Direction.North;
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
        }
    }

    public static Vector2 GetFrontVector2(Move move, float spaces, bool centered)
    {
        Vector2 result = move.gameObject.transform.position;
        result += GetVector2FromDirection(move.lookDirection) * spaces;
        if (centered) result += new Vector2(0.5f,-0.5f);
        return result;
    }
}
