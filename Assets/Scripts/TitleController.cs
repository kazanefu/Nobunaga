using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleController : MonoBehaviour
{
    [SerializeField] private string _mainSceneName = "MainScene";

    public void ClickStart()
    {
        SceneManager.LoadScene(_mainSceneName);
    }

    public void ClickExit()
    {
        Application.Quit();
    }

}
