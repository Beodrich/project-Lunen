using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveDetection
{
    [System.Serializable]
    public class ColliderInfo
    {
        public GameObject gameObject;
        public string tag;

        public ColliderInfo(GameObject go)
        {
            this.gameObject = go;
            this.tag = go.tag;
        }
    }

    public List<ColliderInfo> colliderInfos;
    public MoveScripts.Direction moveDirection;
    public bool canMoveInWater;

    public bool inGrass = false;
    public GameObject grassObject;
    [Space(10)]
    public bool inDoor = false;
    public GameObject doorObject;
    [Space(10)]
    public bool inShop = false;
    public GameObject shopObject;
    [Space(10)]
    public bool inCutscene = false;
    public GameObject cutsceneObject;


    public void StartDetection(Collider2D[] hit, MoveScripts.Direction direction)
    {
        inGrass = false;
        inDoor = false;
        inShop = false;
        inCutscene = false;

        moveDirection = direction;
        colliderInfos = new List<ColliderInfo>();

        foreach (Collider2D c in hit) colliderInfos.Add(new ColliderInfo(c.gameObject));
        foreach (ColliderInfo ci in colliderInfos)
        {
            switch (ci.tag)
            {
                default: break;
                case "ShopKeeper":
                    inShop = true;
                    shopObject = ci.gameObject;
                    break;
                case "Grass":
                    inGrass = true;
                    grassObject = ci.gameObject;
                    break;
                case "Door":
                    inDoor = true;
                    doorObject = ci.gameObject;
                    break;
                case "Cutscene":
                    inCutscene = true;
                    cutsceneObject = ci.gameObject;
                    break;
            }
        }
    }

    public bool CanPhysicallyMove()
    {
        if (GetValidObjectCount() > 0) //First check if the area is just a void or not
        {
            if (inDoor) return true;
            if (GetValidWallCount() > 0) return false;
            if (GetValidNPCCount() > 0) return false;
            return true;
        }
        else return false;
    }

    private int GetValidObjectCount()
    {
        int total = 0;
        total += GetTotalWallCount();
        total += GetValidPathCount();
        return total;
    }

    //Get All Walls, Regardless of Walking Allowance
    private int GetTotalWallCount()
    {
        int total = 0;
        foreach (ColliderInfo ci in colliderInfos)
        {
            if (ci.tag == "Wall")
            {
                total++;
            }
            
        }
        return total;
    }

    //Get Walls That Block The Player In A Given Direction
    private int GetValidWallCount()
    {
        int total = 0;
        foreach (ColliderInfo ci in colliderInfos)
        {
            if (ci.tag == "Wall")
            {
                WallWalkScript wws = ci.gameObject.GetComponent<WallWalkScript>();
                if (wws != null)
                {
                    switch (moveDirection)
                    {
                        case MoveScripts.Direction.North: total += (wws.CanWalkNorth ? 0 : 1); break;
                        case MoveScripts.Direction.South: total += (wws.CanWalkSouth ? 0 : 1); break;
                        case MoveScripts.Direction.East: total += (wws.CanWalkEast ? 0 : 1); break;
                        case MoveScripts.Direction.West: total += (wws.CanWalkWest ? 0 : 1); break;
                    }
                }
                else
                {
                    total++;
                }
            }
            
        }
        return total;
    }

    private int GetValidNPCCount()
    {
        int total = 0;
        foreach (ColliderInfo ci in colliderInfos)
        {
            if (ci.tag == "Player" || ci.tag == "Trainer" || ci.tag == "NPC" || ci.tag == "Thing")
            {
                total++;
            }
            
        }
        return total;
    }

    private int GetValidPathCount()
    {
        int total = 0;
        foreach (ColliderInfo ci in colliderInfos)
        {
            if (ci.tag == "Path")
            {
                total++;
            }
            
        }
        return total;
    }

    public void DEBUG_PrintColliders()
    {
        foreach (ColliderInfo ci in colliderInfos)
        {
            Debug.Log("Object: " + ci.gameObject.name + " | Tag: " + ci.tag);
            
        }
    }

}
