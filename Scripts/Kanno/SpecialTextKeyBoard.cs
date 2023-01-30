using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialTextKeyBoard : MonoBehaviour
{
    //アイヌ語特殊文字一覧
    private readonly string[] _specialTexts = { "セ゚",
                                                "ツ゚", "ト゚",
                                                "ァ", "ィ", "ゥ", "ェ", "ォ",
                                                "ㇰ",
                                                "ㇱ", "ㇲ",
                                                "ッ", "ㇳ",
                                                "ㇴ",
                                                "ㇵ", "ㇶ", "ㇷ", "ㇸ", "ㇹ",
                                                "ㇷ゚",
                                                "ㇺ",
                                                "ㇻ", "ㇼ", "ㇽ", "ㇾ", "ㇿ" };

    private readonly int[] _eachLineCounts = { 1, 2, 5, 1, 2, 2, 1, 5, 1, 1, 5 };

    [SerializeField, Header("生成するキー")]
    private GameObject _specialKey = default;

    [SerializeField, Header("入力部")]
    private TMP_InputField _iF = default;

    private void Awake()
    {
        for (int i = 0; i < _specialTexts.Length; i++)
        {
            GameObject generateButton = Instantiate(_specialKey, this.transform);

            int count = i;

            generateButton.GetComponent<Button>().onClick.AddListener(() => SetText(_specialTexts[count]));

            generateButton.GetComponentInChildren<Text>().text = _specialTexts[i];
        }
    }

    private void SetText(string outputText)
    {
        _iF.text = String.Concat(_iF.text, outputText);
    }
}
