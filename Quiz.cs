using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Quiz : MonoBehaviour

//出題するスクリプト

{
    //使用するデータベース
    private const string CONST_DATA = "ainu_DB_ALL.db";
    [SerializeField, Header("問題用のテキスト")]
    private Text _quizText = default;
    [SerializeField, Header("正解のテキスト")]
    private Text _correctAnswerText = default;
    [SerializeField, Header("不正解のテキスト")]
    private Text _incorrectAnswerText = default;
    [SerializeField, Header("クイズ用のデータの行をカウントする変数")]
    private int _dataCount = 0;
    [SerializeField, Header("クイズで使用するデータの最大値")]
    private int _maxDate = default;
    [SerializeField, Header("解答群を格納する配列")]
    private Text[] _choices =new Text[4];
    [SerializeField, Header("第〇問を表示するテキスト")]
    private Text _countText = default;
    //回答群の数
    private int _textCount = 0;
    //ダミーの数
    private int _trapWordCount = 0;
    //何問目かカウント
    private int _quizCount = 0;
    //選択肢のスクリプト
    private ChoiceScript _choiceScript = default;
    //データベースを格納
    private DataTable _dataTable = default;
    private GameObject[] _Uis = new GameObject[3];

    private void Awake()
    {
        //データベースを引っ張り出す
        SqliteDatabase currentDB = new SqliteDatabase(CONST_DATA);
        //SQL文でクイズに使う単語を抽出
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

        //必要なやつを全取得
        _quizText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();
        _correctAnswerText = GameObject.FindWithTag("CorrectAnswerText").GetComponent<Text>();
        _incorrectAnswerText = GameObject.FindWithTag("IncorrectAnswerText").GetComponent<Text>();
        _countText = GameObject.FindWithTag("CountText").GetComponent<Text>();
        GameObject[] choice = GameObject.FindGameObjectsWithTag("ChoiceText");

        for (int i = 0; i < choice.Length; i++)
        {
            _choices[i] = choice[i].GetComponent<Text>();
        }

        //正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i]= GameObject.FindGameObjectWithTag("CorrectAnswer");

            foreach(Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if(child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
            }
        }
        //不正解のUIを見えなくする処理
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i]= GameObject.FindGameObjectWithTag("IncorrectAnswer");

            foreach(Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if(child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
            }
        }
        
        //最初にクイズ出題処理を呼び出す
        QuizAct();
    }
    private void Start()
    {
    }
    //クイズの出題処理
    public void QuizAct()
    {
        //問題数をカウントアップ
        _quizCount++;
        //問題数を表示
        _countText.text = "第" + $"{_quizCount}" + "問";
        //初期化
        _textCount = 0;
        _dataCount = 0;
        _trapWordCount = 0;
        //出題する単語を選択
        int randomWords = Random.Range(1, _maxDate);
        //ダミーの単語を選択
        int trapChoice = Random.Range(1, _maxDate);

        #region 選択肢をシャッフルする
        //選択肢の配列の長さを数値として格納
        int choicesLength = _choices.Length;
        while (choicesLength > 1)
        {
            //カウントを減らす
            choicesLength--;
            //入れ替える選択肢を選ぶ
            int changeChoices = Random.Range(0, choicesLength);
            //選択肢を入れ替える
            Text change = _choices[changeChoices];
            _choices[changeChoices] = _choices[choicesLength];
            _choices[choicesLength] = change;
        }
        #endregion

        #region 解答群の準備
        //クイズで使う単語
        foreach (DataRow row in _dataTable.Rows)
        {
            //アイヌ語を格納
            string ainu = $"{row["Ainu"]}";
            //日本語を格納
            string japanese = $"{row["Japanese"]}";
            //品詞を格納
            string pos = $"{row["PoS"]}";

            //答えとダミーが一緒だったら再抽選
            if (randomWords == trapChoice)
            {
                trapChoice = Random.Range(1, _maxDate);
            }
            //カウントアップ
            _dataCount++;
            //問題と正解を表示する処理
            if (_dataCount == randomWords)
            {
                //出題単語を表示
                _quizText.text = $"{ainu}";
                //解答に表示する答えを設定
                _correctAnswerText.text = $"{japanese}";
                _incorrectAnswerText.text = $"{japanese}";
                //正解の選択肢に単語を入れる
                _choices[_textCount].text = $"{japanese}";
                //選択肢に対して正解のフラグを設定する
                _choiceScript = _choices[_textCount].GetComponent<ChoiceScript>();
                _choiceScript.TrueSet();
                //選択肢の数をカウントアップ
                _textCount++;
            }
            //ダミー選択肢の設定
            else if (_dataCount == trapChoice&& _trapWordCount < 3)
            {
                //ダミーの解答群
                _choices[_textCount].text = $"{japanese}";
                //ダミーの単語を再抽選
                trapChoice = Random.Range(trapChoice + 1, _maxDate);
                //選択肢に対して不正解のフラグを設定する
                _choiceScript = _choices[_textCount].GetComponent<ChoiceScript>();
                _choiceScript.FalseSet();
                //選択肢のカウントとダミー選択肢のカウントを+１
                _textCount++;
                _trapWordCount++;
            }
        }
        #endregion
    }
}