using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRouter : MonoBehaviour
{
    public GameBoot gameBoot;
    public LunaDex lunaDex;
    public BattleSetup battleSetup;
    public Director director;
    public GenerateMonster generateMonster;
    public ListOfScenes listOfScenes;
    public CameraFollow cameraFollow;
    public CanvasCollection canvasCollection;
    public PlayerLogic playerLogic;

    public void Awake()
    {
        gameBoot.sr = battleSetup.sr = director.sr = generateMonster.sr = cameraFollow.sr = canvasCollection.sr = this;
    }
}
