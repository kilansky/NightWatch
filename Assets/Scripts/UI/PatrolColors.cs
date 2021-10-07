using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolColors : SingletonPattern<PatrolColors>
{
    public List<Color> patrolMarkerColors = new List<Color>();

    //Removes the first color in the patrolMarkerColors list and assigns it to the guard that called this function
    public Color SetGuardRouteColor()
    {
        Color nextColor = patrolMarkerColors[0];
        patrolMarkerColors.Remove(nextColor);

        return nextColor;
    }

    //Called when a guard is sold, and adds it's color back into the patrolMarkerColors list
    public void RemoveGuardRouteColor(Color removedColor)
    {
        patrolMarkerColors.Insert(0, removedColor);
    }

    //Removes a guard's current color and swaps it for the next one in the patrolMarkerColors list
    public Color ChangeRouteColor(Color removedColor)
    {
        RemoveGuardRouteColor(removedColor);
        return SetGuardRouteColor();
    }
}
