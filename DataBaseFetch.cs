using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBaseFetch : MonoBehaviour
{
    #region 定数
    //データベース名
    private const string DATA_BASE_NAME = "ainu_DB_ALL.db";

    //アイヌ語、日本語問い合わせ
    private const string LANGUAGE_QUERY = "SELECT Ainu,Japanese FROM Language";

    //区切り文字(string型)
    private const string DELIMITER_STRING = "、";

    //区切り文字(char型)
    private const char DELIMITER_CHAR = '、';
    #endregion

    #region コンポーネント
    [SerializeField, Header("文字入力部")]
    private InputField _iF = default;

    [SerializeField, Header("結果出力部")]
    private Text _text = default;
    #endregion

    #region フィールド変数
    //データベース格納用変数
    private SqliteDatabase _ainuDataBase = default;

    //データテーブル格納用
    private DataTable _languageTable = default;

    //検索方式選択用
    private enum SearchMode
    {
        JapaneseToAinu,
        AinuToJapanese
    }
    [SerializeField, Header("検索モード切り替え")]
    private SearchMode _searchMode = default;
    #endregion

    #region メソッド
    #region 初期処理
    /// <summary>
    /// 使用するデータベース、データテーブルの用意
    /// </summary>
    private void Awake()
    {
        //データベースを格納
        _ainuDataBase = new SqliteDatabase(DATA_BASE_NAME);

        //アイヌ語、日本語のテーブルを作成
        _languageTable = _ainuDataBase.ExecuteQuery(LANGUAGE_QUERY);
    }
    #endregion

    #region 検索
    /// <summary>
    /// 入力された内容をもとにデータテーブル内から検索する
    /// </summary>
    public void Search()
    {
        //何も入力されていなかったら
        if (_iF.text == "")
        {
            return;
        }

        //類似検索結果格納用リスト
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
                    if (japanese.Contains(_iF.text))
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
                    if (ainu.Contains(_iF.text))
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
        for (int i = similaritySearchResults.Count - 1; i >= 0; i--)
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
                    if (similaritySearchResults[i].Japanese == _iF.text)
                    {
                        exactMatch.Add(similaritySearchResults[i]);
                        similaritySearchResults.RemoveAt(i);
                    }

                    /*
                     *( 、)で区切られた要素が一致したら結果をリストに格納し、
                     * 部分一致結果から削除する
                     */
                    else if (similaritySearchResults[i].Japanese.Contains(DELIMITER_STRING))
                    {
                        foreach (string item in similaritySearchResults[i].Japanese.Split(DELIMITER_CHAR))
                        {
                            if (item == _iF.text)
                            {
                                exactMatch.Add(similaritySearchResults[i]);
                                similaritySearchResults.RemoveAt(i);
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
                    if (similaritySearchResults[i].Ainu == _iF.text)
                    {
                        exactMatch.Add(similaritySearchResults[i]);
                        similaritySearchResults.RemoveAt(i);
                    }

                    break;
            }
        }

        /*
         * 検索結果をテキストとして出力する
         * 検索結果を１、２...と連番で表示する
         * 検索結果が無かった時は該当なしと表示する
         */
        _text.text = "\n" + "完全一致" + "\n";

        for (int k = 0; k < exactMatch.Count; k++)
        {
            _text.text = String.Concat(_text.text, $"{k + 1}", ". ", exactMatch[k].Ainu, " : ", exactMatch[k].Japanese, "\n");
        }

        if (exactMatch.Count == 0)
        {
            _text.text = String.Concat(_text.text, "該当なし", "\n");
        }

        _text.text = String.Concat(_text.text, "", "\n", "部分一致", "\n");

        for (int l = 0; l < similaritySearchResults.Count; l++)
        {
            _text.text = String.Concat(_text.text, $"{l + 1}", ". ", similaritySearchResults[l].Ainu, " : ", similaritySearchResults[l].Japanese, "\n");
        }

        if (similaritySearchResults.Count == 0)
        {
            _text.text = String.Concat(_text.text, "該当なし", "\n");
        }
    }
    #endregion

    /// <summary>
    /// 検索方式切り替え用(日本語からアイヌ語)
    /// </summary>
    public void JapaneseToAinu()
    {
        _searchMode = SearchMode.JapaneseToAinu;
    }

    /// <summary>
    /// 検索方式切り替え用(アイヌ語から日本語)
    /// </summary>
    public void AinuToJapanese()
    {
        _searchMode = SearchMode.AinuToJapanese;
    }
    #endregion
}