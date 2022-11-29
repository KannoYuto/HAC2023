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
    [SerializeField]
    private GameObject _kaitouButton = default;
    private Image _kaitouUI = default;
    //『解答』のスクリプト
    [SerializeField]
    private Kaitou _kaitou = default;

    private void Awake()
    {
        _kaitouButton = GameObject.FindWithTag("AnswerUI");
        _kaitou = _kaitouButton.GetComponent<Kaitou>();
        _kaitouUI = _kaitouButton.GetComponent<Image>();
        _kaitouUI.GetComponent<Image>().enabled = false;
    }
    public void OnClick()
    {
        //選択肢が選択されたら『解答』ボタンを表示
        _kaitouUI.GetComponent<Image>().enabled = true;
        //選択された答えが正解か否かのフラグを設定
        _kaitou.SetBool(isAnswer);
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