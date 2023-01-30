using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class QuizController : MonoBehaviour
{
    //変更後の色
    private Color _pushColor = new Color32(200, 200, 200, 255);
    [SerializeField, Header("選択肢が自動で入る")]
    private GameObject[] _choise = new GameObject[4];
    //EventSystem(選択した選択肢を識別するため)
    [SerializeField, Header("EventSystemを入れる")]
    private EventSystem _eventSystem = default;
    //クイズのスクリプト
    private Quiz _quiz = default;
    //クイズのスクリプト
    private Result _result = default;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];
    [SerializeField, Header("コラムボタンがない方の再出題ボタンを入れる")]
    private GameObject _reQuizButtonSingle = default;

    private void Awake()
    {
        //クイズのスクリプトを取得
        _quiz = GameObject.FindGameObjectWithTag("QuestionText").GetComponent<Quiz>();
        _result = GameObject.FindGameObjectWithTag("EndText").GetComponent<Result>();
        //配列に選択肢を格納する
        GameObject[] choise = GameObject.FindGameObjectsWithTag("ChoiseUi");

        for (int i = 0; i < choise.Length; i++)
        {
            _choise[i] = choise[i].gameObject;
        }
    }
    #region 選択肢の色変える
    public void ColorChange()
    {
        //選択肢の色をリセット
        for (int i = 0; i < _choise.Length; i++)
        {
            _choise[i].gameObject.GetComponent<Image>().color = Color.white;
        }
        //押された選択肢の色を変える
        _eventSystem.currentSelectedGameObject.GetComponent<Image>().color = _pushColor;
    }
    #endregion

    #region 選択肢の色を戻す
    public void ResetButton()
    {
        //色を元に戻す
        for (int i = 0; i < _choise.Length; i++)
        {
            _choise[i].gameObject.GetComponent<Image>().color = Color.white;
        }
    }
    #endregion

    #region 再出題(リザルト以外で使用する)
    public void ReQuiz()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            //このタグのオブジェクトの親オブジェクトを取得
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnButton");
            //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //コラムボタンがないほうの再出題ボタンを表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _reQuizButtonSingle;
            //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //再出題
        _quiz.QuizReStart();
    }
    #endregion

    #region クイズを最初から始める時の処理(リザルトのボタンで使用する)
    public void Reset()
    {
        //リザルトのUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchConfirmation").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //最初にクイズのタイトルを非表示(髙橋)
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizTitle");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }

        //最初にクイズのタイトルを非表示(髙橋)
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //正解数をリセット
        _result.ResetCount();
    }
    #endregion

    #region モード変更時にクイズ機能を初期化
    public void ReStart()
    {
        //リザルトのUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _result.transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //正解数をリセット
        _result.ResetCount();
        //再出題
        _quiz.QuizReset();
    }
    #endregion

    #region クイズ開始時に非表示にする
    public void QuizInitialization()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswerText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        //不正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswerText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }
    #endregion
}
