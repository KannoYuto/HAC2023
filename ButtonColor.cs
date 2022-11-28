using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColor : MonoBehaviour
{
    //選択中の解答の色を変える
    [SerializeField]
    private GameObject Button_1;
    [SerializeField]
    private GameObject Button_2;
    [SerializeField]
    private GameObject Button_3;
    [SerializeField]
    private GameObject Button_4;
    //選択中の選択肢の色を変える
    private Color PushColor = new Color32(200, 200, 200, 255);

    private void Awake()
    {
        Button_1 = GameObject.FindWithTag("ChoiceUI1");
        Button_2 = GameObject.FindWithTag("ChoiceUI2");
        Button_3 = GameObject.FindWithTag("ChoiceUI3");
        Button_4 = GameObject.FindWithTag("ChoiceUI4");
    }
    public void onClick_Button_1()
    {
        Button_1.GetComponent<Image>().color = PushColor;
        Button_2.GetComponent<Image>().color = Color.white;
        Button_3.GetComponent<Image>().color = Color.white;
        Button_4.GetComponent<Image>().color = Color.white;
    }

    public void onClick_Button_2()
    {
        Button_1.GetComponent<Image>().color = Color.white;
        Button_2.GetComponent<Image>().color = PushColor;
        Button_3.GetComponent<Image>().color = Color.white;
        Button_4.GetComponent<Image>().color = Color.white;
    }
    public void onClick_Button_3()
    {
        Button_1.GetComponent<Image>().color = Color.white;
        Button_2.GetComponent<Image>().color = Color.white;
        Button_3.GetComponent<Image>().color = PushColor;
        Button_4.GetComponent<Image>().color = Color.white;
    }

    public void onClick_Button_4()
    {
        Button_1.GetComponent<Image>().color = Color.white;
        Button_2.GetComponent<Image>().color = Color.white;
        Button_3.GetComponent<Image>().color = Color.white;
        Button_4.GetComponent<Image>().color = PushColor;
    }
    public void Reset_Button()
    {
        Button_1.GetComponent<Image>().color = Color.white;
        Button_2.GetComponent<Image>().color = Color.white;
        Button_3.GetComponent<Image>().color = Color.white;
        Button_4.GetComponent<Image>().color = Color.white;
    }
}