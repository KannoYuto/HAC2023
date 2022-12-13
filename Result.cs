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
    [SerializeField, Header("最後に表示するランクを入力(上から順に高得点)")]
    private string[] _rankText = default;
    [SerializeField, Header("ランク分けの基準となる数値を入れる(上から順に高得点のライン)")]
    private int[] _rankBorder = default;
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
        //最高ランク
        if(_score >= _rankBorder[0])
        {
            text.text = $"{_score}" + "問正解!\n" + $"{_rankText[0]}";
        }
        //2番目
        else if (_score >= _rankBorder[1])
        {
            text.text = $"{_score}" + "問正解!\n" + $"{_rankText[1]}";
        }
        //3番目
        else if (_score >= _rankBorder[2])
        {
            text.text = $"{_score}" + "問正解!\n" + $"{_rankText[2]}";
        }
        //4番目
        else
        {
            text.text = $"{_score}" + "問正解!\n" + $"{_rankText[3]}";
        }
    }
    public void ResetCount()
    {
        _score = 0;
    }
}
