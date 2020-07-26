using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageSystem : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public List<GameData.PlayerLunen> StoredLunen;

    void Start()
    {
        sr = GetComponent<SetupRouter>();
    }

    public void StoreLunen(Monster monster)
    {
        StoredLunen.Add(GameData.GeneratePlayerLunen(monster, sr));
    }
}
