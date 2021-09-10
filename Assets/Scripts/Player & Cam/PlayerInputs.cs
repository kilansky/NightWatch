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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Interact Button Pressed
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed) //Checks if the interact button was pressed and then puts that on the queue for putting on the item
        {

        }
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
