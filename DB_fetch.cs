using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DB_fetch : MonoBehaviour
{
    //データベース格納用変数
    private SqliteDatabase _ainuDB = default;

    //分類する○詞の数
    private const int POS = 4;

    //データテーブル格納用
    private DataTable _languageTable = default;

    //データテーブル格納用配列
    private DataTable[] _pOSTable = default;

    //検索方式選択用
    private enum SearchMode
    {
        JapaneseToAinu,
        AinuToJapanese
    }
    [SerializeField]
    private SearchMode _searchMode = default;

    //コンポーネント取得用変数
    [SerializeField]
    private InputField _iF = default;
    [SerializeField]
    private Text _text = default;

    #region 初期処理
    private void Awake()
    {
        //データベースを格納
        _ainuDB = new SqliteDatabase("ainu_DB_ALL.db");

        //アイヌ語、日本語のテーブルを作成
        _languageTable = _ainuDB.ExecuteQuery("SELECT Ainu,Japanese FROM Language");

        //配列を初期化
        _pOSTable = new DataTable[POS];

        //品詞ごとに分けられたテーブルを配列に格納
        for (int i = 0; i < POS; i++)
        {
            switch (i)
            {
                case 0:
                    //～名詞
                    _pOSTable[i] = _ainuDB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%名詞';");
                    break;

                case 1:
                    //～動詞
                    _pOSTable[i] = _ainuDB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%動詞'");
                    break;

                case 2:
                    //～副詞
                    _pOSTable[i] = _ainuDB.ExecuteQuery("SELECT * FROM Language WHERE PoS like '%副詞'");
                    break;

                case 3:
                    //上記以外の品詞
                    _pOSTable[i] = _ainuDB.ExecuteQuery("SELECT * FROM Language WHERE PoS NOT like '%名詞' AND PoS NOT like '%動詞' AND PoS NOT like '%副詞'");
                    break;
            }
        }
    }
    #endregion

    #region 検索開始ボタン
    public void StartSerch_OR_Analysis()
    {
        //検索開始
        Search(_iF.text);
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

        //検索結果格納用リスト
        List<SearchResults> similaritySearchResults = new List<SearchResults>();

        //検索ワードを含むレコードを抽出
        foreach (DataRow row in _languageTable.Rows)
        {
            //列の要素を文字列として取り出し
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";

            //抽出結果格納用リスト
            SearchResults searchResult = new SearchResults();

            //検索方式選択
            switch (_searchMode)
            {
                //日本語からアイヌ語
                case SearchMode.JapaneseToAinu:

                    /*
                     * 入力された日本語が要素中に含まれていたら
                     * 類似検索結果としてリストへ格納する
                     */
                    if (japanese.Contains(searchWord))
                    {
                        searchResult.Ainu = ainu;
                        searchResult.Japanese = japanese;
                        similaritySearchResults.Add(searchResult);
                    }

                    break;

                //アイヌ語から日本語
                case SearchMode.AinuToJapanese:

                    /*
                     * 入力されたアイヌ語が要素中に含まれていたら
                     * 類似検索結果としてリストへ格納する
                     */
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
            //検索方式選択
            switch (_searchMode)
            {
                //日本語からアイヌ語
                case SearchMode.JapaneseToAinu:

                    /*
                     * 完全一致したら結果をリストに格納し、
                     * 部分一致結果から削除する
                     */
                    if (similaritySearchResults[k].Japanese == searchWord)
                    {
                        exactMatch.Add(similaritySearchResults[k]);
                        similaritySearchResults.RemoveAt(k);
                    }

                    /*
                     *( 、)で区切られた要素が一致したら結果をリストに格納し、
                     * 部分一致結果から削除する
                     */
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

                //アイヌ語から日本語
                case SearchMode.AinuToJapanese:

                    /*
                     * 完全一致したら結果をリストに格納し、
                     * 部分一致結果から削除する
                     */
                    if (similaritySearchResults[k].Ainu == searchWord)
                    {
                        exactMatch.Add(similaritySearchResults[k]);
                        similaritySearchResults.RemoveAt(k);
                    }

                    break;
            }
        }

        //検索結果をテキストとして出力する
        _text.text = null;

        _text.text = String.Concat(_text.text, "\n", "完全一致", "\n");

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

        if (similaritySearchResults.Count == 0)
        {
            _text.text = String.Concat(_text.text, "該当なし", "\n");
        }
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