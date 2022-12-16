using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Slider _slider = default;
    [SerializeField,Header("制限時間の最大値")]
    private float _maxTime = 30;
    private float _nowTime = default;
    [SerializeField, Header("クイズのスクリプト")]
    private Answer _answer = default;
    private bool isCount = false;
    [SerializeField, Header("タイマーのfillを入れる")]
    private Image _fill = default;
    [SerializeField]
    private AppController _appController = default;
    private int _mode = default;

    private void Awake()
    {
        _answer = GameObject.FindWithTag("AnswerUI").GetComponent<Answer>();
        _appController = GameObject.FindWithTag("AppController").GetComponent<AppController>();
    }
    public void TimerStart()
    {
        isCount = true;
        _slider = this.GetComponent<Slider>();
        _slider.maxValue = _maxTime;
        _nowTime = _maxTime;
        _slider.value = _maxTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        _mode = _appController.CurrentMode();
        //制限時間が有効な状態かどうか
        if (isCount && _mode == 1)
        {
            //制限時間を減らしていく
            _nowTime -= Time.deltaTime;
            _slider.value = _nowTime;
            //カウントが0になったら現在選択されている選択肢で正解を判定
            //(選択されていない場合は不正解)
            if (_slider.value == 0)
            {
                _answer.OnClick();
                //制限時間を止める
                TimerStop();
            }
            if (_slider.value <= _maxTime / 4)
            {
                _fill.color = new Color32(255, 0, 0, 255);
            }
            else if (_slider.value <= _maxTime / 2)
            {
                _fill.color = new Color32(255, 220, 60, 255);
            }
            else
            {
                _fill.color = new Color32(25, 191, 108, 255);
            }
        }
    }

    public void TimerStop()
    {
        //制限時間を止める
        isCount = false;
    }
}
