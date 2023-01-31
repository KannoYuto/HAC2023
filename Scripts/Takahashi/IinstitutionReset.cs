using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IinstitutionReset : MonoBehaviour
{
    [SerializeField, Header("Scroll ViewのContentを入れる")]
    private GameObject _content = default;
    [SerializeField, Header("Scroll View2のContentを入れる")]
    private GameObject _content2 = default;

    public void PositionReset()
    {
        //スクロールの位置を元に戻す
        _content.transform.position = new Vector2(_content.transform.position.x, 0);
        _content2.transform.position = new Vector2(_content.transform.position.x, 0);
    }
}
