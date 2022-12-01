using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScript : MonoBehaviour
{
    //選択肢のスクリプト

    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //『解答』ボタン
    [SerializeField, Header("解答ボタンが自動で入る")]
    private GameObject _answerButton = default;
    //『解答』ボタンのimage
    private Image _answerUI = default;
    //『解答』のスクリプト
    [SerializeField, Header("解答のスクリプトが自動で入る")]
    private Answer _answer = default;

    private void Awake()
    {
        //解答のオブジェクトを取得
        _answerButton = GameObject.FindWithTag("AnswerUI");
        //解答のスクリプトを取得
        _answer = _answerButton.GetComponent<Answer>();
        //解答のイメージを取得
        _answerUI = _answerButton.GetComponent<Image>();
        //解答を非表示
        _answerUI.GetComponent<Image>().enabled = false;
    }
    public void OnClick()
    {
        //選択肢が選択されたら『解答』ボタンを表示
        _answerUI.GetComponent<Image>().enabled = true;
        //選択された答えが正解か否かのフラグを設定
        _answer.SetBool(isAnswer); 
    }

    //選択肢に対して正誤をセット
    public void TrueSet()
    {
        isAnswer = true;
        return;
    }
    public void FalseSet()
    {
        isAnswer = false;
        return;
    }
}