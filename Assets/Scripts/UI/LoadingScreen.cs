using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject _darkness;

    private void Awake()
    {
        _darkness.SetActive(true);
    }
}
