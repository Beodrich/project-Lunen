using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Track", menuName = "GameElements/Music Track")]
public class MusicTrack : ScriptableObject
{
    public AudioClip audioClip;
    public bool loops;
    public float startLoop;
    public float endLoop;

    public float loopLength
    {
        get
        {
            return (endLoop-startLoop);
        }
    }
}
