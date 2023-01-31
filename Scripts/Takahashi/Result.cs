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
    private string[] _rankTexts = default;
    [SerializeField, Header("ランク分けの基準となる数値を入れる(上から順に高ランクのライン)")]
    private int[] _rankBorders = default;
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
    public void ResultText()
    {
        //最高ランク
        if (_score >= _rankBorders[0])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankTexts[0]}";
        }
        //2番目のランク
        else if (_score >= _rankBorders[1])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankTexts[1]}";
        }
        //3番目のランク
        else if (_score >= _rankBorders[2])
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankTexts[2]}";
        }
        //4番目のランク
        else
        {
            this.GetComponent<TextMeshProUGUI>().text = $"{_score}" + "問正解!\n" + $"{_rankTexts[3]}";
        }
    }
    public void ResetCount()
    {
        //スコアをリセット
        _quiz.Reset();
        _score = 0;
    }
}
