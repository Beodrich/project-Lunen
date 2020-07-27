using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class DoorToLocation : MonoBehaviour
{
    [HideInInspector] public SceneAttributes attributes;

    public GameScene targetScene;

    public System.Guid thisGuid;
    public string thisGuidString;
    public string targetGuidString;

    public bool fadeOutOnTransition;
    public bool stopOnTransition;

    public MoveScripts.Direction exitDirection;

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position+new Vector3(0.5f, -0.5f);
        Gizmos.color = new Color(1,0,1,0.8f);
        Gizmos.DrawCube(center, transform.localScale);
        DrawArrow.ForGizmo(center, MoveScripts.GetVector2FromDirection(exitDirection), new Color(1, 1, 1, 1f), 0.5f, 20f);
    }
}
