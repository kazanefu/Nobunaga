using LitMotion;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class TitleController : MonoBehaviour
{
    [SerializeField] private string _sceneName = "MainScene";
    private InputAction _inputAction;
    private InputSystem_Actions _actions;


    [SerializeField] private PanelController _howToPanel; 
    private GameManager _gameManager;
    private static readonly List<RaycastResult> _hits = new List<RaycastResult>();

    // ポインタの下に Button などがあるか
    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null || Pointer.current == null) return false;

        var data = new PointerEventData(EventSystem.current)
        {
            position = Pointer.current.position.ReadValue()
        };

        _hits.Clear();
        EventSystem.current.RaycastAll(data, _hits);

        foreach (RaycastResult hit in _hits)
        {
            if (hit.gameObject.GetComponentInParent<Selectable>() != null) return true;
        }
        return false;
    }
    private void Awake()    
    {
        _actions = new InputSystem_Actions();
        _inputAction = _actions.UI.Click;
        LMotion.Create(0f, 1f, 0.4f)
            .WithOnComplete(() =>
            {
                GameObject targetObject = GameObject.Find("GameManager");
                _gameManager = targetObject.GetComponent<GameManager>();
            })
            .WithEase(Ease.OutBack)
            .RunWithoutBinding()
            .AddTo(gameObject);
    }

    private void OnEnable()
    {
        _inputAction.Enable();
    }

    private void OnDisable()
    {
        _inputAction.Disable();
    }

    private void Update()
    {
        if (_sceneName == "TitleScene") return;
        if (_howToPanel != null && _howToPanel.IsOpen) return;

        if (_inputAction.WasPressedThisFrame())
        {
            if (IsPointerOverButton()) return;
            ClickStart();
        }
    }
    public void ClickStart()
    {
        LMotion.Create(0f,0f,0.4f)
            .WithOnComplete(() =>
            {
                if (_sceneName == "GameScene")
                {
                    _gameManager.LoadGameScene();
                }
                else
                {
                    _gameManager.LoadTitleScene();
                }
            })
            .RunWithoutBinding()
            .AddTo(gameObject);
    }

    public void ClickExit()
    {
        Application.Quit();
        EditorApplication.isPlaying = false;
    }

}
