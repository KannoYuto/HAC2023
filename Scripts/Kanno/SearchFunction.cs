using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NMeCab.Specialized;
using TMPro;
/// <summary>
/// 検索機能
/// </summary>
public class SearchFunction : MonoBehaviour
{
    #region 定数
    //データベース名
    private const string DATA_BASE_NAME = "ainu_DB_ALL_column.db";

    //アイヌ語、日本語問い合わせ
    private const string LANGUAGE_QUERY = "SELECT ColumnID,Ainu,Japanese,Reading FROM Language";

    private const string EXACT_MATCH = "完全一致";

    private const string PARTIALLY_CONSISTENT = "部分一致";

    private const string NONE_RESULT = "該当なし";

    //区切り文字(string型)
    private const string DELIMITER_STRING = "、";

    //区切り文字(char型)
    private const char DELIMITER_CHAR = '、';
    #endregion

    #region コンポーネント
    [SerializeField, Header("文字入力部")]
    private TMP_InputField _iF = default;

    [SerializeField, Header("結果出力部")]
    private TextMeshProUGUI[] _texts = new TextMeshProUGUI[4];

    [SerializeField, Header("コラム表示ボタン")]
    private Transform[] _buttons = new Transform[4];

    [SerializeField, Header("結果区分表示用")]
    private TextMeshProUGUI _resultText = default;

    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];

    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnNameText = default;

    [SerializeField, Header("コラムの単語の日本語を表示するテキストを入れる")]
    private TextMeshProUGUI _columnJapaneseText = default;

    [SerializeField, Header("コラムの単語の名前を表示するテキストを入れる")]
    private TextMeshProUGUI _columnReadText = default;

    [SerializeField, Header("コラムの内容を表示するテキストを入れる")]
    private TextMeshProUGUI _columnText = default;
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

    [SerializeField, Header("現在のページ数")]
    private int _nowPage = default;

    private List<SearchResults> _similaritySearchResults = new List<SearchResults>();

    private List<SearchResults> _exactMatch = new List<SearchResults>();

    [SerializeField, Header("キーボードを格納")]
    private Transform _keyBoard = default;

    [SerializeField, Header("左矢印")]
    private Image _leftArrow = default;

    [SerializeField, Header("右矢印")]
    private Image _rightArrow = default;
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

    //private string MorphologicalAnalysis(string word)
    //{
    //    string dicDir = @"Assets/NMeCab-0.10.2/dic/ipadic";

    //    string hiragana = null;

    //    List<string> analysisWords = new List<string>();

    //    using (MeCabIpaDicTagger tagger = MeCabIpaDicTagger.Create(dicDir))
    //    {
    //        MeCabIpaDicNode[] nodes = tagger.Parse(word);

    //        foreach (MeCabIpaDicNode item in nodes)
    //        {
    //            analysisWords.Add(item.Reading);
    //        }
    //    }

    //    foreach (string reading in analysisWords)
    //    {
    //        hiragana = String.Concat(hiragana, reading);
    //    }

    //    return hiragana;
    //}

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

        KeyBoardNonDisplay();

        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchInput");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }

        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchOutput");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }

        //類似検索結果格納用リスト
        List<SearchResults> similaritySearchResults = new List<SearchResults>();

        //検索ワードを含むレコードを抽出
        foreach (DataRow row in _languageTable.Rows)
        {
            //列の要素を文字列として取り出し
            string id = $"{row["ColumnID"]}";
            string ainu = $"{row["Ainu"]}";
            string japanese = $"{row["Japanese"]}";
            string reading = $"{row["Reading"]}";

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
                    if (japanese.Contains(_iF.text) || reading.Contains(_iF.text.ToHiragana()))
                    {
                        searchResult.ID = id;
                        searchResult.Ainu = ainu;
                        searchResult.Japanese = japanese;
                        searchResult.Reading = reading;
                        similaritySearchResults.Add(searchResult);
                    }

                    break;

                //アイヌ語から日本語
                case SearchMode.AinuToJapanese:

                    /*
                     * 入力されたアイヌ語が要素中に含まれていたら
                     * 類似検索結果としてリストへ格納する
                     */
                    if (ainu.Contains(_iF.text.ToKatakana()))
                    {
                        searchResult.ID = id;
                        searchResult.Ainu = ainu;
                        searchResult.Japanese = japanese;
                        searchResult.Reading = reading;
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
                    if (similaritySearchResults[i].Japanese == _iF.text || similaritySearchResults[i].Reading == _iF.text.ToHiragana())
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
                            if (item == _iF.text/* || MorphologicalAnalysis(item).ToHiragana() == _iF.text.ToHiragana()*/)
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
                    if (similaritySearchResults[i].Ainu == _iF.text.ToKatakana())
                    {
                        exactMatch.Add(similaritySearchResults[i]);
                        similaritySearchResults.RemoveAt(i);
                    }

                    break;
            }
        }

        _nowPage = 0;
        _resultText.text = EXACT_MATCH;
        _leftArrow.enabled = false;
        _rightArrow.enabled = true;
        _similaritySearchResults = similaritySearchResults;
        _exactMatch = exactMatch;

        if (_similaritySearchResults.Count == 0 && _exactMatch.Count == 0)
        {
            _resultText.text = NONE_RESULT;
            _leftArrow.enabled = false;
            _rightArrow.enabled = false;

            foreach (TextMeshProUGUI text in _texts)
            {
                text.text = "";
            }

            foreach(Transform button in _buttons)
            {
                button.GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            for (int k = 0; k < _texts.Length; k++)
            {
                if (k < _exactMatch.Count)
                {
                    _texts[k].text = $"{_exactMatch[k].Ainu} : {_exactMatch[k].Japanese}";

                    if (_exactMatch[k].ID != "")
                    {
                        _buttons[k].GetComponent<Image>().enabled = true;

                        string columnID = _exactMatch[k].ID;

                        _buttons[k].GetComponent<Button>().onClick.RemoveAllListeners();

                        _buttons[k].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                    }
                    else
                    {
                        _buttons[k].GetComponent<Image>().enabled = false;
                    }
                }
                else
                {
                    _texts[k].text = "";

                    _buttons[k].GetComponent<Image>().enabled = false;
                }
            }
        }

        //コラムボタンがないほうの再出題ボタンを表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _columnNameText.transform.parent.gameObject;
            //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }

        /*
         * 検索結果をテキストとして出力する
         * 検索結果を１、２...と連番で表示する
         * 検索結果が無かった時は該当なしと表示する
         */
        //_text.text = "\n" + "完全一致" + "\n";

        //for (int k = 0; k < exactMatch.Count; k++)
        //{
        //    _text.text = String.Concat(_text.text, $"{k + 1}", ". ", exactMatch[k].Ainu, " : ", exactMatch[k].Japanese, "\n");
        //}

        //if (exactMatch.Count == 0)
        //{
        //    _text.text = String.Concat(_text.text, "該当なし", "\n");
        //}

        //_text.text = String.Concat(_text.text, "", "\n", "部分一致", "\n");

        //for (int l = 0; l < similaritySearchResults.Count; l++)
        //{
        //    _text.text = String.Concat(_text.text, $"{l + 1}", ". ", similaritySearchResults[l].Ainu, " : ", similaritySearchResults[l].Japanese, "\n");
        //}

        //if (similaritySearchResults.Count == 0)
        //{
        //    _text.text = String.Concat(_text.text, "該当なし", "\n");
        //}
    }
    #endregion

    /// <summary>
    /// 現在のモードを取得
    /// </summary>
    /// <returns></returns>
    public int GetCurrentMode()
    {
        return (int)_searchMode;
    }

    public void KeyBoardOnDisplay()
    {
        if (_keyBoard.gameObject.activeSelf)
        {
            KeyBoardNonDisplay();
        }
        else
        {
            _keyBoard.gameObject.SetActive(true);
            _iF.interactable = false;
        }
    }

    public void KeyBoardNonDisplay()
    {
        _keyBoard.gameObject.SetActive(false);
        _iF.interactable = true;
    }

    public void Delete()
    {
        if (_iF.text.Length != 0)
        {
            _iF.text = _iF.text.Remove(_iF.text.Length - 1);
        }
    }

    public void Space()
    {
        _iF.text = String.Concat(_iF.text, "　");
    }

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

    public void SearchBack()
    {
        _iF.text = "";

        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchInput");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchOutput");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        switch (this.GetCurrentMode())
        {
            case 0:

                this.GetComponent<langch>().JapaneseToAinu().enabled = true;
                this.GetComponent<langch>().AinuToJapanese().enabled = false;

                break;

            case 1:

                this.GetComponent<langch>().JapaneseToAinu().enabled = false;
                this.GetComponent<langch>().AinuToJapanese().enabled = true;

                break;
        }
    }

    public void RightArrow()
    {
        if (4 * (_nowPage + 1) - 4 < _similaritySearchResults.Count)
        {
            _nowPage++;
            _resultText.text = PARTIALLY_CONSISTENT;
            _leftArrow.enabled = true;

            for (int l = 0; l < _texts.Length; l++)
            {
                switch (l)
                {
                    case 0:

                        if (4 * _nowPage - 4 < _similaritySearchResults.Count)
                        {
                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 4].Ainu} : {_similaritySearchResults[4 * _nowPage - 4].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 4].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 4].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            _texts[l].text = "";

                            _buttons[l].GetComponent<Image>().enabled = false;

                            _rightArrow.enabled = false;
                        }

                        break;

                    case 1:

                        if (4 * _nowPage - 3 < _similaritySearchResults.Count)
                        {
                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 3].Ainu} : {_similaritySearchResults[4 * _nowPage - 3].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 3].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 3].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            _texts[l].text = "";

                            _buttons[l].GetComponent<Image>().enabled = false;

                            _rightArrow.enabled = false;
                        }

                        break;

                    case 2:

                        if (4 * _nowPage - 2 < _similaritySearchResults.Count)
                        {
                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 2].Ainu} : {_similaritySearchResults[4 * _nowPage - 2].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 2].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 2].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            _texts[l].text = "";

                            _buttons[l].GetComponent<Image>().enabled = false;

                            _rightArrow.enabled = false;
                        }

                        break;

                    case 3:

                        if (4 * _nowPage - 1 < _similaritySearchResults.Count)
                        {
                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 1].Ainu} : {_similaritySearchResults[4 * _nowPage - 1].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 1].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 1].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }
                        }
                        else
                        {
                            _texts[l].text = "";

                            _buttons[l].GetComponent<Image>().enabled = false;

                            _rightArrow.enabled = false;
                        }

                        break;
                }

            }

            if (!(4 * _nowPage < _similaritySearchResults.Count))
            {
                _rightArrow.enabled = false;
            }
        }
    }

    public void LeftArrow()
    {
        if (_nowPage > 0)
        {
            _nowPage--;
            _resultText.text = PARTIALLY_CONSISTENT;

            _rightArrow.enabled = true;

            if (_nowPage == 0)
            {
                _resultText.text = EXACT_MATCH;

                _leftArrow.enabled = false;

                for (int k = 0; k < _texts.Length; k++)
                {
                    if (k < _exactMatch.Count)
                    {
                        _texts[k].text = $"{_exactMatch[k].Ainu} : {_exactMatch[k].Japanese}";

                        if (_exactMatch[k].ID != "")
                        {
                            _buttons[k].GetComponent<Image>().enabled = true;

                            string columnID = _exactMatch[k].ID;

                            _buttons[k].GetComponent<Button>().onClick.RemoveAllListeners();

                            _buttons[k].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                        }
                        else
                        {
                            _buttons[k].GetComponent<Image>().enabled = false;
                        }
                    }
                    else
                    {
                        _texts[k].text = "";

                        _buttons[k].GetComponent<Image>().enabled = false;
                    }
                }
            }
            else
            {
                for (int l = 0; l < _texts.Length; l++)
                {
                    switch (l)
                    {
                        case 0:

                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 4].Ainu} : {_similaritySearchResults[4 * _nowPage - 4].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 4].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 4].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }

                            break;

                        case 1:

                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 3].Ainu} : {_similaritySearchResults[4 * _nowPage - 3].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 3].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 3].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }

                            break;

                        case 2:

                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 2].Ainu} : {_similaritySearchResults[4 * _nowPage - 2].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 2].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 2].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }

                            break;

                        case 3:

                            _texts[l].text = $"{_similaritySearchResults[4 * _nowPage - 1].Ainu} : {_similaritySearchResults[4 * _nowPage - 1].Japanese}";

                            if (_similaritySearchResults[4 * _nowPage - 1].ID != "")
                            {
                                _buttons[l].GetComponent<Image>().enabled = true;

                                string columnID = _similaritySearchResults[4 * _nowPage - 1].ID;

                                _buttons[l].GetComponent<Button>().onClick.RemoveAllListeners();

                                _buttons[l].GetComponent<Button>().onClick.AddListener(() => ColumnOpen(columnID));
                            }
                            else
                            {
                                _buttons[l].GetComponent<Image>().enabled = false;
                            }

                            break;
                    }

                }
            }
        }
    }

    public void ColumnOpen(string id)
    {
        DataTable columnTable = _ainuDataBase.ExecuteQuery($"SELECT * FROM Column WHERE ColumnID = '{id}'");

        foreach (DataRow row in columnTable.Rows)
        {
            //アイヌ語を格納
            string ainu = $"{row["Ainu"]}";
            //コラムの内容を格納
            string columnText = $"{row["Columntext"]}";
            //コラムの内容を格納
            string columnJapanese = $"{row["Japanese"]}";
            //単語の読み方を格納
            string columnRead = $"{row["Pronunciation"]}";
            _columnJapaneseText.text = "【" + $"{columnJapanese}" + "】";
            _columnNameText.text = $"{ainu}";
            _columnReadText.text = $"{columnRead}";
            _columnText.text = $"{columnText}";
        }
        //コラムボタンがないほうの再出題ボタンを表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _columnNameText.transform.parent.gameObject;
            //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }
    }
    public void ColumnClose()
    {
        //コラムボタンがないほうの再出題ボタンを表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _columnNameText.transform.parent.gameObject;
            //親を除く、子のオブジェクトのイメージとテキストをすべて非表示
            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }

    public int NowPage()
    {
        return _nowPage;
    }

    public void InputOff()
    {
        _iF.interactable = false;
    }

    public void InputOn()
    {
        _iF.interactable = true;
    }
    #endregion
}