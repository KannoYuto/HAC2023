using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

public class ColumnText : MonoBehaviour
{
    //使用するデータベース
    private const string CONST_DATA = "ainu_DB_ALL_column.db";
    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnNameText = default;
    [SerializeField, Header("コラムの単語の日本語を表示するテキストを入れる")]
    private TextMeshProUGUI _columnJapaneseText = default;
    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnReadText = default;
    [SerializeField, Header("コラムの内容を表示するテキストを入れる")]
    private TextMeshProUGUI _columnText = default;
    //データベースを格納
    private DataTable _dataTable = default;
    [SerializeField, Header("EventSystemを入れる")]
    private EventSystem _eventSystem = default;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];

    private void Awake()
    {
        //データベースを引っ張り出す
        SqliteDatabase currentDB = new SqliteDatabase(CONST_DATA);
        //SQL文でクイズに使う単語を抽出(～や「」などの言葉を除外)
        _dataTable = currentDB.ExecuteQuery(
            "SELECT Ainu,Columntext,Pronunciation,Japanese " +
            "FROM Column ");
    }
    public void OnClick()
    {       
        foreach (DataRow row in _dataTable.Rows)
        {
            
            if (_eventSystem.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text == $"{row["Ainu"]}")
            {
                //アイヌ語を格納
                string ainu = $"{row["Ainu"]}";
                //コラムの内容を格納
                string columnText = $"{row["Columntext"]}";
                //コラムの内容を格納
                string columnJapanese = $"{row["Japanese"]}";
                //単語の読み方を格納
                string columnRead = $"{row["Pronunciation"]}";
                _columnJapaneseText.text = "【" + $"{columnJapanese}"+ "】";
                _columnNameText.text = $"{ainu}";
                _columnReadText.text = $"{columnRead}";
                _columnText.text = $"{columnText}";
            }
        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText").transform.parent.gameObject;

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
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }

        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText");

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
}
