using UnityEngine;

[CreateAssetMenu(fileName = "CustomerInteraction",
    menuName = "ScriptableObjects/Manual/ManualPage")]
public class ManualPage : ScriptableObject
{
    [SerializeField] private Canvas _leftPage;
    [SerializeField] private Canvas _rightPage;
    
    public Canvas LeftPage => _leftPage;
    public Canvas RightPage => _rightPage;
}
