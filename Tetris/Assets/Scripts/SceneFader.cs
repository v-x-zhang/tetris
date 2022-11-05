using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    #region Singleton

    public static SceneFader instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public Animator animator;

    int nextScene;
    public void FadeTo(int sceneIndex)
    {
        animator.SetTrigger("FadeOut");
        nextScene = sceneIndex;
    }

    public void OnEndFade()
    {
        StartCoroutine(LoadAsynchronously(nextScene));

    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        while (!operation.isDone)
        {
            yield return null;
        }

    }

}
