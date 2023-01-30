using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class langch : MonoBehaviour
{
    [SerializeField, Header("アプリコントローラーからスクリプトを取得")]
    private SearchFunction _aC = default;

    [Header("検索方式切り替えUI")]
    [SerializeField]
    private Image _japaneseToAinu = default;
    [SerializeField]
    private Image _ainuToJapanese = default;

    private void Awake()
    {
        _aC = this.GetComponent<SearchFunction>();

        _japaneseToAinu.enabled = true;
        _ainuToJapanese.enabled = false;
    }

    public void OnClick()
    {
        if (!_japaneseToAinu.enabled)
        {
            _aC.JapaneseToAinu();

            _japaneseToAinu.enabled = true;
            _ainuToJapanese.enabled = false;
        }
        else
        {
            _aC.AinuToJapanese();

            _japaneseToAinu.enabled = false;
            _ainuToJapanese.enabled = true;
        }
    }

    public Image JapaneseToAinu()
    {
        return _japaneseToAinu;
    }

    public Image AinuToJapanese()
    {
        return _ainuToJapanese;
    }
}
