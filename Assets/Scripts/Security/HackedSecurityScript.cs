using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackedSecurityScript : MonoBehaviour
{
    public float hackBaseDuration;
    public float hackDurationMod;
    [Range(0, 3)]public int hackResistance;
    public GameObject objectBeingDisabled;
    public bool Hacked;

    private float hackDuration;

    public void HackedFunction(int hackLevel)
    {
        hackDuration = hackBaseDuration + (hackDurationMod * hackLevel);
        objectBeingDisabled.GetComponent<Alert>().TimedDeactivation();
        StartCoroutine(Disabled());
    }

    private IEnumerator Disabled()
    {
        if (objectBeingDisabled.GetComponent<FieldOfView>())
            objectBeingDisabled.GetComponent<FieldOfView>().RemoveWaypoints();

        objectBeingDisabled.SetActive(false);
        yield return new WaitForSeconds(hackDuration);
        objectBeingDisabled.SetActive(true);

        if (objectBeingDisabled.GetComponent<FieldOfView>())
            StartCoroutine(objectBeingDisabled.GetComponent<FieldOfView>().FindTargetsWithDelay(0.25f));

        Hacked = false;
    }
}
