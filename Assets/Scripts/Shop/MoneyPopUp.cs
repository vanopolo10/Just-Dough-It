using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MoneyPopUp : MonoBehaviour
{
    private Animation _animation;
    
    public void Initialize(int money)
    {
        GetComponent<TMP_Text>().text = "+" + money;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
