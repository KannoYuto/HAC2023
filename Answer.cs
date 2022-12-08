using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Answer : MonoBehaviour
{
    //解答のスクリプト

    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];
    [SerializeField, Header("リザルトを表示するテキストが自動で入る")]
    private Result _result = default;

    private void Awake()
    {
        _result = GameObject.FindWithTag("EndText").GetComponent<Result>();
    }
    public void OnClick()
    {
        #region 選択されたのが正解だったら
        if (isAnswer)
        {
            //正解のUIを見えるようにする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                //このタグのオブジェクトの親オブジェクトを取得
                _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswerText").transform.parent.gameObject;
                //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
                foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
                {
                    if (child.gameObject.GetComponent<Image>())
                    {
                        child.gameObject.GetComponent<Image>().enabled = true;
                    }
                    else if (child.gameObject.GetComponent<Text>())
                    {
                        child.gameObject.GetComponent<Text>().enabled = true;
                    }
                }
            }
            _result.CorrectAnswerAdd();
            //『解答』ボタンを非表示
            this.gameObject.GetComponent<Image>().enabled = false;
        }
        #endregion

        #region 不正解なら
        else
        {
            //不正解のUIを見えるようにする処理
            //
            for (int i = 0; i < _Uis.Length; i++)
            {
                //このタグのオブジェクトの親オブジェクトを取得
                _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswerText").transform.parent.gameObject;
                //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
                foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
                {
                    if (child.gameObject.GetComponent<Image>())
                    {
                        child.gameObject.GetComponent<Image>().enabled = true;
                    }
                    else if (child.gameObject.GetComponent<Text>())
                    {
                        child.gameObject.GetComponent<Text>().enabled = true;
                    }
                }
            }
            _result.IncorrectAnswerAdd();
            //『解答』ボタンを非表示
            this.gameObject.GetComponent<Image>().enabled = false;
        }
        #endregion
    }
    public void SetBool(bool isAnswer)
    {
        this.isAnswer = isAnswer;
        return;
    }
}