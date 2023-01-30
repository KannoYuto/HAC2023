using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoldingSystem : MonoBehaviour
{
    [SerializeField]
    private Button[] _buttons = new Button[10];

    //折り畳み部分を取得
    [SerializeField]
    private Transform[] FoldinfImages = new Transform[10];
    [SerializeField,Header("Contentを入れる")]
    private GameObject  _content = default;
    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];

    VerticalLayoutGroup contentLayout;

    // Start is called before the first frame update
    private void Awake()
    {
        //_buttons = GameObject.FindGameObjectsWithTag("Image").Select(GameObject => GameObject.transform.GetChild(0).GetComponent<Button>()).ToArray();

        //FoldinfImages = GameObject.FindGameObjectsWithTag("Space").Select(GameObject => GameObject.transform).ToArray();

        for(int s = 0; s < FoldinfImages.Length; s++)
        {
            FoldinfImages[s].gameObject.SetActive(false);
        }

        contentLayout = GameObject.Find("Content").GetComponent<VerticalLayoutGroup>();

        for (int i = 0; i < _buttons.Length; i++)
        {
            int b = i;
            _buttons[i].onClick.AddListener(() => OnClickSetInfo(b));
        }
    }

    public void OnClickSetInfo(int num)
    {
        if (FoldinfImages[num].gameObject.activeSelf)
        {
            FoldinfImages[num].gameObject.SetActive(false);           
        }
        else
        {
            FoldinfImages[num].gameObject.SetActive(true);

            for (int i = 0; i < _Uis.Length; i++)
            {
                _Uis[i] = this.gameObject;

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
    }

    IEnumerator LayoutRenewal()
    {
        //1フレーム停止
        yield return null;

        //再開後の処理
        contentLayout.enabled = false;
        contentLayout.enabled = true;
    }

    public void Reset()
    {
        for (int i = 0; i < FoldinfImages.Length; i++)
        {
            FoldinfImages[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].GetComponent<Spritechange>().Reset();
        }
        _content.transform.position = new Vector2(_content.transform.position.x,0);
    }
}
