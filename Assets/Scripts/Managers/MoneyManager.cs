using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : SingletonPattern<MoneyManager>
{
    public int startingMoney = 1500;
    public TextMeshProUGUI moneyText;

    private int money;
    public int Money { get { return money; } }

    // Start is called before the first frame update
    private void Start()
    {
        money = startingMoney;
        moneyText.text = "Money: $" + money.ToString();
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        moneyText.text = "Money: $" + money.ToString();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "Money: $" + money.ToString();
    }
}
