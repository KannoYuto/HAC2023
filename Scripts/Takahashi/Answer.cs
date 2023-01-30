using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Answer : MonoBehaviour
{
    //解答のスクリプト

    //使用するデータベース
    private const string CONST_DATA = "ainu_DB_ALL_column.db";
    //データベースを格納
    private DataTable _dataTable = default;
    //正解の選択肢か否かのフラグ
    private bool isAnswer = false;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];
    [SerializeField, Header("リザルトを表示するテキストが自動で入る")]
    private Result _result = default;

    [SerializeField, Header("表示するコラムのオブジェクトを入れる")]
    private GameObject _column = default;
    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnNameText = default;
    [SerializeField, Header("コラムの単語の日本語を表示するテキストを入れる")]
    private TextMeshProUGUI _columnJapaneseText = default;
    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnReadText = default;
    [SerializeField, Header("コラムの内容を表示するテキストを入れる")]
    private TextMeshProUGUI _columnText = default;
    [SerializeField,Header("コラムボタンがない方の再出題ボタンを入れる")]
    private GameObject _reQuizButtonSingle = default;

    private void Awake()
    {
        _result = GameObject.FindWithTag("EndText").GetComponent<Result>();

        //データベースを引っ張り出す
        SqliteDatabase currentDB = new SqliteDatabase(CONST_DATA);
        //SQL文でクイズに使う単語を抽出(～や「」などの言葉を除外)
        _dataTable = currentDB.ExecuteQuery("SELECT * FROM Column");
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
                    else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
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
                    else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                    {
                        child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                    }
                }
            }
            _result.IncorrectAnswerAdd();
            //『解答』ボタンを非表示
            this.gameObject.GetComponent<Image>().enabled = false;
        }
        #endregion        

        //コラムボタンを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
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
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }

        //もしも単語のコラムが存在したらコラムボタンを非表示
        foreach (DataRow row in _dataTable.Rows)
        {
            
            if (GameObject.FindGameObjectWithTag("QuestionText").GetComponent<TextMeshProUGUI>().text == $"{row["Ainu"]}" || 
                GameObject.FindGameObjectWithTag("QuestionText").GetComponent<TextMeshProUGUI>().text == $"{row["Japanese"]}")
            {
                for (int i = 0; i < _Uis.Length; i++)
                {
                    _Uis[i] = GameObject.FindGameObjectWithTag("ColumnButton");
                    //コラムボタンを非表示
                    foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
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
                //アイヌ語を格納
                string ainu = $"{row["Ainu"]}";
                //コラムの内容を格納
                string columnText = $"{row["Columntext"]}";
                //コラムの内容を格納
                string columnJapanese = $"{row["Japanese"]}";
                //単語の読み方を格納
                string columnRead = $"{row["Pronunciation"]}";
                _columnJapaneseText.text = "【" + $"{columnJapanese}" + "】";
                _columnNameText.text = $"{ainu}";
                _columnReadText.text = $"{columnRead}";
                _columnText.text = $"{columnText}";
                return;
            }            
        }        
    }
    public void QuizColumnClick()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _column;

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
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }

        }
    }

    public void BackClick()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _column;

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

    public void SetBool(bool isAnswer)
    {
        this.isAnswer = isAnswer;
        return;
    }
    public void ResetBool()
    {
        isAnswer = false;
    }
}