using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackedSecurityScript : MonoBehaviour
{
    public float hackBaseDuration;
    public float hackDurationMod;
    public GameObject objectBeingDisabled;
    public bool Hacked;
    private float hackDuration;
    

    private void Update()
    {
        if (Hacked)
        {
            hackDuration = hackBaseDuration;
            Hacked = false;
            StartCoroutine(Disabled());
        }
    }

    public void HackedFunction(int hackLevel)
    {
        hackDuration = hackBaseDuration + (hackDurationMod * hackLevel);
        
        StartCoroutine(Disabled());
    }

    private IEnumerator Disabled()
    {
        objectBeingDisabled.SetActive(false);
        yield return new WaitForSeconds(hackDuration);
        objectBeingDisabled.SetActive(true);
    }
}
