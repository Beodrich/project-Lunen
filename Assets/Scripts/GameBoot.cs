using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoot : MonoBehaviour
{
    [HideInInspector]
    public ListOfScenes sceneReference;
    public ListOfScenes.LocationEnum bootScene;
    // Start is called before the first frame update
    void Start()
    {
        sceneReference = GetComponent<ListOfScenes>();
        sceneReference.LoadScene(bootScene);
    }
}
