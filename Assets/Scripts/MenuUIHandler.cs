using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MenuUIHandler : MonoBehaviour
{
    public MenuUIHandler instance { set; get; }

    public static event Action OnPlayClicked;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // functions needed for the buttons
    public void PlayClicked() 
    {
        SceneManager.LoadScene(3);
        OnPlayClicked?.Invoke();
    }

    public void SettingsClicked()
    {

    }

    public void HowToPlayClicked()
    {

        SceneManager.LoadScene(2);
    }

    // for back button in how-to-play/settings screens
    // may need to do seperate one for the settinfs if needed to save settings new data
    public void BackClicked() 
    {
        SceneManager.LoadScene(0);
    }

    public void QuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
