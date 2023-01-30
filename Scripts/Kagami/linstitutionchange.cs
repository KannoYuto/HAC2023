using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class linstitutionchange : MonoBehaviour
{
    //ドロップダウンを格納する変数
    [SerializeField] private Dropdown dropdown;
    //Scroll Viewを格納する変数
    [SerializeField] private GameObject ScrollView;
    //Scroll View2を格納する変数
    [SerializeField] private GameObject ScrollView2;

    //非表示にするUIを格納する配列
    private GameObject[] _Uis = new GameObject[1];
    [SerializeField, Header("Scroll ViewのContentを入れる")]
    private GameObject _content = default;
    [SerializeField, Header("Scroll View2のContentを入れる")]
    private GameObject _content2 = default;

    // ドロップダウンが変更されたときに実行
    public void ChangeDropdown()
    {
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
            }
        }

        if (dropdown .value == 0)
        {
            ScrollView.SetActive(true);
            ScrollView2.SetActive(false);
            _content2.transform.position = new Vector2(_content.transform.position.x, 0);
        }

         else if (dropdown.value == 1)
        {
            ScrollView.SetActive(false);
            _content.transform.position = new Vector2(_content.transform.position.x, 0);
            ScrollView2.SetActive(true);
        }
    }
}
