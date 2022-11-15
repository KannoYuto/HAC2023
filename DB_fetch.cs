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

    //モード選択用
    private enum AnalysisMode
    {
        Search,
        Translation
    }
    [SerializeField]
    private AnalysisMode _analysisMode = default;

    //検索方式選択用
    private enum SearchMode
    {
        JapaneseToAinu,
        AinuToJapanese
    }
    [SerializeField]
    private SearchMode _searchMode = default;

    //形態素解析用辞書パス
    private const string DIC_DIR = @"Assets/NMeCab-0.10.2/dic/ipadic";

    //形態素解析結果格納用配列
    private AnalysisResults[] _analysisResults = default;

    //コンポーネント取得用変数
    [SerializeField]
    private InputField _iF = default;

    [SerializeField]
    private Text _text = default;

    #region 初期処理
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
    #endregion

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

                results.Add(result);
            }
        }

        return results;
    }
    #endregion

    #region 検索/翻訳開始ボタン
    public void StartSerch_OR_Analysis()
    {
        switch (_analysisMode)
        {
            case AnalysisMode.Search:

                //検索開始
                Search(_iF.text);

                break;

            case AnalysisMode.Translation:

                //テキストが入力されていたら
                if (Analysis(_iF.text) != null)
                {
                    //翻訳開始
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

        //問い合わせ
        DataTable query = _dB.ExecuteQuery("SELECT Ainu,Japanese FROM Language");

        //検索結果格納用リスト
        List<SearchResults> similaritySearchResults = new List<SearchResults>();

        //検索ワードを含むレコードを抽出
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

        //検索ワードと完全一致するレコードを格納するリスト
        List<SearchResults> exactMatch = new List<SearchResults>();

        //部分一致の中から完全一致するものを見つける
        for (int k = similaritySearchResults.Count - 1; k >= 0; k--)
        {
            switch (_searchMode)
            {
                case SearchMode.JapaneseToAinu:

                    //完全一致したら
                    if (similaritySearchResults[k].Japanese == searchWord)
                    {
                        exactMatch.Add(similaritySearchResults[k]);
                        similaritySearchResults.RemoveAt(k);
                    }

                    //、で区切られた要素が一致したら
                    else if (similaritySearchResults[k].Japanese.Contains("、"))
                    {
                        foreach (string item in similaritySearchResults[k].Japanese.Split('、'))
                        {
                            if (item == searchWord)
                            {
                                exactMatch.Add(similaritySearchResults[k]);
                                similaritySearchResults.RemoveAt(k);
                                break;
                            }
                        }
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

        //検索結果を出力する
        _text.text = null;

        _text.text = String.Concat(_text.text, "完全一致", "\n");

        for (int l = 0; l < exactMatch.Count; l++)
        {
            _text.text = String.Concat(_text.text, $"{l + 1}", ". ", exactMatch[l].Ainu, " : ", exactMatch[l].Japanese, "\n");
        }

        if (exactMatch.Count == 0)
        {
            _text.text = String.Concat(_text.text, "該当なし", "\n");
        }

        _text.text = String.Concat(_text.text, "", "\n", "部分一致", "\n");

        for (int m = 0; m < similaritySearchResults.Count; m++)
        {
            _text.text = String.Concat(_text.text, $"{m + 1}", ". ", similaritySearchResults[m].Ainu, " : ", similaritySearchResults[m].Japanese, "\n");
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

    public void JapaneseToAinu()
    {
        _searchMode = SearchMode.JapaneseToAinu;
    }

    public void AinuToJapanese()
    {
        _searchMode = SearchMode.AinuToJapanese;
    }
}