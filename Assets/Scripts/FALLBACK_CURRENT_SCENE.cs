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

    private void OnDrawGizmos()
    {
        DrawGizmoDisk(transform, 0.5f);
    }

    private float GIZMO_DISK_THICKNESS = 0.01f;
    public void DrawGizmoDisk(Transform t, float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.8f, 0.8f, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(t.position+new Vector3(0.5f, -0.5f), t.rotation, new Vector3(1, 1, GIZMO_DISK_THICKNESS));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
}
