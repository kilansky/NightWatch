using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : SingletonPattern<PlayerInputs>
{
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } } //True while game is paused

    private Vector3 cameraMovement = Vector3.zero;
    public Vector3 CameraMovement { get { return cameraMovement; } } //Returns the movement of the camera this frame

    private bool leftClickPressed;
    private bool leftClickReleased;
    private Vector2 aimPosition;
    public bool LeftClickPressed { get { return leftClickPressed; } } //True while left mouse button is held
    public bool LeftClickReleased { get { return leftClickReleased; } } //True for 1 frame when left mouse button is released
    public Vector2 AimPosition { get { return aimPosition; } } //Provides the x,y position of the mouse on screen

    //Aim Mouse Input
    public void PointMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            aimPosition = context.ReadValue<Vector2>();
        }
    }

    //Check for left clicking actions
    public void LeftClick(InputAction.CallbackContext context)
    {      
        if (context.performed && !leftClickPressed)//Left Click Pressed
        {
            leftClickPressed = true;
        }
        else if(leftClickPressed)//Left Click Released
        {
            leftClickPressed = false;
            leftClickReleased = true;

            StartCoroutine(ResetInput());
        }
    }

    //After 1 frame, reset the left click released input event
    private IEnumerator ResetInput()
    {
        yield return new WaitForEndOfFrame();
        leftClickReleased = false;
    }

    //WASD Pressed to move the camera
    public void CameraMove(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if any of the WASD keys were input
        {
            Vector2 cameraAdjustment = context.ReadValue<Vector2>();
            cameraMovement = new Vector3(cameraAdjustment.x, 0, cameraAdjustment.y);
        }
        else if (context.canceled) //When button is released, set movement back to zero
        {
            cameraMovement = Vector3.zero;
        }
    }

    //Scroll Wheel used to zoom the camera
    public void CameraZoom(InputAction.CallbackContext context)
    {
        float zoomInput = context.ReadValue<Vector2>().y;

        if (context.performed && zoomInput > 0)
            cameraMovement.y = -15;

        if (context.performed && zoomInput < 0)
            cameraMovement.y = 15;

        if(zoomInput == 0)
            cameraMovement.y = 0;
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
