using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DB_fetch : MonoBehaviour
{
    //データベース格納用
    [SerializeField]
    private SqliteDatabase[] _dBs = default;

    [SerializeField, Header("データベースの個数")]
    private int _dBNumber = default;

    [SerializeField, Header("検索するワード")]
    private string _searchWord = default;

    [SerializeField, Header("コンポーネント取得用")]
    private InputField _iF = default;

    private void Awake()
    {
        //配列の初期化
        _dBs = new SqliteDatabase[_dBNumber];

        /*
         * 配列にデータベースを格納
         */
        for (int i = 0; i < _dBNumber; i++)
        {
            _dBs[i] = new SqliteDatabase("ainu_DB_" + $"{i}" + ".db");
        }
    }

    private void JapaneseSearch(string searchWord ,int lineX)
    {
        SqliteDatabase currentDB = _dBs[lineX];
        DataTable query = currentDB.ExecuteQuery("SELECT Ainu,Japanese FROM Language");

        foreach (DataRow row in query.Rows)
        {
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";
            if (searchWord != "" && japanese.Contains(searchWord))
            {
                print($"{ainu}" + " : " + $"{japanese}");
            }
            else
            {
                print("見つからなかった");
            }
        }
    }

    private void AinuSearch(string searchWord, int lineX)
    {
        SqliteDatabase currentDB = _dBs[lineX];
        DataTable query = currentDB.ExecuteQuery("SELECT Ainu,Japanese FROM Language");

        foreach (DataRow row in query.Rows)
        {
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";
            if (searchWord != "" && ainu.Contains(searchWord))
            {
                print($"{ainu}" + " : " + $"{japanese}");
            }
            else
            {
                print("見つからなかった");
            }
        }
    }

    public void SearchStart_Japanese()
    {
        _searchWord = _iF.text;
        JapaneseSearch(_searchWord, 0);
    }

    public void SearchStart_Ainu()
    {
        _searchWord = _iF.text;
        AinuSearch(_searchWord, 0);
    }
}
