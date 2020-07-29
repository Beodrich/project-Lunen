using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLog : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

     // Private VARS
     private List<string> Eventlog = new List<string>();
     private string guiText = "";
 
     // Public VARS
     public int maxLines = 10;
     public Font myFont;
     public float fadeOutTime;
     public float hideTime;

     void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
    }
     
     void OnGUI()
     {
         if (hideTime > 0 && sr.database.DebugMode)
         {
             GUIStyle myStyle = new GUIStyle();
            myStyle.font = myFont;
            myStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(1, 1, Screen.width/4, Screen.height / 3), guiText, myStyle);
            myStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(0, 0, Screen.width/4, Screen.height / 3), guiText, myStyle);
         }
         
     }

     public void Update()
     {
         hideTime -= Time.deltaTime;
     }
 
     public void AddEvent(string eventString)
     {
         Eventlog.Add(eventString);
         hideTime = fadeOutTime;
 
         if (Eventlog.Count >= maxLines)
             Eventlog.RemoveAt(0);
 
         guiText = "";
 
         foreach (string logEvent in Eventlog)
         {
             guiText += logEvent;
             guiText += "\n";
         }
     }
 }