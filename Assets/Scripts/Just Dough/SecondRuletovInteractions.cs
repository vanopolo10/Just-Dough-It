using UnityEngine;

public class SecondRuletovInteractions : MonoBehaviour
{
    private Customer _customer;
    [SerializeField] private GameObject _mafioziPrefab;
    [SerializeField] private GameObject _mafiozi;
    [SerializeField] private Animator _mafioziAnimator, _mafioziDisplacementAnimator;

    private void Start()
    {
        _customer = GetComponent<Customer>();
        _customer.OnCounterReached += OnCounterReached;

        _mafiozi = Instantiate(_mafioziPrefab, transform.position, Quaternion.identity);
        _mafioziAnimator = _mafiozi.transform.GetChild(0).GetComponent<Animator>();
        _mafioziDisplacementAnimator = _mafiozi.GetComponent<Animator>();

        CustomerRouteMover routeMover = FindAnyObjectByType<CustomerRouteMover>();
        CustomerRouteMover added = _mafiozi.AddComponent<CustomerRouteMover>();
        added.CopyValues(routeMover, FindAnyObjectByType<NpcMover>());
        added.Initialize(_mafioziAnimator.gameObject.GetComponent<CustomerAnimatorController>());
    }

    public void OnCounterReached() {
        //_mafioziAnimator.SetBool("IsWalking", false);
        _mafioziDisplacementAnimator.SetTrigger("StartDisplacement");
    }
}
