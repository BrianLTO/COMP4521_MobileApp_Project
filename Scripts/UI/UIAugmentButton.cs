using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAugmentButton : MonoBehaviour
{
    //assigned in inspector
    public int buttonID;

    // Start is called before the first frame update
    void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
    }

    //triggered on this button click
    void TaskOnClick()
    {
        UIUpgrade.instance.FocusAugment(buttonID);
    }
}
