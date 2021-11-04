using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : SingletonPattern<PlayerInputs>
{
    //Keyboard Related Variables
    private Vector3 wasdMovement = Vector3.zero;
    private bool interact;
    private bool hotkey1;
    private bool hotkey2;
    private bool hotkey3;
    private bool hotkey4;
    [HideInInspector] public Vector3 WASDMovement { get { return wasdMovement; } } //Returns the movement vector of the camera this frame
    [HideInInspector] public bool Interact { get { return interact; } } //True for 1 frame when the interact (E) key is pressed           
    [HideInInspector] public bool Hotkey1 { get { return hotkey1; } }   //True for 1 frame when hotkey1 (1) is pressed           
    [HideInInspector] public bool Hotkey2 { get { return hotkey2; } }   //True for 1 frame when hotkey2 (2) is pressed           
    [HideInInspector] public bool Hotkey3 { get { return hotkey3; } }   //True for 1 frame when hotkey3 (3) is pressed           
    [HideInInspector] public bool Hotkey4 { get { return hotkey4; } }   //True for 1 frame when hotkey4 (4) is pressed           

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
    [HideInInspector] public float ScrollingInput { get { return scrollValue; } }          //Provides the scroll wheel value

    //Pause Variables
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } } //True while the game is paused
    public bool canPause = true;

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

            StartCoroutine(ResetPressedMouseInput());
        }
        else if(leftClickHeld)//Left Click Released
        {
            leftClickHeld = false;
            leftClickReleased = true;

            StartCoroutine(ResetReleasedMouseInput());
        }
    }

    //Check for right clicking actions
    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.performed && !rightClickHeld)//Right Click Pressed
        {
            rightClickPressed = true;
            rightClickHeld = true;

            StartCoroutine(ResetPressedMouseInput());
        }
        else if (rightClickHeld)//Right Click Released
        {
            rightClickHeld = false;
            rightClickReleased = true;

            StartCoroutine(ResetReleasedMouseInput());
        }
    }

    //After 1 frame, reset the click released input event
    private IEnumerator ResetPressedMouseInput()
    {
        yield return new WaitForEndOfFrame();
        leftClickPressed = false;
        rightClickPressed = false;
    }

    //After 1 frame, reset the click released input event
    private IEnumerator ResetReleasedMouseInput()
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

    //Interact key pressed
    public void InteractInput(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if the interact button was pressed
        {
            interact = true;
            StartCoroutine(ResetPressedKeyInput());
        }
    }

    //Hotkey 1 pressed
    public void Hotkey1Input(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if hotkey 1 was pressed
        {
            hotkey1 = true;
            StartCoroutine(ResetPressedKeyInput());
        }
    }

    //Hotkey 2 pressed
    public void Hotkey2Input(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if hotkey 2 was pressed
        {
            hotkey2 = true;
            StartCoroutine(ResetPressedKeyInput());
        }
    }

    //Hotkey 3 pressed
    public void Hotkey3Input(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if hotkey 3 was pressed
        {
            hotkey3 = true;
            StartCoroutine(ResetPressedKeyInput());
        }
    }

    //Hotkey 4 pressed
    public void Hotkey4Input(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if hotkey 4 was pressed
        {
            hotkey4 = true;
            StartCoroutine(ResetPressedKeyInput());
        }
    }

    //After 1 frame, reset the key pressed input event
    private IEnumerator ResetPressedKeyInput()
    {
        yield return new WaitForEndOfFrame();
        interact = false;
        hotkey1 = false;
        hotkey2 = false;
        hotkey3 = false;
        hotkey4 = false;
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
        if (context.performed && canPause) //prevent pausing when dead
        {
            if (!IsPaused) //If the game is not paused then pause the game
            {
                isPaused = true;
                Time.timeScale = 0;

                if(!GameManager.Instance.nightWatchPhase)
                    HUDController.Instance.ShowPauseScreen();
                else
                    NightHUDController.Instance.ShowPauseScreen();
            }
            else //If the game is paused then unpause the game
            {
                isPaused = false;
                Time.timeScale = 1;
                if (!GameManager.Instance.nightWatchPhase)
                    HUDController.Instance.HidePauseScreen();
                else
                    NightHUDController.Instance.HidePauseScreen();
            }
        }
    }

    //Continue button on pause screen pressed
    public void ContinueButtonPressed()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (!GameManager.Instance.nightWatchPhase)
            HUDController.Instance.HidePauseScreen();
        else
            NightHUDController.Instance.HidePauseScreen();
    }
}
