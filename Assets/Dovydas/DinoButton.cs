using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DinoButton : MonoBehaviour
{
    public Text costText;
    public GameObject noMoneyText, maxReachedText;
    public GameObject selectedText;
    public int cost;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Select() {
        selectedText.SetActive(true);
    }

    public void Deselect()
    {
        selectedText.SetActive(false);
    }

    public void Show_NoMoney()
    {
        noMoneyText.SetActive(true);
    }

    public void Hide_NoMoney()
    {
        noMoneyText.SetActive(false);
    }

    public void Show_MaxReached()
    {
        maxReachedText.SetActive(true);
    }

    public void Hide_MaxReached()
    {
        maxReachedText.SetActive(false);
    }

    public void Show_HaveFree(int howMany)
    {
        costText.text = howMany + " FREE";
    }

    public void Hide_HaveFree()
    {
        costText.text = cost.ToString();
    }
}
