using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    
    [SerializeField] private float _postLoadDelay = 0.3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(Load(index));
    }

    private IEnumerator Load(int index)
    {
        var darkness = Darkness.Instance;

        darkness.FadeIn();
        yield return new WaitUntil(() => darkness.IsDark());

        var op = SceneManager.LoadSceneAsync(index);
        op.allowSceneActivation = false;

        yield return new WaitUntil(() => op.progress >= 0.9f);

        op.allowSceneActivation = true;

        yield return null;
        yield return null;
        yield return new WaitForSecondsRealtime(_postLoadDelay);

        darkness.FadeOut();
    }
}