using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceScript : MonoBehaviour
{
    //選択肢のスクリプト

    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //『解答』ボタン
    [SerializeField]
    private GameObject _kaitouButton = default;
    //『解答』のスクリプト
    [SerializeField]
    private Kaitou _kaitou = default;

    private void Awake()
    {
        _kaitouButton = GameObject.FindWithTag("AnswerUI");
        _kaitou = _kaitouButton.GetComponent<Kaitou>();
    }
    private void Start()
    {
        _kaitouButton.SetActive(false);
    }
    public void OnClick()
    {
        //選択肢が選択されたら『解答』ボタンを表示
        _kaitouButton.SetActive(true);
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