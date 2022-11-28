using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Kaitou : MonoBehaviour
{
    //解答のスクリプト

    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //正解と不正解のテキスト
    [SerializeField]
    private GameObject _seikai = default;
    [SerializeField]
    private GameObject _huseikai = default;

    public void OnClick()
    {
        //選択されたのが正解だったら
        if (isAnswer)
        {
            //正解のテキストを非表示
            _seikai.SetActive(true);
            //『解答』ボタンを非表示
            this.gameObject.SetActive(false);
        }
        //不正解なら
        else
        {
            //不正解のテキストを非表示
            _huseikai.SetActive(true);
            //『解答』ボタンを非表示
            this.gameObject.SetActive(false);
        }
    }
    public void SetBool(bool isAnswer)
    {
        this.isAnswer = isAnswer;
        return;
    }
}