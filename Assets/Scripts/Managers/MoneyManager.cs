using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MoneyManager : SingletonPattern<MoneyManager>
{
    [Header("Starting Money")]
    public int startingMoney = 800;

    [Header("Penalties & Awards")]
    public int itemStolenPenalty = -250;
    public int thiefApprehendedAward = 100;
    public int endOfNightPayment = 300;

    private int money;
    public int Money { get { return money; } }

    // Start is called before the first frame update
    private void Start()
    {
        //Set starting money if in day 1 of either building
        if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            money = startingMoney;
        }
        //Otherwise, use the money earned from the previous night
        else
        {
            money = PlayerPrefs.GetInt("Money", money);
        }

        print(HUDController.Instance.moneyText.text);
        print(money.ToString());

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
        HUDController.Instance.moneyText.text = "Money: $" + money.ToString();
    }

    public void ResetMoney()
    {
        money = startingMoney;
        HUDController.Instance.moneyText.text = "Money: $" + money.ToString();
    }
}
