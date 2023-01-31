using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class AppController : MonoBehaviour
{
    private enum Mode
    {
        Search,
        Quiz,
        Others
    }
    [SerializeField, Header("アプリのモード")]
    private Mode _mode = default;

    [SerializeField, Header("機能別のUI(自動取得)")]
    private GameObject[] _uIByFunctions = new GameObject[3];

    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];

    [SerializeField, Header("timerを入れる")]
    private Timer _timer = default;
    [SerializeField, Header("施設一覧のScroll ViewのContentを入れる")]
    private GameObject _institutionContent = default;
    [SerializeField, Header("施設一覧のScroll View2のContentを入れる")]
    private GameObject _institutionContent2 = default;
    [SerializeField, Header("コラムのScroll ViewのContentを入れる")]
    private GameObject _columnContent = default;
    [SerializeField, Header("コラム一覧のFoldingSystemを入れる")]
    private FoldingSystem _foldingSystem = default;
    [SerializeField, Header("QuestionTextを入れる")]
    private Quiz _quiz = default;
    [SerializeField, Header("コラム一覧のColumnTextを入れる")]
    private GameObject _columnText = default;
    [SerializeField, Header("検索のColumnTextを入れる")]
    private GameObject _searchColumnText = default;
    [SerializeField, Header("検索のScroll Viewを入れる")]
    private GameObject _searchUI = default;

    [SerializeField, Header("検索機能")]
    private SearchFunction _sF = default;
    private Vector3 _clickStartPos = default;
    [SerializeField, Header("フリック時の閾値(30くらい)")]
    private float _threshold = default;

    private void Awake()
    {
        _quiz = GameObject.FindWithTag("QuestionText").GetComponent<Quiz>();
        for (int i = 0; i < _uIByFunctions.Length; i++)
        {
            switch (i)
            {
                case 0:

                    _uIByFunctions[i] = GameObject.FindGameObjectWithTag("SearchUI");

                    break;

                case 1:

                    _uIByFunctions[i] = GameObject.FindGameObjectWithTag("QuizUI");

                    ChildsNonDisplay(_uIByFunctions[i], _uIByFunctions[i].GetComponentsInChildren<Transform>());

                    break;

                case 2:

                    _uIByFunctions[i] = GameObject.FindGameObjectWithTag("OthersUI");

                    ChildsNonDisplay(_uIByFunctions[i], _uIByFunctions[i].GetComponentsInChildren<Transform>());

                    break;
            }
        }
        //最初にクイズのタイトルを非表示(髙橋)
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
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchOutput");

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _clickStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 direction = Input.mousePosition - _clickStartPos;

            float abs_X = Mathf.Abs(direction.x);
            float abs_Y = Mathf.Abs(direction.y);

            if (abs_Y > 100f)
            {
                return;
            }

            //その他の施設一覧を開いている時の処理
            if (abs_X >= _threshold && _institutionContent.GetComponent<Image>().enabled)
            {
                if (direction.x > 0)
                {
                    BackButton();
                }                
            }
            //その他のコラム一覧を開いている時の処理
            else if (abs_X >= _threshold && _columnContent.gameObject.transform.parent.GetComponent<Image>().enabled)
            {
                if (direction.x > 0)
                {
                    BackButton();
                }               
            }
            //その他のコラムを開いている時の処理
            else if (abs_X >= _threshold && _columnText.GetComponent<TextMeshProUGUI>().enabled)
            {
                if (direction.x > 0)
                {
                    ColumnBackProcess();
                }               
            }                           
            //検索結果画面の一番最初のページを開いている時の処理
            else if (abs_X >= _threshold && this.GetComponent<SearchFunction>().NowPage() == 0 && _searchUI.GetComponent<Image>().enabled 
                && _searchColumnText.GetComponent<TextMeshProUGUI>().enabled == false)
            {
                if (direction.x > 0)
                {
                    this.gameObject.GetComponent<SearchFunction>().SearchBack();
                }
                else
                {
                    this.gameObject.GetComponent<SearchFunction>().RightArrow();
                }
            }
            //検索結果画面の一番最初のページを開いている時以外の処理
            else if (abs_X >= _threshold && _searchUI.GetComponent<Image>().enabled 
                && _searchColumnText.GetComponent<TextMeshProUGUI>().enabled == false)
            {
                if (direction.x > 0)
                {
                    this.gameObject.GetComponent<SearchFunction>().LeftArrow();
                }
                else
                {
                    this.gameObject.GetComponent<SearchFunction>().RightArrow();
                }
            }
            //検索でColumnを開いている時の処理
            else if (abs_X >= _threshold && _searchColumnText.GetComponent<TextMeshProUGUI>().enabled)
            {
                if (direction.x > 0)
                {
                    this.gameObject.GetComponent<SearchFunction>().ColumnClose();
                }                
            }
        }
    }

    private void ChildsDisplay(GameObject parent, Transform[] childs)
    {
        foreach (Transform child in childs.Skip(1))
        {
            if (child.gameObject.GetComponent<Image>())
            {
                child.gameObject.GetComponent<Image>().enabled = true;
            }
            else if (child.gameObject.GetComponent<TextMeshProUGUI>())
            {
                child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            }
            else if (child.gameObject.GetComponent<Text>())
            {
                child.gameObject.GetComponent<Text>().enabled = true;
            }
            else
            {
                continue;
            }
        }
    }

    private void ChildsNonDisplay(GameObject parent, Transform[] childs)
    {
        foreach (Transform child in childs.Skip(1))
        {
            if (child.gameObject.GetComponent<Image>())
            {
                child.gameObject.GetComponent<Image>().enabled = false;
            }
            else if (child.gameObject.GetComponent<TextMeshProUGUI>())
            {
                child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else if (child.gameObject.GetComponent<Text>())
            {
                child.gameObject.GetComponent<Text>().enabled = false;
            }
            else
            {
                continue;
            }
        }
    }

    public void SearchMode()
    {
        _sF.KeyBoardNonDisplay();
        //クイズ画面を開いている時に別モードのボタンを押したら確認する(髙橋)
        if (_mode == Mode.Quiz && (_quiz.QuizMode() == 1 || _quiz.QuizMode() == 0))
        {
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("SearchConfirmation");

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
        }
        else
        {
            SearchModeProcess();
        }
    }

    public void SearchModeProcess()
    {
        this.GetComponent<QuizController>().ResetButton();
        this.GetComponent<SearchFunction>().InputOn();
        _quiz.Reset();
        //クイズのタイトルを非表示(髙橋)
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
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
            }
        }

        this.gameObject.GetComponent<QuizController>().ReStart();
        _mode = Mode.Search;

        ChildsDisplay(_uIByFunctions[0], _uIByFunctions[0].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[1], _uIByFunctions[1].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[2], _uIByFunctions[2].GetComponentsInChildren<Transform>());

        switch (this.GetComponent<SearchFunction>().GetCurrentMode())
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
        this.GetComponent<SearchFunction>().SearchBack();
        _timer.TimerStop();
    }

    public void QuizMode()
    {
        if (_mode == Mode.Quiz)
        {
            return;
        }

        _mode = Mode.Quiz;

        _sF.KeyBoardNonDisplay();

        this.GetComponent<SearchFunction>().InputOff();

        //クイズのタイトルを表示(髙橋)
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("QuizTitle");

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
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = true;
                }
            }
        }

        ChildsNonDisplay(_uIByFunctions[0], _uIByFunctions[0].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[2], _uIByFunctions[2].GetComponentsInChildren<Transform>());

        this.GetComponent<QuizController>().QuizInitialization();

        #region クイズ開始時に必要の無いUIを非表示(確認画面2種,result画面,正解不正解のUI)髙橋

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
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
            }
        }
        #endregion
    }

    public void OthersMode()
    {
        //クイズ画面を開いている時に別モードのボタンを押したら確認する
        if (_mode == Mode.Quiz && (_quiz.QuizMode() == 1 || _quiz.QuizMode() == 0))
        {
            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = GameObject.FindGameObjectWithTag("OtherConfirmation");

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
                    else if (child.gameObject.GetComponent<Text>())
                    {
                        child.gameObject.GetComponent<Text>().enabled = true;
                    }
                }
            }
        }
        else
        {
            OthersModeProcess();
        }
    }

    public void OthersModeProcess()
    {
        this.GetComponent<QuizController>().ResetButton();
        
        _quiz.Reset();

        _sF.KeyBoardNonDisplay();

        this.GetComponent<SearchFunction>().InputOff();

        //クイズのタイトルを非表示
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

        //画面を切り替えたらクイズ機能を初期化
        this.gameObject.GetComponent<QuizController>().ReStart();
        _mode = Mode.Others;

        ChildsNonDisplay(_uIByFunctions[0], _uIByFunctions[0].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[1], _uIByFunctions[1].GetComponentsInChildren<Transform>());
        ChildsDisplay(_uIByFunctions[2], _uIByFunctions[2].GetComponentsInChildren<Transform>());
        _timer.TimerStop();

        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("IinstitutionUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }
    //施設紹介を表示する処理
    public void IinstitutionProcess()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("OthersUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("IinstitutionUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }

        }
    }
    public void ColumnProcess()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("OthersUI");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }

        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }

        }
    }
    public void ColumnBackProcess()
    {
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText").transform.parent.gameObject;

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = true;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = true;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }

        }
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("ColumnText");

            foreach (Transform child in _Uis[i].GetComponentsInChildren<Transform>().Skip(1))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().enabled = false;
                }
                else if (child.gameObject.GetComponent<Text>())
                {
                    child.gameObject.GetComponent<Text>().enabled = false;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }

        }
    }

    public void BackButton()
    {
        _institutionContent.transform.position = new Vector2(_institutionContent.transform.position.x, 0);
        _institutionContent2.transform.position = new Vector2(_institutionContent.transform.position.x, 0);
        _columnContent.transform.position = new Vector2(_institutionContent.transform.position.x, 0);
        _columnContent.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        _columnContent.GetComponent<RectTransform>().offsetMax = new Vector2(-933, 0);

        _foldingSystem.Reset();
        OthersModeProcess();
    }
    public int CurrentMode()
    {
        return (int)_mode;
    }

    public void Test()
    {
        Debug.Log("Clicked!!");
    }

    public void Cancel()
    {
        #region 確認画面で『いいえ』を選択した時に確認画面を非表示にする(髙橋)
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("SearchConfirmation");

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
        for (int i = 0; i < _Uis.Length; i++)
        {
            _Uis[i] = GameObject.FindGameObjectWithTag("OtherConfirmation");

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
        #endregion
    }
}
