using UnityEngine;

public class RuletovInteractions : MonoBehaviour
{
    [SerializeField] private Transform _pieTransform1;
    [SerializeField] private Transform _pieTransform2;
    [SerializeField] private ShrinkDespawner _despawnerReference;
    private Customer _customer;

    private GameObject _spawnedPie1 = null;
    private GameObject _spawnedPie2 = null;

    private int _questsCompleted = -1;
    private CustomerQuest _currentQuest = null;

    private void Start()
    {
        _customer = GetComponent<Customer>();
        if (_customer == null)
        {
            Debug.LogError("[RuletovInteractions] No Customer component found on the GameObject.");
        }

        _currentQuest = _customer.Quest;
        if(_currentQuest == null)
        {
            Debug.LogError("[RuletovInteractions] Customer does not have a quest assigned.");
        }

        if(_despawnerReference == null)
        {
            _despawnerReference = GetComponent<ShrinkDespawner>();
            if (_despawnerReference == null) 
                Debug.LogError("[RuletovInteractions] No ShrinkDespawner reference assigned.");
        }

        //_customer.OnQuestCompleted += HandleQuestCompletion;
        _customer.QuestInitialized += UpdateQuest;
        _customer.ProductAccepted += SpawnPie;
    }
    public void UpdateQuest() {
        _currentQuest = _customer.Quest;
        if (_currentQuest == null)
        {
            Debug.LogError("[RuletovInteractions] Tried to update quest, but Customer does not have a quest assigned.");
        }

        _questsCompleted++;
        Debug.Log($"[RuletovInteractions] Quest completed. Total quests completed: {_questsCompleted}");
        // и тут мы будем отыгрывать разные уникальные взаимодесйствия нашего мафиозника в зависимости от количества выполненных квестов
        switch (_questsCompleted)
        {
            default:
                break;
            case 1:
                break;
            case 2:
                _customer.AnimatorController.SetCustomTrigger("Hesitant");
                _spawnedPie2.GetComponent<ShrinkDespawner>().DespawnSelf();
                break;
            case 3:
                break;
        }
    }

    public void SpawnPie(GameObject pieObj) {

        if (_spawnedPie1 == null)
        {
            _spawnedPie1 = Instantiate(pieObj, _pieTransform1.position, _pieTransform1.rotation, _pieTransform1);
            
        }
        else
        {
            _spawnedPie2 = Instantiate(pieObj, _pieTransform2.position, _pieTransform2.rotation, _pieTransform2);

            _spawnedPie2.AddComponent<ShrinkDespawner>();
            _spawnedPie2.GetComponent<ShrinkDespawner>().Setup(_despawnerReference.DespawnTime, _despawnerReference.ShrinkCurve);

            _customer.AnimatorController.SetCustomTrigger("Stare"); // тут везде хард-код,так как разовая фигня
        }
    }
}
