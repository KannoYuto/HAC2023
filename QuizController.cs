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
    [SerializeField]
    private GameObject[] _choise = new GameObject[4];
    //EventSystem
    [SerializeField] private EventSystem eventSystem;
    //クイズのスクリプトがあるGameObject
    private GameObject _quizObj = default;
    //クイズのスクリプト
    private Quiz _quiz;

    //正解と不正解のテキスト
    private GameObject[] _Uis = new GameObject[3];

    private void Awake()
    {
        //クイズのスクリプトを取得
        _quizObj = GameObject.Find("Question_Text");
        _quiz = _quizObj.GetComponent<Quiz>();
        GameObject[] choise = GameObject.FindGameObjectsWithTag("ChoiseUi");

        for (int i = 0; i < choise.Length; i++)
        {
            _choise[i] = choise[i].gameObject;
        }
    }
    //色変える
    public void ColorChange()
    {
        GameObject button_ob = eventSystem.currentSelectedGameObject;
        for (int i = 0; i < _choise.Length; i++)
        {
            _choise[i].gameObject.GetComponent<Image>().color = Color.white;
        }
        button_ob.gameObject.GetComponent<Image>().color = PushColor;
    }
    //UI消す
    public void ResetButton()
    {
        for (int i = 0; i < _choise.Length; i++)
        {
            _choise[i].gameObject.GetComponent<Image>().color = Color.white;
        }
    }
    //再出題
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
}
