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
    private SqliteDatabase[] _dBs = default;

    //○行の数
    private const int LINE = 10;

    //○詞の数
    private const int POS = 4;

    //データテーブル格納用
    private DataTable[,] _dTs = new DataTable[LINE, POS];

    private void Awake()
    {
        //配列の初期化
        _dBs = new SqliteDatabase[LINE];

        //データベースを配列に格納
        for (int i = 0; i < LINE; i++)
        {
            //_dBs[0]にア行、_dBs[1]にカ行...
            _dBs[i] = new SqliteDatabase("ainu_DB_" + $"{i}" + ".db");

            //2次元配列にそれぞれのSQL文を実行した結果を格納
            for (int j = 0; j < POS; j++)
            {
                switch (j)
                {
                    case 0:
                        //～名詞のテーブルを格納
                        _dTs[j, i] = _dBs[i].ExecuteQuery("SELECT * FROM Language WHERE PoS LIKE '％名詞'");
                        break;

                    case 1:
                        //～動詞のテーブルを格納
                        _dTs[j, i] = _dBs[i].ExecuteQuery("SELECT * FROM Language WHERE PoS LIKE '％動詞'");
                        break;

                    case 2:
                        //～副詞のテーブルを格納
                        _dTs[j, i] = _dBs[i].ExecuteQuery("SELECT * FROM Language WHERE PoS LIKE '％副詞'");
                        break;

                    case 3:
                        //その他のテーブルを格納
                        _dTs[j, i] = _dBs[i].ExecuteQuery("SELECT * FROM Language WHERE PoS NOT LIKE '％名詞' AND PoS NOT LIKE '％動詞' AND PoS NOT LIKE '副詞'");
                        break;
                }
            }
        }
    }
}
