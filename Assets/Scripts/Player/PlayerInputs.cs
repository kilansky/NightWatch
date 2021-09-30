using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : SingletonPattern<PlayerInputs>
{
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } } //True while the game is paused

    private Vector3 wasdMovement = Vector3.zero;
    [HideInInspector] public Vector3 WASDMovement { get { return wasdMovement; } } //Returns the movement vector of the camera this frame

    //Mouse Related Variables
    private bool leftClickPressed;
    private bool leftClickHeld;
    private bool leftClickReleased;
    private bool rightClickPressed;
    private bool rightClickHeld;
    private bool rightClickReleased;
    private Vector2 mousePos;
    private float scrollValue;
    [HideInInspector] public bool LeftClickPressed { get { return leftClickPressed; } }    //True for 1 frame when left mouse button is pressed
    [HideInInspector] public bool LeftClickHeld { get { return leftClickHeld; } }          //True while left mouse button is held
    [HideInInspector] public bool LeftClickReleased { get { return leftClickReleased; } }  //True for 1 frame when left mouse button is released
    [HideInInspector] public bool RightClickPressed { get { return rightClickPressed; } }  //True for 1 frame when right mouse button is pressed
    [HideInInspector] public bool RightClickHeld { get { return rightClickHeld; } }        //True while right mouse button is held
    [HideInInspector] public bool RightClickReleased { get { return rightClickReleased; } }//True for 1 frame when right mouse button is released
    [HideInInspector] public Vector2 MousePosition { get { return mousePos; } }            //Provides the x,y position of the mouse on screen
    [HideInInspector] public float ScrollingInput { get { return scrollValue; } }            //Provides the x,y position of the mouse on screen

    //Aim Mouse Input
    public void PointMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mousePos = context.ReadValue<Vector2>();
        }
    }

    //Check for left clicking actions
    public void LeftClick(InputAction.CallbackContext context)
    {      
        if (context.performed && !leftClickHeld)//Left Click Pressed
        {
            leftClickPressed = true;
            leftClickHeld = true;

            StartCoroutine(ResetPressedInput());
        }
        else if(leftClickHeld)//Left Click Released
        {
            leftClickHeld = false;
            leftClickReleased = true;

            StartCoroutine(ResetReleasedInput());
        }
    }

    //Check for right clicking actions
    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.performed && !rightClickHeld)//Right Click Pressed
        {
            rightClickPressed = true;
            rightClickHeld = true;

            StartCoroutine(ResetPressedInput());
        }
        else if (rightClickHeld)//Right Click Released
        {
            rightClickHeld = false;
            rightClickReleased = true;

            StartCoroutine(ResetReleasedInput());
        }
    }

    //After 1 frame, reset the click released input event
    private IEnumerator ResetPressedInput()
    {
        yield return new WaitForEndOfFrame();
        leftClickPressed = false;
        rightClickPressed = false;
    }

    //After 1 frame, reset the click released input event
    private IEnumerator ResetReleasedInput()
    {
        yield return new WaitForEndOfFrame();
        leftClickReleased = false;
        rightClickReleased = false;
    }

    //WASD Pressed to move the camera
    public void WASDInput(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if any of the WASD keys were input
        {
            Vector2 wasdInput = context.ReadValue<Vector2>();
            wasdMovement = new Vector3(wasdInput.x, 0, wasdInput.y);
        }
        else if (context.canceled) //When button is released, set movement back to zero
        {
            wasdMovement = Vector3.zero;
        }
    }

    //Scroll Wheel used to zoom the camera
    public void ScrollWheel(InputAction.CallbackContext context)
    {
        float scrollInput = context.ReadValue<Vector2>().y;

        if (context.performed && scrollInput > 0)
            scrollValue = -15f;

        if (context.performed && scrollInput < 0)
            scrollValue = 15f;

        if(scrollInput == 0)
            scrollValue = 0;
    }

    /// Pause function to pause the game based on the isPause variable and will stop the game time while displaying the pause screen
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed) //prevent pausing when dead
        {
            if (!IsPaused) //If the game is not paused then pause the game
            {
                isPaused = true;
                Time.timeScale = 0;
                HUDController.Instance.ShowPauseScreen();
            }
            else //If the game is paused then unpause the game
            {
                isPaused = false;
                Time.timeScale = 1;
                HUDController.Instance.HidePauseScreen();
            }
        }
    }
}
