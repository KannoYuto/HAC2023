using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

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

    //タイトル画面を開いている時に制限時間を止める用(髙橋)
    [SerializeField,Header("timerを入れる")]
    private Timer _timer = default;
    private void Awake()
    {
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
            else
            {
                continue;
            }
        }
    }

    public void SearchMode()
    {
        //クイズ画面を開いている時に別モードのボタンを押したら確認する(髙橋)
        if (_mode == Mode.Quiz)
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
        _timer.TimerStop();
    }

    public void QuizMode()
    {
        if(_mode == Mode.Quiz)
        {
            return;
        }

        _mode = Mode.Quiz;

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
            }
        }
        #endregion
    }

    public void OthersMode()
    {
        //クイズ画面を開いている時に別モードのボタンを押したら確認する
        if (_mode == Mode.Quiz)
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
