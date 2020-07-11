using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DatabaseSceneEntrance
{
    public System.Guid guid;
    public string name;
    public Vector3Int position;
    public MoveScripts.Direction facingDirection;
}
