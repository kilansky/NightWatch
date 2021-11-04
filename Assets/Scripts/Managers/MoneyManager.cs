using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MoneyManager : SingletonPattern<MoneyManager>
{
    [Header("Starting Money")]
    public int startingMoney = 1500;

    [Header("Penalties & Awards")]
    public int itemStolenPenalty = -500;
    public int thiefApprehendedAward = 150;
    public int endOfNightPayment = 300;

    [HideInInspector] public int money;
    public int Money { get { return money; } }

    // Start is called before the first frame update
    private void Start()
    {

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            money = startingMoney;
        }
        else
        {
            money = PlayerPrefs.GetInt("Money");
        }
        
        HUDController.Instance.moneyText.text = "Money: $" + money.ToString();
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        HUDController.Instance.moneyText.text = "Money: $" + money.ToString();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        print("Money is now at " + money);
        HUDController.Instance.moneyText.text = "Money: $" + money.ToString();
    }
}
