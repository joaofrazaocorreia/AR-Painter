using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingScreen;

    private void Start()
    {
        loadingScreen.blocksRaycasts = false;
        loadingScreen.gameObject.SetActive(false);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(UIManager.LoadSceneCoroutine(index, loadingScreen));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
