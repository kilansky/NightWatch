using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingAlert : MonoBehaviour
{
    public Image flashingImage;
    public float flashCycleTime;

    private float timeElapsed = 0;
    private float alpha = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlashCycle());
    }

    //Repeatedly fades an image in and out
    private IEnumerator FlashCycle()
    {
        while(true)//Repeat fade in/out forever
        {
            flashingImage.color = new Color(1, 1, 1, 1);

            //Fade to transparent
            while (timeElapsed < flashCycleTime)
            {
                alpha = Mathf.Lerp(1, 0, timeElapsed / flashCycleTime);
                flashingImage.color = new Color(1, 1, 1, alpha);
                timeElapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            flashingImage.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.25f);

            timeElapsed = 0;
            //Fade to opaque
            while (timeElapsed < flashCycleTime)
            {
                alpha = Mathf.Lerp(0, 1, timeElapsed / flashCycleTime);
                flashingImage.color = new Color(1, 1, 1, alpha);
                timeElapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            flashingImage.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
