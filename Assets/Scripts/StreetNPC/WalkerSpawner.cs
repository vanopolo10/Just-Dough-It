using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CustomerModelSpawner))]
public class WalkerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> _models;

    [Header("Routes")]
    [SerializeField] private List<Route> _routes;
    [SerializeField] private float _minSpawnDelay = 2f;
    [SerializeField] private float _maxSpawnDelay = 5f;

    [Header("NpcMover")]
    [SerializeField] private NpcMover _npcMover;

    [Header("Destruction")]
    [SerializeField] private bool _destroyAtEnd = true;
    [SerializeField] private float _destroyDelay = 0f;

    private CustomerModelSpawner _modelSpawner;
    private bool _isSpawning = true;

    private void Awake()
    {
        _modelSpawner = GetComponent<CustomerModelSpawner>();

        if (_npcMover == null)
            Debug.LogError("NpcMover not assigned in WalkerSpawner!");
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (_isSpawning)
        {
            var availableRoutes = _routes.Where(r => r.IsAvailable).ToList();
            
            if (availableRoutes.Count > 0)
            {
                var route = availableRoutes[Random.Range(0, availableRoutes.Count)];
                route.Occupy();

                GameObject prefab = _models[Random.Range(0, _models.Count)];
                GameObject spawned = _modelSpawner.Spawn(prefab, route.Points[0]);

                if (spawned && _npcMover)
                    _npcMover.MoveRoutine(MoveNpcRoutine(spawned.transform, route));
            }

            yield return new WaitForSeconds(Random.Range(_minSpawnDelay, _maxSpawnDelay));
        }
    }

    private IEnumerator MoveNpcRoutine(Transform npc, Route route)
    {
        for (int i = 0; i < route.Points.Count; i++)
        {
            Transform point = route.Points[i];

            yield return _npcMover.FaceTo(npc, point.position);
            yield return _npcMover.MoveTo(npc, point.position);
        }

        if (_destroyAtEnd && npc)
        {
            if (_destroyDelay > 0f)
                yield return new WaitForSeconds(_destroyDelay);

            Destroy(npc.gameObject);
        }

        route.Free();
    }
    
    public void StopSpawning() => _isSpawning = false;
    
    public void StartSpawning()
    {
        if (!_isSpawning)
        {
            _isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }
    
    public void StopNPC(Transform npc)
    {
        if (_npcMover != null)
            _npcMover.Stop(npc);
    }
    
    public void StopAllNPCs()
    {
        if (_npcMover != null)
            _npcMover.StopAll();
    }

    private void OnDestroy()
    {
        StopAllNPCs();
    }
}