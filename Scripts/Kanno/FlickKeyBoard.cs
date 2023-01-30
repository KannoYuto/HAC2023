using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FlickKeyBoard : MonoBehaviour
{
    [SerializeField]
    private Transform _parent = default;

    private EventSystem _eventSystem = default;

    private PointerEventData _pointData = default;

    private List<RaycastResult> _rayResult = new List<RaycastResult>();

    [SerializeField]
    private Transform[] _keys = new Transform[5];

    [SerializeField]
    private Transform _selectKey = default;

    [SerializeField]
    private TMP_InputField _iF = default;

    private void Awake()
    {
        _parent = this.transform;

        _eventSystem = EventSystem.current;

        _pointData = new PointerEventData(_eventSystem);

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _keys = new Transform[5];

            _pointData.position = Input.mousePosition;

            _eventSystem.RaycastAll(_pointData, _rayResult);

            if (_rayResult.Count == 0)
            {
                return;
            }

            foreach (RaycastResult key in _rayResult)
            {
                if (key.gameObject.tag == "Key")
                {
                    List<Transform> keys = new List<Transform>();

                    foreach (Transform keyTransform in key.gameObject.GetComponentsInChildren<Transform>())
                    {
                        if (keyTransform.tag == "Key")
                        {
                            keys.Add(keyTransform);
                        }
                    }

                    _keys = keys.ToArray();

                    break;
                }
            }

            if (_keys[0] != null)
            {
                _keys[0].SetAsLastSibling();

                for (int i = 1; i < _keys.Length; i++)
                {
                    _keys[i].GetComponent<Image>().enabled = true;
                    _keys[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _pointData.position = Input.mousePosition;

            _eventSystem.RaycastAll(_pointData, _rayResult);

            foreach (RaycastResult key in _rayResult)
            {
                if (key.gameObject.tag == "Key")
                {
                    _selectKey = key.gameObject.transform;

                    break;
                }
            }

            for (int j = 0; j < _keys.Length; j++)
            {
                if (_keys[j] != null && _keys[j] == _selectKey)
                {
                    _iF.text = String.Concat(_iF.text, _keys[j].GetComponentInChildren<TextMeshProUGUI>().text);

                    break;
                }
            }

            if (_keys[0] != null)
            {
                for (int k = 1; k < _keys.Length; k++)
                {
                    _keys[k].GetComponent<Image>().enabled = false;
                    _keys[k].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }
}
