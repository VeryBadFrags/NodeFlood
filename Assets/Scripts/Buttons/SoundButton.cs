using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour {

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    public GameObject checkmark;
    public AudioSource[] musics;
    

    void Start()
    {
        SetMuteValue();
    }

    public void SwitchSound()
    {
        GameModel.IS_MUTE = !GameModel.IS_MUTE;
       
        SetMuteValue();
        FindObjectOfType<MusicManager>().SetMusicMuteState();
    }

    void SetMuteValue()
    {
        // Mute the sound sources
        GameObject[] sounds = GameObject.FindGameObjectsWithTag("Sounds");
        foreach (GameObject sound in sounds)
        {
            sound.GetComponent<AudioSource>().mute = GameModel.IS_MUTE;
        }

        // Update the button's sprite
        if (GameModel.IS_MUTE)
        {
            checkmark.GetComponentInChildren<Image>().sprite = soundOffSprite;
        }
        else
        {
            checkmark.GetComponentInChildren<Image>().sprite = soundOnSprite;
        }
    }

}
