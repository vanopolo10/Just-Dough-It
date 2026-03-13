using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _buttonSprites;
    [SerializeField] private float _spriteChangeRate;
    
    private Image _buttonImage;

    private void Start()
    {
        _buttonImage = GetComponent<Image>();
        StartCoroutine(ChangeImagePerTime());
    }

    private IEnumerator ChangeImagePerTime()
    {
        int index = 0;
        
        while (true)
        {
            _buttonImage.sprite = _buttonSprites[index];
            index = (index + 1) % _buttonSprites.Length;
            yield return new WaitForSeconds(_spriteChangeRate);
        }
    }
}
