using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LunenActionButton : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public Text Name;
    public Text TypeText;
    public Image ComboRibbon;
    public Image TypeRibbon;
    [HideInInspector] public Button button;
    [HideInInspector] public SpriteColorShift scs;
    [HideInInspector] public Image image;
    [HideInInspector] public Color defaultColor;
    public int MonsterIndex;
    public int ActionIndex;

    private void Awake()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        button = GetComponent<Button>();
        scs = GetComponent<SpriteColorShift>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void ToggleSCS()
    {
        scs.SetColorState(!scs.changeColor);
    }

    public void RestoreOriginalColor()
    {
        image.color = defaultColor;
    }

    public void ExecuteAction()
    {
        sr.canvasCollection.ExecuteAction(MonsterIndex, ActionIndex);
    }
}
