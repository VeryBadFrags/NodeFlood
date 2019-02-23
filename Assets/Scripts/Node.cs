using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public int x, y;
    public int owner = -1;
    public int direction = 0;

    public void RotateNode()
    {
        direction = (direction + 1) % 4;
        // TODO Rotate sprite?
    }

    public void ChangeOwner(int newOwner)
    {
        owner = newOwner;
        // TODO Update sprite?
    }

}
