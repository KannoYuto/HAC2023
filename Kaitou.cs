using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Kaitou : MonoBehaviour
{
    //解答のスクリプト

    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //正解と不正解のテキスト
    [SerializeField]
    private GameObject _correctAnswer = default;
    [SerializeField]
    private GameObject _incorrectAnswer = default;
    private GameObject[] _Uis = new GameObject[3];
    private void Awake()
    {
    }
    public void OnClick()
    {
        //選択されたのが正解だったら
        if (isAnswer)
        {
            //正解のUIを見えなくする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswer");

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
            //『解答』ボタンを非表示
            this.gameObject.SetActive(false);
        }
        //不正解なら
        else
        {
            //不正解のUIを見えなくする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswer");

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