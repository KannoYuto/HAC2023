using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Re_Quiz : MonoBehaviour
{
    //正解と不正解のテキストを表示＆再出題するスクリプト

    //クイズのスクリプトがあるGameObject
    private GameObject _quizObj;
    //クイズのスクリプト
    private Quiz _quiz;

    //正解と不正解のテキスト
    [SerializeField]
    private GameObject _seikai = default;
    [SerializeField]
    private GameObject _fuseikai;
    private GameObject[] _Uis = new GameObject[3];

    private void Awake()
    {
        //クイズのスクリプトを取得
        _quizObj = GameObject.Find("Question_Text");
        _quiz = _quizObj.GetComponent<Quiz>();
        _seikai = GameObject.FindWithTag("CorrectAnswer");
        _fuseikai = GameObject.FindWithTag("IncorrectAnswer");
    }
    public void OnClick()
    {
        //正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswer");

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
            _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswer");

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
}
