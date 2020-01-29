using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseState { Idle, Select }

public class MouseCursor : MonoBehaviour
{
    public static MouseCursor instance;

    [SerializeField] private GameObject idleCursor;
    [SerializeField] private GameObject selectCursor;

    private MouseState state;
    public MouseState State {
        set
        {
            state = value;
            StateMachine();
        }
    }
    private RectTransform mouseRect;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        mouseRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        State = MouseState.Idle;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        mouseRect.anchoredPosition = mousePos;
    }

    private void StateMachine()
    {
        switch(state)
        {
            case MouseState.Idle:
                idleCursor.SetActive(true);
                selectCursor.SetActive(false);
                break;
            case MouseState.Select:
                idleCursor.SetActive(false);
                selectCursor.SetActive(true);
                break;
        }
    }
}
