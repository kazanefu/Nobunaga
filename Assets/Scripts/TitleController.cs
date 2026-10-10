using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class TitleController : MonoBehaviour
{
    [SerializeField] private string _mainSceneName = "MainScene";
    private InputAction _inputAction;
    private InputSystem_Actions _actions;
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
        SceneManager.LoadScene(_mainSceneName);
    }

    public void ClickExit()
    {
        Application.Quit();
    }

}
