using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MenuUIHandler : MonoBehaviour
{
    public MenuUIHandler instance { set; get; }

    public static event Action OnPlayClicked;

    public GameObject howToPlayScreen;
    public GameObject creditsScreen;
    public GameObject logo;
    public Slider musicSlider;

    void Start()
    {
        musicSlider.gameObject.SetActive(false);
        creditsScreen.gameObject.SetActive(false);
        howToPlayScreen.gameObject.SetActive(false);
        logo.gameObject.SetActive(true);
    }

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
        SceneManager.LoadScene(1);
        OnPlayClicked?.Invoke();
    }

    public void SettingsClicked()
    {
        bool shown = musicSlider.isActiveAndEnabled;
        if (!shown)
        {
            musicSlider.gameObject.SetActive(true);
        }
        else
        {
            musicSlider.gameObject.SetActive(false);
        }
    }

    public void HowToPlayClicked()
    {
        howToPlayScreen.SetActive(true);
        logo.gameObject.SetActive(false);
    }

    public void CreditsClicked()
    {
        creditsScreen.SetActive(true);
        logo.gameObject.SetActive(false);
    }

    // for back button in how-to-play/credits screens
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


    // for credits screen XD
    public void MoazLinkedInButton()
    {
        Application.OpenURL("https://www.linkedin.com/in/moaz-nasser-a81a24388");
    }

    public void MoazButton()
    {
        Application.OpenURL("https://mr-nobody4444.github.io/Portfolio/");
    }

    public void HabibaLinkedInButton()
    {
        Application.OpenURL("https://www.linkedin.com/in/habiba-saad-5a343b357?utm_source=share_via&utm_content=profile&utm_medium=member_ios");
    }

    public void HabibaButton()
    {
        Application.OpenURL("https://habiba-sa3d.itch.io/");
    }

    public void AyaLinkeInButton()
    {
        Application.OpenURL("https://www.linkedin.com/in/ayasafwat/");
    }

    public void AyaButton()
    {
        Application.OpenURL("https://yonkicore.itch.io/");
    }
}
