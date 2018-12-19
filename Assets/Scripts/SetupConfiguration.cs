using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupConfiguration : MonoBehaviour
{
    public bool[] playerActive = new bool[4];

    public void Active1(bool active){

        playerActive[0] = active;

    }
    public void Active2(bool active){

        playerActive[1] = active;

    }
    public void Active3(bool active){

        playerActive[2] = active;

    }
    public void Active4(bool active){

        playerActive[3] = active;

    }
}
