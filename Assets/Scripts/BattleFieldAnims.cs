using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldAnims : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public Image image;

    public int BattleFieldIndex;

    public AnimationSet currentSet;
    [HideInInspector] private string currentType = "Idle";
    public MoveScripts.Direction lookDirection;
    public float animTime;
    public int animIndex;

    void Awake()
    {
        GameObject main = GameObject.Find("BattleSetup");
        if (main != null) sr = main.GetComponent<SetupRouter>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.battleSetup.InBattle)
        {
            animTime += Time.deltaTime;
            if (currentSet != null)
            {
                animIndex = currentSet.GetAnimationIndex(currentType, lookDirection, animTime);
                image.sprite = currentSet.GetAnimationSprite(currentType, animIndex);
            }
        }
    }

    public void SetAnimationSet(AnimationSet set)
    {
        currentSet = set;
        animTime = 0;
        animIndex = 0;
        image.sprite = currentSet.GetAnimationSprite(currentType, animIndex);
        image.SetNativeSize();
    }

    public void DisableImage()
    {
        image.sprite = sr.database.transparentSprite;
        currentSet = null;
        //Debug.Log("Got to Disable Image Function");
    }
}
