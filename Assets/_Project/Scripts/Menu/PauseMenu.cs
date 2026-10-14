using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputEnabler enabler;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private TextMeshProUGUI loadingText;

    private const string MAIN_MENU_SCENE_NAME = "MainMenu";

    private void OnEnable() => enabler.OnPaused += TogglePause;

    private void OnDisable() => enabler.OnPaused -= TogglePause;

    public IEnumerator LoadMain()
    {
        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE_NAME);

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                loadingText.text = $"Press any key to continue...";
                loadingIcon.SetActive(false);

                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    asyncLoad.allowSceneActivation = true;

                    Time.timeScale = 1f;
                }
            }

            yield return null;
        }
    }

    public void TogglePause(bool isPaused)
    {
        pauseScreen.SetActive(isPaused);
    }

    public void ResumeButtonClicked()
    {
        pauseScreen.SetActive(false);
        enabler.CurrentlyPaused = false;
        Time.timeScale = 1f;
    }

    public void OpenOptions() => optionsScreen.SetActive(true);

    public void CloseOptions() => optionsScreen.SetActive(false);

    public void QuitToMainMenu() => StartCoroutine(LoadMain());
}