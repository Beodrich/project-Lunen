using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class DoorToLocation : MonoBehaviour
{
    [HideInInspector] public SceneAttributes attributes;

    public GameScene targetScene;
    public Vector2 doorSize = new Vector2(1,1);

    public System.Guid thisGuid;
    public string thisGuidString;
    public string targetGuidString;

    public bool fadeOutOnTransition;
    public bool stopOnTransition;

    public MoveScripts.Direction exitDirection;

    private void OnDrawGizmos()
    {
        if (doorSize.x < 1) doorSize.x = 1;
        if (doorSize.y < 1) doorSize.y = 1;
        Vector3 center = transform.position+new Vector3(0.5f + (doorSize.x-1)/2, -0.5f - (doorSize.y-1)/2);
        Gizmos.color = new Color(1,0,1,0.8f);
        Gizmos.DrawCube(center, doorSize);
        DrawArrow.ForGizmo(center, MoveScripts.GetVector2FromDirection(exitDirection), new Color(1, 1, 1, 1f), 0.5f, 20f);
        GetComponent<BoxCollider2D>().offset = center-transform.position;
        GetComponent<BoxCollider2D>().size = doorSize;
    }
}
