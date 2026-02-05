using System.Collections.Generic;
using UnityEngine;

public class ModelRandomizer : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _face;
    [SerializeField] private SkinnedMeshRenderer _hat;
    [SerializeField] private SkinnedMeshRenderer _hatNoble;
    [SerializeField] private SkinnedMeshRenderer _glasses;
    [SerializeField] private SkinnedMeshRenderer _sunglasses;
    [SerializeField] private SkinnedMeshRenderer _scarf;
    [SerializeField] private SkinnedMeshRenderer _collar;
    [SerializeField] private SkinnedMeshRenderer _pants;
    [SerializeField] private SkinnedMeshRenderer _gloves;
    [SerializeField] private SkinnedMeshRenderer _coat;
    
    [SerializeField] private List<Material> _faceMats;
    [SerializeField] private List<Material> _clothMats;

    public void Start()
    {
        TurnAndColorMeshes();
    }
    public void TurnAndColorMeshes()
    {
        if (_face != null && _faceMats.Count > 0)
            _face.material = _faceMats[Random.Range(0, _faceMats.Count)];

        if (_clothMats is not { Count: > 0 })
            return;

        Material coatMat = _clothMats[Random.Range(0, _clothMats.Count)];

        if (_coat != null)
            _coat.material = coatMat;
        
        if (_pants != null)
            _pants.material = _clothMats[Random.Range(0, _clothMats.Count)];
        
        if (_gloves != null)
            _gloves.material = _clothMats[Random.Range(0, _clothMats.Count)];
            
        if (_scarf != null)
            switch (Random.Range(0, 2))
            {
                case 0:
                    _collar.material = coatMat;
                    _scarf.enabled = false;
                    break;
                case 1:
                    _collar.enabled = false;
                    _scarf.material = _clothMats[Random.Range(0, _clothMats.Count)];
                    break;
            }

        if(_hat != null & _hatNoble != null)
            switch (Random.Range(0, 2))
            {
                case 0:
                    _hat.material = _clothMats[Random.Range(0, _clothMats.Count)];
                    _hatNoble.enabled = false;
                    break;
                case 1:
                    _hat.enabled = false;
                    _hatNoble.material = _clothMats[Random.Range(0, _clothMats.Count)];
                    break;
            }

        if(_glasses != null & _sunglasses != null)
            switch (Random.Range(0, 5))
            {
                case 0:
                case 1:
                case 2:
                    _glasses.enabled = false;
                    _sunglasses.enabled = false;
                    break;
                case 3:
                    _glasses.enabled = false;
                    _sunglasses.material = _clothMats[Random.Range(0, _clothMats.Count)];
                    break;
                case 4:
                    _sunglasses.enabled = false;
                    _glasses.material = _clothMats[Random.Range(0, _clothMats.Count)];
                    break;
            }
    }
}
