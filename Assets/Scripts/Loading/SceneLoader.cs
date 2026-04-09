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

        darkness.FadeIn(0);
        yield return new WaitUntil(() => darkness.IsDark());

        SceneManager.LoadScene(index);
        yield return new WaitForSecondsRealtime(_postLoadDelay);

        darkness.FadeOut(0);
    }
}