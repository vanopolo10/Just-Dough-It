using UnityEngine;

public class ProductCountDisplay : MonoBehaviour
{
    [SerializeField] private string _baseTextKey;
    [SerializeField] private CustomerManager _manager;

    private TextTypewriter _text;
    private Customer _customer;

    private void OnEnable()
    {
        _text = GetComponent<TextTypewriter>();
        
        if (_manager == null)
            _manager = FindAnyObjectByType<CustomerManager>();

        _manager.CustomerSpawned += RecordCustomer;
    }

    private void OnDisable()
    {
        _manager.CustomerSpawned -= RecordCustomer;
    }

    private void Start()
    {
        _text.Clear();
    }

    private void RecordCustomer(Customer customer)
    {
        print("[ProductCountDisplay] Recording customer " + customer.name);
        
        if (customer.GetType() == typeof(MultiQuestCustomer))
        {
            print("[ProductCountDisplay] Spawned customer is a multi quest Customer. Ignoring.");
            return;
        }

        _customer = customer;

        customer.QuestInitialized += PrepareDisplay;
    }

    private void PrepareDisplay()
    {
        _customer.Quest.QuestStarted += UpdateDisplay;
        _customer.ProductAccepted += UpdateDisplay;
        // _customer.QuestCompleted += CleanUp;
    }

    private void UpdateDisplay(GameObject _)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        print("[ProductCountDisplay] Updating display for customer " + _customer.name);
        
        if (_customer.Quest.ProductsLeft == 0)
        {
            CleanUp();
            return;
        }

        _ = _text.StartTyping(_baseTextKey, _customer.Quest.ProductsLeft.ToString());
    }

    private void CleanUp()
    {
        print("[ProductCountDisplay] Cleaning up display for customer " + _customer.name);
        
        _customer.QuestInitialized -= UpdateDisplay;
        _customer.ProductAccepted -= UpdateDisplay;
        _customer.QuestCompleted -= CleanUp;
        _text.Clear();
    }
}