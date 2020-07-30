using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public List<MusicSource> musicSource;
    public List<SoundSource> soundSource;
    public SoundEffect bootSoundEffectTest;
    public MusicTrack bootMusicTrackTest;
    
    [System.Serializable]
    public class MusicSource
    {
        public AudioSource source;
        public bool active;
        public MusicTrack track;
        public float currentTime;
    }

    [System.Serializable]
    public class SoundSource
    {
        public AudioSource source;
        public bool active;
        public SoundEffect sound;
        public float currentTime;
        public float durationTime;
    }

    void Start()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
    }

    private void Update()
    {
        foreach (SoundSource source in soundSource)
        {
            if (source.active)
            {
                source.currentTime = (float)source.source.timeSamples / source.sound.audioClip.frequency;
                if (source.currentTime >= source.durationTime)
                {
                    source.sound = null;
                    source.active = false;
                }
                else if (!source.source.isPlaying)
                {
                    source.sound = null;
                    source.active = false;
                }
            }
        }
        foreach (MusicSource source in musicSource)
        {
            if (source.active)
            {
                source.currentTime = (float)source.source.timeSamples / source.track.audioClip.frequency;
                if (source.track.loops)
                {
                    if (source.source.timeSamples > source.track.endLoop * source.track.audioClip.frequency)
                    {
                        source.source.timeSamples -= Mathf.RoundToInt(source.track.loopLength * source.track.audioClip.frequency);
                        source.currentTime -= source.track.loopLength;
                    }
                }
                else
                {
                    if (source.currentTime > source.track.audioClip.length)
                    {
                        source.track = null;
                        source.active = false;
                    }
                }
            }
        }
    }

    public void PlaySoundEffect(string title)
    {
        for (int i = 0; i < sr.database.GlobalSoundEffectList.Count; i++)
        {
            if (sr.database.GlobalSoundEffectList[i].name == title)
            {
                AddSFX(sr.database.GlobalSoundEffectList[i]);
                i = sr.database.GlobalSoundEffectList.Count;
            }
        }
    }

    public void AddSFX(SoundEffect _soundEffect, bool _loops = false)
    {
        for (int i = 0; i < soundSource.Count; i++)
        {
            if (!soundSource[i].active)
            {
                soundSource[i].active = true;
                soundSource[i].currentTime = 0;
                soundSource[i].durationTime = _soundEffect.audioClip.length;
                soundSource[i].sound = _soundEffect;
                soundSource[i].source.clip = soundSource[i].sound.audioClip;
                soundSource[i].source.Play();
                i = soundSource.Count;
            }
        }
        
    }

    public void PlayMusicTrack(MusicTrack track)
    {
        for (int i = 0; i < musicSource.Count; i++)
        {
            if (!musicSource[i].active)
            {
                musicSource[i].active = true;
                musicSource[i].currentTime = 0;
                musicSource[i].track = track;
                musicSource[i].source.clip = musicSource[i].track.audioClip;
                musicSource[i].source.Play();
                i = musicSource.Count;
            }
        }
    }
}
