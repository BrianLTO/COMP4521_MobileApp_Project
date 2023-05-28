using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIngame : MonoBehaviour
{
    public static UIIngame instance;

    //called when pause button is hit
    public void PauseGame()
    {
        GameController.instance.pauseGame();
    }

    //called when calibrate button is hit
    public void CalibrateMovement()
    {
        PlayerMovement.instance.Calibrate();
    }

    private void Awake()
    {
        instance = this;
    }
}
