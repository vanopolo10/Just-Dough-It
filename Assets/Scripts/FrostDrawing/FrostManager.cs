using UnityEngine;
using System;
public class FrostManager : MonoBehaviour
{
    public static event Action OnResetAll;

    public static void ResetAllWindows()
    {
        OnResetAll?.Invoke();
    }
}

