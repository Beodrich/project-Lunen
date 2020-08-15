using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Template Scene", menuName = "GameElements/Template Scene")]
public class TemplateScene : ScriptableObject
{
    public SceneReference scene;
    public string generatePath;
}
