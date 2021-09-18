using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public int startingMoney = 1500;
    public TextMeshProUGUI moneyText;
    private int money;

    // Start is called before the first frame update
    void Start()
    {
        money = startingMoney;
        moneyText.text = money.ToString();
    }

    private void SubtractMoney(int amount)
    {
        money -= amount;
        moneyText.text = "Money: " + money.ToString();
    }

    private void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "Money: " + money.ToString();
    }
}
