using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Re_Quiz : MonoBehaviour
{
    //正解と不正解のテキストを表示＆再出題するスクリプト

    //クイズのスクリプトがあるGameObject
    private GameObject _quizObj;
    //クイズのスクリプト
    private Quiz _quiz;

    //正解と不正解のテキスト
    [SerializeField]
    private GameObject _seikai;
    [SerializeField]
    private GameObject _huseikai;

    private void Start()
    {
        //クイズのスクリプトを取得
        _quizObj = GameObject.Find("Question_Text");
        _quiz = _quizObj.GetComponent<Quiz>();
    }
    public void OnClick()
    {
        _huseikai.SetActive(false);
        _seikai.SetActive(false);
        //再出題
        _quiz.Quiz_act();
    }
}
