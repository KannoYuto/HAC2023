using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Result : MonoBehaviour
{
    [SerializeField]
    private int _score = 0;
    [SerializeField, Header("最後に表示するランクを入力(上から順に高得点)")]
    private string[] _rankText = default;
    [SerializeField, Header("ランク分けの基準となる数値を入れる(上から順に高ランクのライン)")]
    private int[] _rankBorder = default;
    private Quiz _quiz = default;

    private void Awake()
    {
        _quiz = GameObject.FindWithTag("QuestionText").GetComponent<Quiz>();
    }
    //正解時の加算処理
    public void CorrectAnswerAdd()
    {
        _score ++;
    }
    //不正解時の減算処理
    public void IncorrectAnswerAdd()
    {
    }
    public void ResultText()
    {
        //最高ランク
        if (_score >= _rankBorder[0])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankText[0]}";
        }
        //2番目
        else if (_score >= _rankBorder[1])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankText[1]}";
        }
        //3番目
        else if (_score >= _rankBorder[2])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankText[2]}";
        }
        //4番目
        else
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankText[3]}";
        }
    }
    public void ResetCount()
    {
        _quiz.Reset();
        _score = 0;
    }
}
