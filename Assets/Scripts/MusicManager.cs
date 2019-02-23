using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioSource[] musics;

    private AudioSource currentMusic;

    // Use this for initialization
    void Start () {
        int randomSound = (int)Random.Range(0f, musics.Length);
        currentMusic = musics[randomSound % musics.Length];
        SetMusicMuteState();
        currentMusic.Play();
    }
	
	public void SetMusicMuteState()
    {
        currentMusic.mute = GameModel.IS_MUTE;
    }

}
