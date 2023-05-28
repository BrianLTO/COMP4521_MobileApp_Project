using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJoystick : MonoBehaviour
{
    public static UIJoystick instance { get; private set; }

    //variables assigned in inspector
    public GameObject childObject;
    public int maxDistance = 100;

    Image joystickBase;
    Image joystick;
    RectTransform child;
    RectTransform rect;
    Vector2 basePos { get { return rect.localPosition; } }

    //reset joystick on enabled
    private void OnEnable()
    {
        resetPosition();
    }

    void Awake()
    {
        instance = this;
        joystickBase = GetComponent<Image>();
        joystick = gameObject.transform.GetChild(0).GetComponent<Image>();
        child = childObject.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
    }

    //set joystick origin position on screen
    public void setOrigin(Vector2 position)
    {
        joystickBase.enabled = true;
        joystick.enabled = true;
        rect.localPosition = position;
    }

    //set joystick stick position on screen
    public void setStick(Vector2 position)
    {
        //dont allow the stick to be too far away from the base
        if ((position - basePos).magnitude > maxDistance) position = Vector2.ClampMagnitude(position - basePos, maxDistance);
        child.localPosition = position;
    }

    //disable the joystick, called when no touch is detected
    public void resetPosition()
    {
        joystickBase.enabled = false;
        joystick.enabled = false;
    }

}
