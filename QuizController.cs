using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class QuizController : MonoBehaviour
{
    //変更後の色
    private Color PushColor = new Color32(200, 200, 200, 255);
    [SerializeField, Header("選択肢が自動で入る")]
    private GameObject[] _choise = new GameObject[4];
    //EventSystem(選択した選択肢を識別するため)
    [SerializeField,Header("EventSistemを入れる")] 
    private EventSystem eventSystem;
    //クイズのスクリプト
    private Quiz _quiz;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];

    private void Awake()
    {
        //クイズのスクリプトを取得
        _quiz = GameObject.Find("Question_Text").GetComponent<Quiz>();
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
        //押した選択肢を取得
        GameObject button_ob = eventSystem.currentSelectedGameObject;
        //選択肢の色をリセット
        for (int i = 0; i < _choise.Length; i++)
        {
            _choise[i].gameObject.GetComponent<Image>().color = Color.white;
        }
        //押された選択肢の色を変える
        button_ob.gameObject.GetComponent<Image>().color = PushColor;
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

    #region 再出題
    public void ReQuiz()
    {
        //正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswerText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
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
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
            }
        }
        //再出題
        _quiz.QuizAct();
    }
    #endregion
}
