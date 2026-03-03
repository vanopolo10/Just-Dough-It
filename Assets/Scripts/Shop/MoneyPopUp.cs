using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MoneyPopUp : MonoBehaviour
{
    public void Initialize(int money)
    {
        GetComponent<TMP_Text>().text = "+" + money;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
