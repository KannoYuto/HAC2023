using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    }

    private void ChildsDisplay(GameObject parent, Transform[] childs)
    {
        foreach (Transform child in childs.Skip(1))
        {
            if (child.gameObject.GetComponent<Image>())
            {
                child.gameObject.GetComponent<Image>().enabled = true;
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
    }

    public void QuizMode()
    {
        _mode = Mode.Quiz;

        ChildsNonDisplay(_uIByFunctions[0], _uIByFunctions[0].GetComponentsInChildren<Transform>());
        ChildsDisplay(_uIByFunctions[1], _uIByFunctions[1].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[2], _uIByFunctions[2].GetComponentsInChildren<Transform>());

        this.GetComponent<QuizController>().QuizInitialization();
        this.GetComponent<QuizController>().AnswerButton().enabled = false;
    }

    public void OthersMode()
    {
        _mode = Mode.Others;

        ChildsNonDisplay(_uIByFunctions[0], _uIByFunctions[0].GetComponentsInChildren<Transform>());
        ChildsNonDisplay(_uIByFunctions[1], _uIByFunctions[1].GetComponentsInChildren<Transform>());
        ChildsDisplay(_uIByFunctions[2], _uIByFunctions[2].GetComponentsInChildren<Transform>());
    }
}
