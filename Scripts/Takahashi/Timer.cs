using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Slider _slider = default;
    [SerializeField,Header("制限時間の最大値")]
    private float _maxTime = default;
    private float _nowTime = default;
    [SerializeField, Header("クイズのスクリプト")]
    private Answer _answer = default;
    private bool isCount = false;
    [SerializeField, Header("タイマーのfillを入れる")]
    private Image _fill = default;
    private AppController _appController = default;

    private void Awake()
    {
        //回答ボタンのスクリプトを取得
        _answer = GameObject.FindWithTag("AnswerUI").GetComponent<Answer>();
        //アプリコントローラーからスクリプトを取得
        _appController = GameObject.FindWithTag("AppController").GetComponent<AppController>();
    }
    public void TimerStart()
    {
        //タイマーを稼働させる
        isCount = true;
        //スライダーを取得
        _slider = this.GetComponent<Slider>();
        //スライダーの数値を初期化
        _slider.maxValue = _maxTime;
        //スライダーのゲージを初期化
        _nowTime = _maxTime;
        _slider.value = _maxTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(_appController.CurrentMode() != 1)
        {
            return;
        }
        //制限時間が有効な状態かどうか
        if (isCount && _appController.CurrentMode() == 1)
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
            //制限時間が1/4になったらゲージの色を赤くする
            if (_slider.value <= _maxTime / 4)
            {
                _fill.color = new Color32(255, 0, 0, 255);
            }
            //制限時間が1/4になったらゲージの色を黄色くする
            else if (_slider.value <= _maxTime / 2)
            {
                _fill.color = new Color32(255, 220, 60, 255);
            }
            //元の色にする
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
