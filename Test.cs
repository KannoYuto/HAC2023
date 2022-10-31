using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NMeCab.Specialized;

public class Test : MonoBehaviour
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

        //配列にデータベースを格納
        for (int i = 0; i < _dBNumber; i++)
        {
            _dBs[i] = new SqliteDatabase("ainu_DB_" + $"{i}" + ".db");
        }
    }
    #region 日本語からアイヌ語変換
    private void JapaneseSearch(string searchWord)
    {
        if (searchWord == "")
        {
            print("テキストを入力してください");
            return;
        }

        for (int j = 0; j < _dBs.Length; j++)
        {
            SqliteDatabase currentDB = _dBs[j];
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
    }
    #endregion

    #region アイヌ語から日本語変換
    private void AinuSearch(string searchWord)
    {
        if (searchWord == "")
        {
            print("テキストを入力してください");
            return;
        }

        for (int j = 0; j < _dBs.Length; j++)
        {
            SqliteDatabase currentDB = _dBs[j];
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
    }
    #endregion

    #region 形態素解析
    private void MorphologicalAnalysis(string searchWord)
    {
        if (searchWord == "")
        {
            print("テキストを入力してください");
            return;
        }

        string dicDir = @"Assets/NMeCab-0.10.2/dic/ipadic";

        using (MeCabIpaDicTagger tagger = MeCabIpaDicTagger.Create(dicDir))
        {
            MeCabIpaDicNode[] nodes = tagger.Parse(searchWord);

            foreach (MeCabIpaDicNode item in nodes)
            {
                Debug.Log($"{item.Surface}, {item.PartsOfSpeech}, {item.PartsOfSpeechSection1}, {item.PartsOfSpeechSection2}");
            }
        }

        return;
    }
    #endregion

    public void SearchStart_Japanese()
    {
        _searchWord = _iF.text;
        JapaneseSearch(_searchWord);
    }

    public void SearchStart_Ainu()
    {
        _searchWord = _iF.text;
        AinuSearch(_searchWord);
    }

    public void StartAnalysis()
    {
        _searchWord = _iF.text;
        MorphologicalAnalysis(_searchWord);
    }
}
