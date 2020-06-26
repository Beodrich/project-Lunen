using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Tilemaps;

public class NukeTilemap : MonoBehaviour
{
    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private void WipeTiles()
    {
        Tilemap mapToClear = GetComponent<Tilemap>();
        mapToClear.ClearAllTiles();
    }
    #endif
}
