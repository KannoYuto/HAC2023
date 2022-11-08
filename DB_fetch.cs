using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NMeCab.Specialized;

public class DB_fetch : MonoBehaviour
{
    //データベース格納用変数
    private SqliteDatabase _dB = default;

    //分類する○詞の数
    private const int POS = 4;

    //データテーブル格納用配列
    private DataTable[] _dTs = default;

    //コンポーネント取得用変数
    [SerializeField]
    private InputField _iF = default;

    //モード選択用
    private enum SearchMode
    {
        Word,
        Sentence
    }
    [SerializeField]
    private SearchMode _searchMode = default;

    //形態素解析用辞書パス
    private const string DIC_DIR = @"Assets/NMeCab-0.10.2/dic/ipadic";

    //形態素解析結果格納用クラス
    public class Results
    {
        public string Surface { get; set; }
        public string PartsOfSpeech { get; set; }
        public string PartsOfSpeechSection1 { get; set; }
        public string PartsOfSpeechSection2 { get; set; }
    }

    //形態素解析結果格納用配列
    private Results[] _results = default;

    private List<List<string>> _translationResults = new List<List<string>>();

    [SerializeField]
    private int _index = default;

    [SerializeField]
    private List<string> _inList = new List<string>();

    private void Awake()
    {
        //データベースを格納
        _dB = new SqliteDatabase("ainu_DB_ALL.db");

        //配列を初期化
        _dTs = new DataTable[POS];

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
    }

    private void Update()
    {
        if (_translationResults.Count != 0)
        {
            _inList = _translationResults[_index];
        }
    }

    #region 形態素解析
    private List<Results> Analysis(string searchWord)
    {
        //何も入力されていなかったら
        if (searchWord == "")
        {
            return null;
        }

        //解析結果格納用
        List<Results> results = new List<Results>();

        //形態素解析した要素をリストに格納
        using (MeCabIpaDicTagger tagger = MeCabIpaDicTagger.Create(DIC_DIR))
        {
            MeCabIpaDicNode[] nodes = tagger.Parse(searchWord);

            foreach (MeCabIpaDicNode item in nodes)
            {
                Results result = new Results();
                result.Surface = $"{item.Surface}";
                result.PartsOfSpeech = $"{item.PartsOfSpeech}";
                result.PartsOfSpeechSection1 = $"{item.PartsOfSpeechSection1}";
                result.PartsOfSpeechSection2 = $"{item.PartsOfSpeechSection2}";

                print(result.Surface + " : " + result.PartsOfSpeech + " : " + result.PartsOfSpeechSection1 + " : " + result.PartsOfSpeechSection2);

                results.Add(result);
            }
        }

        return results;
    }
    #endregion

    #region 検索/翻訳開始ボタン
    public void StartSerch_OR_Analysis()
    {
        switch (_searchMode)
        {
            case SearchMode.Word:

                Search(_iF.text);

                break;

            case SearchMode.Sentence:

                if (Analysis(_iF.text) != null)
                {
                    _results = Analysis(_iF.text).ToArray();
                    Translation(_results);
                }

                break;
        }
    }
    #endregion

    #region 検索
    private void Search(string searchWord)
    {
        //何も入力されていなかったら
        if (searchWord == "")
        {
            return;
        }


    }
    #endregion

    #region 翻訳
    private void Translation(Results[] analysisResults)
    {
        for (int j = 0; j < _results.Length; j++)
        {
            DataTable query = default;

            if (_results[j].PartsOfSpeech.Contains("名詞"))
            {
                query = _dTs[0];
            }
            else if (_results[j].PartsOfSpeech.Contains("動詞"))
            {
                query = _dTs[1];
            }
            else if (_results[j].PartsOfSpeech.Contains("副詞"))
            {
                query = _dTs[2];
            }
            else
            {
                query = _dTs[3];
            }

            //翻訳結果格納用リスト
            List<string> translationResults = new List<string>();

            foreach (DataRow row in query.Rows)
            {
                string ainu = $"{row["Ainu"]}";
                string japanese = $"{row["Japanese"]}";

                if (japanese.Contains(_results[j].Surface))
                {
                    print(ainu);
                    translationResults.Add(ainu);
                }
            }

            if (translationResults.Count == 0)
            {
                print(_results[j].Surface);
                translationResults.Add(_results[j].Surface);
            }

            _translationResults.Add(translationResults);
        }

        _results = null;
    }
    #endregion
}
