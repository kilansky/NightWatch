using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectButtons : SingletonPattern<SelectedObjectButtons>
{
    //Sells the security measure that is currently selected
    public void SellButton()
    {
        Debug.Log("Sell Button Pressed");
        MoneyManager.Instance.AddMoney(SecuritySelection.Instance.selectedObject.cost);
        Destroy(SecuritySelection.Instance.selectedObject.gameObject);
        SecuritySelection.Instance.CloseSelection();
    }

    //Moves the security measure that is currently selected
    public void MoveButton()
    {
        Debug.Log("Move Button Pressed");
        SecurityPlacement.Instance.MovePlacedObject();
    }

    //Rotates the security measure that is currently selected
    public void RotateButton()
    {
        Debug.Log("Rotate Button Pressed");
    }

    //Allows setting up patrol points for the selected guard
    public void PatrolPointsButton()
    {
        Debug.Log("Patrol Points Button Pressed");
    }
}
