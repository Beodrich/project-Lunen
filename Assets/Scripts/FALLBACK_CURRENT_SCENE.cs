using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FALLBACK_CURRENT_SCENE : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public string thisScene;

    public MoveScripts.Direction startDirection;
    
    void Awake()
    {
        #if UNITY_EDITOR // conditional compilation is not mandatory
            thisScene = SceneManager.GetActiveScene().path;
            
            GameObject main = GameObject.Find("BattleSetup");
            if (main == null)
            {
                this.transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
                SceneManager.LoadScene("_preload");
            }
            else
            {
                Destroy(this.gameObject);
            }
        #else
            Destroy(this.gameObject);
        #endif
        
    }
}
