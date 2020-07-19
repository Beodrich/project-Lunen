using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "New Database", menuName = "GameElements/Internal/Database")]
public class Database : ScriptableObject
{
    public int LevelCap;

    public List<GameScene> AllScenes;
    
    public List<Item> AllItems;
    public List<Lunen> AllLunen;
    public List<Action> AllActions;

    public List<CutsceneScript> GlobalCutsceneList;
    public List<StoryTrigger> GlobalStoryTriggerList;


    public Sprite transparentSprite;

    public GameObject MonsterTemplate;
    public GameObject EmoteTemplate;
    public GameObject Player;

    public void OnGameStart()
    {
        foreach (StoryTrigger st in GlobalStoryTriggerList)
        {
            st.ResetToDefaults();
        }
    }

    public int LunenToIndex(Lunen lunen)
    {
        for (int i = 0; i < AllLunen.Count; i++)
        {
            if (lunen == AllLunen[i]) return i;
        }
        return -1;
    }

    public Lunen IndexToLunen(int index)
    {
        return AllLunen[index];
    }

    public int ItemToIndex(Item item)
    {
        for (int i = 0; i < AllItems.Count; i++)
        {
            if (item == AllItems[i]) return i;
        }
        return -1;
    }

    public Item IndexToItem(int index)
    {
        return AllItems[index];
    }

    public int ActionToIndex(Action action)
    {
        for (int i = 0; i < AllActions.Count; i++)
        {
            if (action == AllActions[i]) return i;
        }
        return -1;
    }

    public Action IndexToAction(int index)
    {
        return AllActions[index];
    }

    public PackedCutscene GetPackedCutscene(int index)
    {
        if (index < GlobalCutsceneList.Count) return new PackedCutscene(GlobalCutsceneList[index]);
        else return null;
    }

    public PackedCutscene GetPackedCutscene(string find)
    {
        foreach (CutsceneScript cs in GlobalCutsceneList)
        {
            if (find == cs.name) return new PackedCutscene(cs);
        }
        Debug.Log("[ERROR] Unable To Find Cutscene: " + find);
        return null;
    }

    public string[] GetScenesArray()
    {
        List<string> sceneList = new List<string>();
        foreach (GameScene gs in AllScenes)
        {
            sceneList.Add(gs.name);
        }
        return sceneList.ToArray();
    }

    public GameScene IndexToGameScene(int index)
    {
        return AllScenes[index];
    }

    public StoryTrigger StringToStoryTrigger(string name)
    {
        foreach (StoryTrigger st in GlobalStoryTriggerList)
        {
            if (st.name == name) return st;
        }
        Debug.Log("Unable To Load Requested StoryTrigger: " + name);
        return null;
    }

    public string DialogueReplace(string _input)
    {
        string input = _input;
        Regex reg = new Regex("##[^#]+##");
        MatchCollection matches = reg.Matches(input);
        foreach (Match m in matches)
        {
            string path = m.Value;
            string OGpath = path;
            path = path.Trim('#');
            string newValue = GetTriggerValue(path).ToString();
            //Debug.Log(OGpath + " -> " + newValue);
            input = input.Replace(OGpath, newValue);
        }
        
        return input;
    }

    public object GetTriggerValue(string path)
    {
        string strTrigger = GetUntilOrEmpty(path, "/");
        string strPart = path.Replace(strTrigger, "");
        strPart = strPart.Trim('/');
        StoryTrigger st = StringToStoryTrigger(strTrigger);
        return st.GetTriggerValue(strPart);
    }

    public void SetTriggerValue(string path, object value)
    {
        string strTrigger = GetUntilOrEmpty(path, "/");
        string strPart = path.Replace(strTrigger, "");
        strPart = strPart.Trim('/');
        StoryTrigger st = StringToStoryTrigger(strTrigger);
        st.SetTriggerValue(strPart, value);
    }

    public static string GetUntilOrEmpty(string text, string stopAt = "-")
    {
        if (!System.String.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, System.StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return System.String.Empty;
    }
}
