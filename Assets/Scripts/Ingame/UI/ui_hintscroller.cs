using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ui_hintscroller : MonoBehaviour {

    [Header("Hint settings")]
    public string text = "";
    public float speed = 0.2f;
    public bool isEnabled = true;

    private TextMeshProUGUI _text;
    private TextMeshProUGUI _cloneText;

    private RectTransform _rectTransform;
    private Vector2 _startPos;

    private float _sizeW;
    private float _scrollPos;

    public void Awake () {
        // Setup
        this._text = GetComponentInChildren<TextMeshProUGUI>();
        this._text.SetText(this.text);
        this._text.SetLayoutDirty();

        // Get rect
        this._rectTransform = this._text.GetComponent<RectTransform>();

        // Create clone
        GameObject parent = this._text.transform.parent.gameObject;
        this._cloneText = Instantiate<TextMeshProUGUI>(this._text);
        this._cloneText.SetText(this.text);

        RectTransform cloneRect = this._cloneText.GetComponent<RectTransform>();
        cloneRect.SetParent(this._rectTransform);
        cloneRect.anchorMin = new Vector2(1, 0.5f);
        cloneRect.localScale = new Vector3(1f, 1f, 1f);

        Vector3 pos = cloneRect.transform.localPosition;
        cloneRect.transform.localPosition = new Vector3(pos.x, 0, pos.z);
        cloneRect.anchoredPosition = new Vector2(10f, 0f);

        // vars
        this._sizeW = this._text.preferredWidth + cloneRect.transform.localPosition.x;
        this._startPos = this._rectTransform.anchoredPosition;
        this._scrollPos = 0;
    }

    // Update is called once per frame
    public void Update () {
        if (!this.isEnabled) return;
        this._rectTransform.anchoredPosition = new Vector2(this._startPos.x - this._scrollPos, this._startPos.y);
        this._scrollPos = (this._scrollPos % this._sizeW) + this.speed;
    }
}
