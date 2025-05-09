using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.ProBuilder.MeshOperations;

public class RigController : MonoBehaviour
{
    private RigBuilder _rigBuilder;
    private Dictionary<string, Rig> _rigLayersDictionanry = new Dictionary<string, Rig>();

    private void Awake()
    {
        _rigBuilder = GetComponent<RigBuilder>();
    }

    public void Init()
    {
        foreach(RigLayer rigLayer in _rigBuilder.layers)
        {
            if(rigLayer.rig != null)
                _rigLayersDictionanry[rigLayer.name] = rigLayer.rig;
            else
                Debug.LogWarning($"Rig '{rigLayer.name}' is null.");
        }
    }

    public void SetRigWeight(string rigLayerName, float weight = 1.0f)
    {
        _rigLayersDictionanry.TryGetValue(rigLayerName, out Rig rig);
        if (null != rig)
            rig.weight = Mathf.Lerp(rig.weight, weight, Time.deltaTime * 20f);
        else
            Debug.LogWarning($"Rig '{rig.name}' is null.");
    }

    public void SetMultiAimConstraint(string rigLayerName, GameObject targetObject)
    {
        if(!_rigLayersDictionanry.TryGetValue(rigLayerName, out Rig rig))
        {
            Debug.LogWarning($"Rig '{rigLayerName}' not found.");
            return;
        }

        MultiAimConstraint multiAimConstraint = rig.GetComponentInChildren<MultiAimConstraint>();
        if (null == multiAimConstraint)
        {
            Debug.LogWarning($"MultiAimConstraint is null in Rig '{rigLayerName}'.");
            return;
        }
        else
        {
            WeightedTransformArray sourceObject = multiAimConstraint.data.sourceObjects;
            sourceObject.Add(new WeightedTransform(targetObject.transform, 1.0f));

            multiAimConstraint.data.sourceObjects = sourceObject;

            _rigBuilder.Build();
        }            
    }
}
