using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public GameObject fadepanel; //フェードパネルの取得

    Image fadealpha; //フェードパネルのイメージ取得変数

    private float alpha; //パネルのalpha値取得変数

    public bool flag;

    // Start is called before the first frame update
    private void Awake()
    {
        fadealpha = fadepanel.GetComponent<Image>();
        alpha = fadealpha.color.a;
        flag = false;
        Invoke("Flag", 2f);
    }

    void Flag()
    {
        flag = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (flag==true)
        {
            FadeOut();
        }
    }
    void FadeOut()
    {
        alpha -= 0.1f;
        fadealpha.color = new Color(1, 1, 1, alpha);
        if (alpha <= 0)
        {
            fadepanel.SetActive(false);
        }
    }
}
