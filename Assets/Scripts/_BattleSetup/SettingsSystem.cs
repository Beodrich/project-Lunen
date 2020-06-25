using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class SettingsSystem : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public bool LoadSettings;

    [VectorLabels("Menu","Test","Set")]
    public Vector3Int ResolutionX;
    [VectorLabels("Menu","Test","Set")]
    public Vector3Int ResolutionY;
    [VectorLabels("Menu","Test","Set")]
    public Vector3Int Fullscreen;

    public bool TestOn;
    public float TestTime;
    public float TestTimeCurrent;

    public void Awake()
    {
        sr = GetComponent<SetupRouter>();
        RetrieveSettings();
    }

    public void RetrieveSettings()
    {
        if (LoadSettings)
        {
            if (PlayerPrefs.HasKey("ResolutionX")) ResolutionX.x = ResolutionX.y = ResolutionX.z = PlayerPrefs.GetInt("ResolutionX");
            if (PlayerPrefs.HasKey("ResolutionY")) ResolutionY.x = ResolutionY.y = ResolutionY.z = PlayerPrefs.GetInt("ResolutionY");
            if (PlayerPrefs.HasKey("Fullscreen")) Fullscreen.x = Fullscreen.y = Fullscreen.z = PlayerPrefs.GetInt("Fullscreen");
        }
    }

    public void TestSettings()
    {
        if (
            ResolutionX.x != ResolutionX.z ||
            ResolutionY.x != ResolutionY.z ||
            Fullscreen.x != Fullscreen.z
        )
        {
            TestOn = true;
            TestTimeCurrent = TestTime;
            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Options].SetPanelState("Apply Panel", UITransition.State.Enable);
            SetScreen(ResolutionX.x,ResolutionY.x,Fullscreen.x);
        }
        else
        {
            ApplySettings();
        }
        
    }

    public void ApplySettings()
    {
        //PlayerPrefs.SetInt("")
        ResolutionX.z = ResolutionX.y;
        ResolutionY.z = ResolutionY.y;
        Fullscreen.z = Fullscreen.y;

        PlayerPrefs.SetInt("ResolutionX", ResolutionX.z);
        PlayerPrefs.SetInt("ResolutionY", ResolutionY.z);
        PlayerPrefs.SetInt("Fullscreen", Fullscreen.z);

        TestOn = false;
        sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Options].SetPanelState("Apply Panel", UITransition.State.Disable);
    }

    public void RevertSettings()
    {
        if (TestOn)
        {
            SetScreen(ResolutionX.z, ResolutionY.z, Fullscreen.z);
        }
        TestOn = false;
        sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Options].SetPanelState("Apply Panel", UITransition.State.Disable);
    }
    
    public void ExitSettings()
    {
        if (TestOn) RevertSettings();
        sr.canvasCollection.OpenOptionsPanel("Game Settings Panel");
        sr.canvasCollection.OptionsPanelOpen = false;
        sr.canvasCollection.CloseState(CanvasCollection.UIState.Options);
        sr.canvasCollection.OpenState(CanvasCollection.UIState.MainMenu);
    }

    public void SetScreen(int resolutionX, int resolutionY, int fullscreen)
    {
        ResolutionX.y = resolutionX;
        ResolutionY.y = resolutionY;
        Fullscreen.y = fullscreen;

        Screen.SetResolution(resolutionX, resolutionY, (fullscreen == 1));
    }

    public void Update()
    {
        if (TestOn)
        {
            TestTimeCurrent -= Time.deltaTime;
            if (TestTimeCurrent < 0)
            {
                RevertSettings();
            }
        }
    }
}
