using LitMotion;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
public class TitleController : MonoBehaviour
{
    [SerializeField] private string _sceneName = "MainScene";
    private InputAction _inputAction;
    private InputSystem_Actions _actions;

    private GameManager _gameManager;
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
        if(_inputAction.WasPressedThisFrame())
        {
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
    }

}
