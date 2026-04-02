using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerSchedule", menuName = "ScriptableObjects/CustomerSystem/CustomerSchedule")]
public class CustomerSchedule : ScriptableObject
{
    [SerializeField] private List<CustomerPool> _list;
    [SerializeField] private CustomerSchedule _nextDaySchedule;

    public List<CustomerPool> List => _list;
    public CustomerSchedule NextDaySchedule => _nextDaySchedule;
}
