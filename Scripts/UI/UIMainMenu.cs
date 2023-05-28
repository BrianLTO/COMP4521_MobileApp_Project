using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public static UIMainMenu instance;

    //variables assigned in inspector
    public Button continueButton;
    public TextMeshProUGUI continueText;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //disable continue button if there is no save data for a play session
        if (GameController.instance.saveFile != null && !GameController.instance.saveFile.inGame)
        {
            continueButton.interactable = false;
            continueText.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public void UpdateContinueButton(bool hasSaveData)
    {
        if (hasSaveData)
        {
            //enable continue button if save data with a play session is detected
            continueButton.interactable = true;
            continueText.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            //disable continue button if there is no save data for a play session
            continueButton.interactable = false;
            continueText.color = new Color(1f, 1f, 1f, 0.5f);
        }

    }

    //called when start button is hit
    public void StartGame()
    {
        GameController.instance.startGame();
    }

    //called when continue button is hit
    public void StartGameFromSave()
    {
        GameController.instance.startGameFromSave();
    }

    //called when upgrade button is hit
    public void UpgradeMenu()
    {
        gameObject.SetActive(false);
        UIUpgradeMenu.instance.gameObject.SetActive(true);
    }
}
