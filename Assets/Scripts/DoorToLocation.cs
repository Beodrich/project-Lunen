using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MyBox;

public class DoorToLocation : MonoBehaviour
{
    [HideInInspector] public SceneAttributes attributes;

    public GameScene targetScene;

    public int entranceIndex;
    public System.Guid entranceGuid;

    public MoveScripts.Direction exitDirection;
}
