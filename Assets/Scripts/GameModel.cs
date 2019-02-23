using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{

    static public int AI_DIFFICULTY = 1;

    static public bool IS_HUMAN = false;

    static public bool IS_MUTE = false;

    //TODO init the players here
    static public List<Player> players = new List<Player>();

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
