using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NMeCab.Specialized;

public class DB_fetch : MonoBehaviour
{
    [SerializeField]
    private Transform _poolObj = default;

    [SerializeField]
    private ObjectPool _pool = default;

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
    private enum AnalysisMode
    {
        Search,
        Translation
    }
    [SerializeField]
    private AnalysisMode _analysisMode = default;

    private enum SearchMode
    {
        JapaneseToAinu,
        AinuToJapanese
    }
    [SerializeField]
    private SearchMode _searchMode = default;

    //形態素解析用辞書パス
    private const string DIC_DIR = @"Assets/NMeCab-0.10.2/dic/ipadic";

    //形態素解析結果格納用クラス
    public class AnalysisResults
    {
        public string Surface { get; set; }
        public string PartsOfSpeech { get; set; }
        public string PartsOfSpeechSection1 { get; set; }
        public string PartsOfSpeechSection2 { get; set; }
    }

    //形態素解析結果格納用配列
    private AnalysisResults[] _analysisResults = default;

    //検索結果格納用クラス
    public class SearchResults
    {
        public string Ainu { get; set; }
        public string Japanese { get; set; }
    }

    ////翻訳結果格納用二次元リスト
    //private List<List<string>> _translationResults = new List<List<string>>();

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

    #region 形態素解析
    private List<AnalysisResults> Analysis(string searchWord)
    {
        //何も入力されていなかったら
        if (searchWord == "")
        {
            return null;
        }

        //解析結果格納用
        List<AnalysisResults> results = new List<AnalysisResults>();

        //形態素解析した要素をリストに格納
        using (MeCabIpaDicTagger tagger = MeCabIpaDicTagger.Create(DIC_DIR))
        {
            MeCabIpaDicNode[] nodes = tagger.Parse(searchWord);

            foreach (MeCabIpaDicNode item in nodes)
            {
                AnalysisResults result = new AnalysisResults();
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
        foreach (Transform text in _poolObj)
        {
            if (text.gameObject.activeInHierarchy)
            {
                text.gameObject.SetActive(false);
            }
        }

        switch (_analysisMode)
        {
            case AnalysisMode.Search:

                Search(_iF.text);

                break;

            case AnalysisMode.Translation:

                if (Analysis(_iF.text) != null)
                {
                    _analysisResults = Analysis(_iF.text).ToArray();
                    Translation(_analysisResults);
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

        DataTable query = _dB.ExecuteQuery("SELECT Ainu,Japanese FROM Language");

        List<SearchResults> similaritySearchResults = new List<SearchResults>();

        foreach (DataRow row in query.Rows)
        {
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";

            SearchResults searchResult = new SearchResults();

            switch (_searchMode)
            {
                case SearchMode.JapaneseToAinu:

                    if (japanese.Contains(searchWord))
                    {
                        searchResult.Ainu = ainu;
                        searchResult.Japanese = japanese;
                        similaritySearchResults.Add(searchResult);
                    }

                    break;

                case SearchMode.AinuToJapanese:

                    if (ainu.Contains(searchWord))
                    {
                        searchResult.Ainu = ainu;
                        searchResult.Japanese = japanese;
                        similaritySearchResults.Add(searchResult);
                    }

                    break;
            }
        }

        List<SearchResults> exactMatch = new List<SearchResults>();

        for (int k = 0; k < similaritySearchResults.Count; k++)
        {
            switch (_searchMode)
            {
                case SearchMode.JapaneseToAinu:

                    if (similaritySearchResults[k].Japanese == searchWord)
                    {
                        exactMatch.Add(similaritySearchResults[k]);
                        similaritySearchResults.RemoveAt(k);
                    }

                    break;

                case SearchMode.AinuToJapanese:

                    if (similaritySearchResults[k].Ainu == searchWord)
                    {
                        exactMatch.Add(similaritySearchResults[k]);
                        similaritySearchResults.RemoveAt(k);
                    }

                    break;
            }
        }

        for (int l = 0; l < exactMatch.Count; l++)
        {
            _pool.GetPoolObject().GetComponent<Text>().text = exactMatch[l].Ainu + " : " + exactMatch[l].Japanese;
            _pool.GetPoolObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 700 - (100 * l));
            _pool.GetPoolObject().SetActive(true);
        }

        for (int m = 0; m < similaritySearchResults.Count; m++)
        {
            _pool.GetPoolObject().GetComponent<Text>().text = similaritySearchResults[m].Ainu + " : " + similaritySearchResults[m].Japanese;
            _pool.GetPoolObject().GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100 - (100 * m));
            _pool.GetPoolObject().SetActive(true);
        }
    }
    #endregion

    #region 翻訳
    private void Translation(AnalysisResults[] analysisResults)
    {
        for (int j = 0; j < _analysisResults.Length; j++)
        {
            DataTable query = default;

            if (_analysisResults[j].PartsOfSpeech.Contains("名詞"))
            {
                query = _dTs[0];
            }
            else if (_analysisResults[j].PartsOfSpeech.Contains("動詞"))
            {
                query = _dTs[1];
            }
            else if (_analysisResults[j].PartsOfSpeech.Contains("副詞"))
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

                if (japanese.Contains(_analysisResults[j].Surface))
                {
                    print(ainu);
                    translationResults.Add(ainu);
                }
            }

            if (translationResults.Count == 0)
            {
                print(_analysisResults[j].Surface);
                translationResults.Add(_analysisResults[j].Surface);
            }

            //_translationResults.Add(translationResults);
        }

        _analysisResults = null;
    }
    #endregion
}
