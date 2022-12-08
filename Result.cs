using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField]
    private int _score = 0;
    [SerializeField,Header("正解時に加算するスコアの数値")]
    private int _addScore = 0;
    [SerializeField,Header("不正解時に減算するスコアの数値")]
    private int _subtractScore = 0;
    [SerializeField, Header("最後に正解数を表示するテキストが自動で入る")]
    private Text text = default;
    public void CorrectAnswerAdd()
    {
        _score += _addScore;
    }
    public void IncorrectAnswerAdd()
    {
        _score += _subtractScore;
    }
    public void ResultText()
    {
        text = this.GetComponent<Text>();
        text.text = "正解数は" + $"{_score}" + "問!";
    }
    public void ResetCount()
    {
        _score = 0;
    }
}
