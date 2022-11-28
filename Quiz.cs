using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour

//出題するスクリプト

{
    [SerializeField, Header("問題用のテキスト")]
    private Text _quizText = default;
    [SerializeField, Header("正解のテキスト")]
    private Text _seikaiText1 = default;
    [SerializeField, Header("不正解のテキスト")]
    private Text _seikaiText2 = default;
    [SerializeField, Header("クイズ用のデータの行をカウントする変数")]
    private int _dataCount = 0;
    [SerializeField, Header("引っ張り出すデータの個数")]
    private int _maxDate = default;
    [SerializeField, Header("解答群を格納する配列")]
    private Text[] choices =default;
    [SerializeField, Header("第〇問を表示するテキスト")]
    private Text _countText = default;
    //回答群の数
    private int _textCount = 0;
    //ダミーの数
    private int _trapWords = 0;
    //何問目かカウント
    private int _quizCount = 0;
    //選択肢のスクリプト
    private Choice_Script _choiceScript = default;
    //データベースを格納
    private DataTable _dataTable = default;
    private const string CONST_DATA= "ainu_DB_ALL.db";

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
        //最初にクイズ出題処理を呼び出す
        Quiz_act();
    }
    //クイズの出題処理
    public void Quiz_act()
    {
        //問題数をカウントアップ
        _quizCount++;
        //問題数を表示
        _countText.text = "第" + $"{_quizCount}" + "問";
        //初期化
        _textCount = 0;
        _dataCount = 0;
        _trapWords = 0;
        //出題する単語を選択
        int random_words = Random.Range(1, _maxDate);
        //ダミーの単語を選択
        int trap_choice = Random.Range(1, _maxDate);

        #region//選択肢をシャッフルする
        System.Random rng = new System.Random();
        //配列の長さを数値として格納
        int n = choices.Length;
        while (n > 1)
        {
            //カウントを減らす
            n--;
            //0～nのランダムな数値を選ぶ
            int k = rng.Next(n + 1);
            //そのランダムな番号の選択肢とn番目の選択肢を入れ替える
            Text tmp = choices[k];
            choices[k] = choices[n];
            choices[n] = tmp;
        }
        #endregion

        #region//解答群を用意
        foreach (DataRow row in _dataTable.Rows)
        {
            //アイヌ語を格納
            string ainu = $"{row["Ainu"]}";
            //日本語を格納
            string japanese = $"{row["Japanese"]}";
            //品詞を格納
            string pos = $"{row["PoS"]}";

            //答えとダミーが一緒だったら再抽選
            if (random_words == trap_choice)
            {
                trap_choice = Random.Range(1, _maxDate);
            }
            //カウントアップ
            _dataCount++;
            //問題と正解を表示する処理
            if (_dataCount == random_words)
            {
                //出題単語を表示
                _quizText.text = $"{ainu}";
                //解答に表示する答えを設定
                _seikaiText1.text = $"{japanese}";
                _seikaiText2.text = $"{japanese}";
                //正解の選択肢に単語を入れる
                choices[_textCount].text = $"{japanese}";
                //選択肢に対して正解のフラグを設定する
                _choiceScript = choices[_textCount].GetComponent<Choice_Script>();
                _choiceScript.TrueSet();
                //選択肢の数をカウントアップ
                _textCount++;
            }
            //ダミー選択肢の設定
            else if (_dataCount == trap_choice&& _trapWords < 3)
            {
                //ダミーの解答群
                choices[_textCount].text = $"{japanese}";
                //ダミーの単語を再抽選
                trap_choice = Random.Range(trap_choice + 1, _maxDate);
                //選択肢に対して不正解のフラグを設定する
                _choiceScript = choices[_textCount].GetComponent<Choice_Script>();
                _choiceScript.FalseSet();
                //選択肢のカウントとダミー選択肢のカウントを+１
                _textCount++;
                _trapWords++;
            }
        }
        #endregion
    }
}