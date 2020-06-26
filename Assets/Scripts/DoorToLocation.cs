using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class DoorToLocation : MonoBehaviour
{
    public SceneReference TargetLocation;
    public int TargetEntrance;
    [Space(10)]
    [ReadOnly] [SerializeField] private int ThisSceneEntrance;

    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private void CreateSceneAttributesEntrance()
    {
        GameObject scene = GameObject.Find("SceneAttributes");
        if (scene != null)
        {
            SceneAttributes attributes = scene.GetComponent<SceneAttributes>();
            bool foundScene = false;
            int foundIndex = -1;
            for (int i = 0; i < attributes.sceneEntrances.Count; i++)
            {
                if (attributes.sceneEntrances[i].spawn == new Vector2(transform.position.x, transform.position.y))
                {
                    foundScene = true;
                    foundIndex = i;
                }
            }
            if (!foundScene)
            {
                SceneAttributes.Entrance newEntrance = new SceneAttributes.Entrance();
                newEntrance.spawn = transform.position;
                newEntrance.moveAtStart = true;
                foundIndex = attributes.sceneEntrances.Count;
                attributes.sceneEntrances.Add(newEntrance);
                ThisSceneEntrance = foundIndex;
                Debug.Log("Created New Entrance In SceneAttributes: Entrance " + foundIndex);
            }
            else
            {
                ThisSceneEntrance = foundIndex;
                Debug.Log("Entrance With Same Position Exists: Entrance " + foundIndex);
            }
        }
        else
        {
            Debug.Log("Couldn't find the SceneAttributes Object!");
        }
    }
    #endif
}
