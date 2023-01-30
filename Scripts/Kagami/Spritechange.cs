using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spritechange : MonoBehaviour
{
    private Image m_Image;
    public Sprite[] m_Sprite;

    bool Change;

    // Update is called once per frame
    private void Awake()
    {
        Change = false;
        m_Image = this.transform.parent.gameObject.GetComponent<Image>();
    }
    
    public void onClick()
    {
          // changeがtrueの場合
          if (Change)
          {
              //（配列 m_Sprite[0] に格納したスプライトオブジェクトを変数 m_Image に格納したImage コンポーネントに割り当て）
              m_Image.sprite = m_Sprite[0];
              Change = false;
        }
          // changeがfalseの場合
          else
          {
              //（配列 m_Sprite[1] に格納したスプライトオブジェクトを変数 m_Image に格納したImage コンポーネントに割り当て）
              m_Image.sprite = m_Sprite[1];
              Change = true;
        }
    }

    public void Reset()
    {
        //（配列 m_Sprite[0] に格納したスプライトオブジェクトを変数 m_Image に格納したImage コンポーネントに割り当て）
        m_Image.sprite = m_Sprite[0];
        Change = false;
    }
}
