using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NMeCab.Specialized;

public class DB_fetch : MonoBehaviour
{
    //データベース格納用
    private SqliteDatabase _dB = default;

    //○詞の数
    private const int POS = 4;

    //データテーブル格納用
    private DataTable[] _dTs = new DataTable[POS];

    private void Awake()
    {
        //データベースを格納
        _dB = new SqliteDatabase("ainu_DB_ALL.db");

        //それぞれのSQL文を実行した結果を配列に格納
        for (int i = 0; i < POS; i++)
        {
            switch (i)
            {
                case 0:
                    _dTs[i] = _dB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%名詞';");
                    break;

                case 1:
                    _dTs[i] = _dB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%動詞'");
                    break;

                case 2:
                    _dTs[i] = _dB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%副詞'");
                    break;

                case 3:
                    _dTs[i] = _dB.ExecuteQuery("SELECT * FROM Language WHERE PoS NOT like '%名詞' AND PoS NOT like '%動詞' AND PoS NOT like '%副詞'");
                    break;
            }
        }

        foreach (DataRow row in _dTs[2].Rows)
        {
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";
            string pos = $"{row["PoS"]}";

            print($"{ainu}" + " : " + $"{japanese}" + " [" + $"{pos}" + "]");
        }
    }
}
