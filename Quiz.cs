using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Quiz : MonoBehaviour

//出題するスクリプト

{
    //使用するデータベース
    private const string CONST_DATA = "ainu_DB_ALL_column.db";
    [SerializeField, Header("問題用のテキストが自動で入る")]
    private TextMeshProUGUI _quizText = default;
    [SerializeField, Header("正解のテキストが自動で入る")]
    private TextMeshProUGUI _correctAnswerText = default;
    [SerializeField, Header("不正解のテキストが自動で入る")]
    private TextMeshProUGUI _incorrectAnswerText = default;
    [SerializeField, Header("クイズの問題数の最大値を入力")]
    private int _quizMax = 0;
    [SerializeField, Header("クイズ用のデータの行を自動でカウント")]
    private int _dataCount = 0;
    [SerializeField, Header("クイズで使用するデータの最大値を入れる")]
    private int _maxDate = default;
    [SerializeField, Header("一度のクイズで使用できるヒントの回数")]
    private int _hintNumberofuses = default;
    private  int _hintCount = default;
    [SerializeField, Header("解答群が自動で入る")]
    private TextMeshProUGUI[] _choises = new TextMeshProUGUI[4]; 
    [SerializeField, Header("正解の番号が自動で入る")]
    private int[] _answerNumbers = default;
    [SerializeField, Header("第〇問を表示するテキストを入れる")]
    private TextMeshProUGUI _countText = default;
    [SerializeField, Header("タイマーを入れる")]
    private Timer _timer = default;
    [SerializeField, Header("リザルトのテキストが自動で入る")]
    private Result _resultText = default;
    [SerializeField, Header("ヒントボタンを入れる")]
    private GameObject _hintButton = default;
    [SerializeField, Header("ヒントの回数を表示するテキストを入れる")]
    private GameObject _hintCountText = default; 
    [SerializeField, Header("解答ボタンを入れる")]
    private Answer _answerButton = default; 
    //回答群の数
    private int _textCount = 0;
    //ダミーの数
    private int _trapWordCount = 0;
    //何問目かカウント
    private int _quizCount = 0;
    //選択肢のスクリプト
    private ChoiceScript _choiseScript = default;
    //データベースを格納
    private DataTable _dataTable = default;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];
    //正解の配列番号を格納する
    private int _incorrectAnswerNumber = default;
    //ヒント発動時に消す選択肢の番号を格納する
    private int _hintButtonNumber = default;

    private enum Mode
    {
        Ainu,
        Japanese,
        NoStart
    }
    [SerializeField, Header("モード")]
    private Mode mode = default;

    private void Awake()
    {
        //データベースを引っ張り出す
        SqliteDatabase currentDB = new SqliteDatabase(CONST_DATA);
        //SQL文でクイズに使う単語を抽出(～や「」などの言葉を除外)
        _dataTable = currentDB.ExecuteQuery(
            "SELECT Ainu,Japanese " +
            "FROM Language " +
            "WHERE  Japanese NOT like '%～%'" +
            "AND Japanese NOT like '%「%'" +
            "AND Japanese NOT like '%/%'" +
            "AND Japanese NOT like '%（%'" +
            "AND Japanese NOT like 'に%'" +
            "AND Japanese NOT like '%物音%'" +
            "AND Japanese NOT like '%複数形%'" +
            "AND Affiliation is NULL GROUP by Japanese");

        //問題のテキストを取得
        _quizText = this.GetComponent<TextMeshProUGUI>();
        //正解のテキストを取得
        _correctAnswerText = GameObject.FindWithTag("CorrectAnswerText").GetComponent<TextMeshProUGUI>();
        //不正解のテキストを取得
        _incorrectAnswerText = GameObject.FindWithTag("IncorrectAnswerText").GetComponent<TextMeshProUGUI>();
        //リザルトのテキストを取得
        _resultText = GameObject.FindWithTag("EndText").GetComponent<Result>();
        //正解の番号を入れる配列の長さを出題数と同じにする
        _answerNumbers = new int[_quizMax];
        //配列に選択肢のテキストを格納する
        GameObject[] choise = GameObject.FindGameObjectsWithTag("ChoiseText");
        //初期の時点ではクイズを開始していない状態にする
        mode = Mode.NoStart;
        for (int i = 0; i < choise.Length; i++)
        {
            _choises[i] = choise[i].GetComponent<TextMeshProUGUI>();
        }
    }

    public void QuizReset()
    {
        //出題数を初期化
        _quizCount = 0;
        _hintCount = _hintNumberofuses;
        //ヒントの残り回数の表示をセット
        _hintCountText.GetComponent<TextMeshProUGUI>().text = $"{_hintCount}" + "/" + $"{_hintNumberofuses}";
        QuizReStart();
    }

    //問題数のカウントから始める出題処理(一つの問題につき一度しかしなくてよい処理)
    public void QuizReStart()
    {
        #region 出題時に一度だけする処理
        //何問めかのカウントをアップ
        _quizCount++;
        #region 問題数が最大値までに達した時の処理
        if (_quizCount > _quizMax)
        {
            _quizText.text = "";
            _quizCount = 0;
            //問題数を表示を初期化
            _countText.text = "第" + $"{_quizCount}" + "問";
            for (int i = 0; i < _choises.Length; i++)
            {
                _choises[i].text = "";
            }
            for (int i = 0; i < _answerNumbers.Length; i++)
            {
                _answerNumbers[i] = 0;
            }
            //正解のUIを見えなくする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswerText").transform.parent.gameObject;

                foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
            //不正解のUIを見えなくする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswerText").transform.parent.gameObject;

                foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
            //終了のUIを見えなくする処理
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("EndText").transform.parent.gameObject;

                foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
            _resultText.ResultText();
            return;
        }
        #endregion

        //問題数を表示
        _countText.text = "第" + $"{_quizCount}" + "問";
        #endregion

        #region 選択肢をシャッフルする処理
        //選択肢の配列の長さを数値として格納
        int choisesLength = _choises.Length;
        while (choisesLength > 1)
        {
            //カウントを減らす
            choisesLength--;
            //入れ替える選択肢を選ぶ
            int changeChoises = Random.Range(0, choisesLength);
            //選択肢を入れ替える
            TextMeshProUGUI change = _choises[changeChoises];
            _choises[changeChoises] = _choises[choisesLength];
            _choises[choisesLength] = change;
        }
        #endregion

        #region 正解・不正解・終了のUIを非表示
        //正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("CorrectAnswerText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
        //不正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("IncorrectAnswerText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
        //終了のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("EndText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
        if (_hintCount > 0)
        {
            //ヒントを表示
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = _hintButton;

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
        #endregion

        QuizSet();
    }

    //クイズの出題処理
    public void QuizSet()
    {        
        //タイマーのカウントを開始
        _timer.TimerStart();
        _answerButton.ResetBool();
        //初期化
        _textCount = 0;
        _dataCount = 0;
        _trapWordCount = 0;
        //出題する単語を選択
        int correctAnswerWord = Random.Range(1, _maxDate);
        //もしも選ばれた単語がもう一度選ばれたらやり直す
        for(int i = 0; i< _quizCount-1; i++)
        {
            if(_answerNumbers[i]==correctAnswerWord)
            {
                QuizSet();
            }
        }
        //ヒントによる処理を元に戻す
        for (int i = 0; i < _choises.Length; i++)
        {
            _choises[i].transform.parent.gameObject.GetComponent<Button>().interactable = true;
        }
        _answerNumbers[_quizCount - 1] = correctAnswerWord;
        //ダミーの単語を選択
        int trapchoise = Random.Range(1, _maxDate);
        #region 再抽選
        //答えとダミーが一緒だったら答えの番号より後の数字で再抽選
        if (correctAnswerWord == trapchoise)
        {
            trapchoise = Random.Range(correctAnswerWord + 1, _maxDate);
        }
        #endregion

        #region 出題処理
        switch (mode)
        {
            #region 解答群の準備(アイヌ語)
            case Mode.Ainu:
                //クイズで使う単語
                foreach (DataRow row in _dataTable.Rows)
                {
                    //アイヌ語を格納
                    string ainu = $"{row["Ainu"]}";
                    //日本語を格納
                    string japanese = $"{row["Japanese"]}";
                    //品詞を格納
                    string pos = $"{row["PoS"]}";
                    //もしも途中で単語の番号が最大値になったらやり直し
                    if (trapchoise == _maxDate || correctAnswerWord == _maxDate)
                    {
                        QuizSet();
                    }

                    //カウントアップ
                    _dataCount++;
                    //問題と正解を表示する処理
                    if (_dataCount == correctAnswerWord)
                    {
                        //答えの配列番号を格納
                        _incorrectAnswerNumber = _textCount;
                        //出題単語を表示
                        _quizText.text = $"{ainu}";
                        //解答に表示する答えを設定
                        _correctAnswerText.text = $"{japanese}";
                        _incorrectAnswerText.text = $"{japanese}";
                        //正解の選択肢に単語を入れる
                        _choises[_textCount].text = $"{japanese}";
                        //選択肢に対して正解のフラグを設定する
                        _choiseScript = _choises[_textCount].GetComponent<ChoiceScript>();
                        _choiseScript.TrueSet();
                        //選択肢の数をカウントアップ
                        _textCount++;
                    }
                    //ダミー選択肢の設定
                    else if (_dataCount == trapchoise && _trapWordCount < 3)
                    {
                        //ダミーの解答群
                        _choises[_textCount].text = $"{japanese}";
                        //ダミーの単語を再抽選
                        trapchoise = Random.Range(trapchoise + 1, _maxDate);
                        //選択肢に対して不正解のフラグを設定する
                        _choiseScript = _choises[_textCount].GetComponent<ChoiceScript>();
                        _choiseScript.FalseSet();
                        //選択肢のカウントとダミー選択肢のカウントを+１
                        _textCount++;
                        _trapWordCount++;
                    }
                }
                break;
            #endregion

            #region 解答群の準備(日本語)
            case Mode.Japanese:
                //クイズで使う単語
                foreach (DataRow row in _dataTable.Rows)
                {
                    //アイヌ語を格納
                    string japanese = $"{row["Japanese"]}";
                    //日本語を格納
                    string ainu = $"{row["Ainu"]}";
                    //品詞を格納
                    string pos = $"{row["PoS"]}";
                    //もしも途中で単語の番号が最大値になったらやり直し
                    if (trapchoise == _maxDate || correctAnswerWord == _maxDate)
                    {
                        QuizSet();
                    }

                    //カウントアップ
                    _dataCount++;
                    //問題と正解を表示する処理
                    if (_dataCount == correctAnswerWord)
                    {
                        //答えの配列番号を格納
                        _incorrectAnswerNumber = _textCount;
                        //出題単語を表示
                        _quizText.text = $"{japanese}";
                        //解答に表示する答えを設定
                        _correctAnswerText.text = $"{ainu}";
                        _incorrectAnswerText.text = $"{ainu}";
                        //正解の選択肢に単語を入れる
                        _choises[_textCount].text = $"{ainu}";
                        //選択肢に対して正解のフラグを設定する
                        _choiseScript = _choises[_textCount].GetComponent<ChoiceScript>();
                        _choiseScript.TrueSet();
                        //選択肢の数をカウントアップ
                        _textCount++;
                    }
                    //ダミー選択肢の設定
                    else if (_dataCount == trapchoise && _trapWordCount < 3)
                    {
                        //ダミーの解答群
                        _choises[_textCount].text = $"{ainu}";
                        //ダミーの単語を再抽選
                        trapchoise = Random.Range(trapchoise + 1, _maxDate);
                        //選択肢に対して不正解のフラグを設定する
                        _choiseScript = _choises[_textCount].GetComponent<ChoiceScript>();
                        _choiseScript.FalseSet();
                        //選択肢のカウントとダミー選択肢のカウントを+１
                        _textCount++;
                        _trapWordCount++;
                    }
                }
                break;
                #endregion
        }
        #endregion
    }

    #region 出題モードを切り替える
    public void JapaneseMode()
    {
        //最初にクイズのタイトルを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizTitle");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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

        //最初にクイズのタイトルを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchConfirmation").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
        mode = Mode.Japanese;  
        _hintButton.gameObject.GetComponent<Image>().enabled = true;
        QuizReset();
    }
    public void AinuMode()
    {
        //最初にクイズのタイトルを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizTitle");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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

        //最初にクイズのタイトルを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchConfirmation").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
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

        mode = Mode.Ainu;
        _hintButton.gameObject.GetComponent<Image>().enabled = true;
        QuizReset();
    }
    #endregion

    public void Reset()
    {
        mode = Mode.NoStart;
    }

    public int QuizMode()
    {
        return (int) mode;
    }

    #region ヒントの処理
    public void Hint()
    {
        //ヒントの残り回数を減らす
        _hintCount--;
        //ヒントの残り回数の表示を更新
        _hintCountText.GetComponent<TextMeshProUGUI>().text = $"{_hintCount}" + "/" + $"{_hintNumberofuses}";
        //答えの番号格納
        _hintButtonNumber = _incorrectAnswerNumber;
        //ヒントのボタンを非表示
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = _hintButton;

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
        //非表示にする選択肢を選ぶ(2個)
        for (int i = 0; i< 2; i++)
        {
            _hintButtonNumber++;
            if (_hintButtonNumber > 3)
            {
                _hintButtonNumber = 1;
            }
            
            _choises[_hintButtonNumber].transform.parent.gameObject.GetComponent<Button>().interactable = false;
        }  
    }
    #endregion

    public int HintCount()
    {
        _hintCount++;
        return (int)_hintCount;
    }
}