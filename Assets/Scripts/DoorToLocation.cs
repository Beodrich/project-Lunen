using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;

public class DoorToLocation : MonoBehaviour
{
    [HideInInspector] public SceneAttributes attributes;

    public SceneReference DoorTarget;
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

    public int GetTargetSceneIndex()
    {
        if (DoorTarget.ScenePath != "")
        {
            return SceneUtility.GetBuildIndexByScenePath(DoorTarget.ScenePath);
        }
        else
        {
            return -1;
        }
        
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public GameScene GetTargetScene()
    {
        if (GetTargetSceneIndex () >= 0)
        {
            attributes.database.CreateScenesTo(GetTargetSceneIndex());
            return attributes.database.AllScenes[GetTargetSceneIndex()];
        }
        else return null;
        
    }

    public GameScene GetCurrentScene()
    {
        if (GetCurrentSceneIndex () >= 0)
        {
            attributes.database.CreateScenesTo(GetCurrentSceneIndex());
            return attributes.database.AllScenes[GetCurrentSceneIndex()];
        }
        else return null;
    }

    public string GetTargetScenePath()
    {
        return SceneUtility.GetScenePathByBuildIndex(GetTargetSceneIndex());
    }
}
