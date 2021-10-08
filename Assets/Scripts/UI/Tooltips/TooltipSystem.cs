using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : SingletonPattern<TooltipSystem>
{
    public Tooltip tooltip;

    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string header, string content)
    {
        tooltip.SetText(content, header);
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
