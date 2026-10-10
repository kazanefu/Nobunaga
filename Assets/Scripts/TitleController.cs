using LitMotion;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class TitleController : MonoBehaviour
{
    [SerializeField] private string _mainSceneName = "MainScene";
    private InputAction _inputAction;
    private InputSystem_Actions _actions;

    private float _clickTime = default;
    private void Awake()    
    {
        _actions = new InputSystem_Actions();
        _inputAction = _actions.UI.Click;
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
        if(_inputAction.WasPressedThisFrame())
        {
            print("Click");
            ClickStart();
        }
    }
    public void ClickStart()
    {
        LMotion.Create(0f,0f,0.4f)
            .WithOnComplete(() =>
            {
                SceneManager.LoadScene(_mainSceneName);
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
