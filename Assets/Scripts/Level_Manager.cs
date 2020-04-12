using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Manager : Singleton<Level_Manager>
{
    protected Level_Manager() {}
    public Animator animator;
    private int levelToLoad;
    private int howToPlayLevel;
    private int creditLevel;
    // Update is called once per frame
    void Start()
    {
        howToPlayLevel = 2;
        creditLevel = 3;
    }

    public void FadeToLevel(int levelIndex) {
        levelToLoad = levelIndex;
        animator.SetTrigger("fadeOut");
    }

    public void FadeToMainMenu() {
        // reset score and lives
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Lives", 3);
        FadeToLevel(0);
    }

    public void FadeToHowToPlay() {
        FadeToLevel(howToPlayLevel);
    }

    public void FadeToCredit() {
        FadeToLevel(creditLevel);
    }

    public void FadeToNextLevel() {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnFadeComplete() {
        SceneManager.LoadScene(levelToLoad);
    }

    public void ExitGame() {
        Application.Quit();
    }

}
