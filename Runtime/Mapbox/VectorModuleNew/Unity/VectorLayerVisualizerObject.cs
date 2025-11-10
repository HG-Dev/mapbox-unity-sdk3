using System;
using Mapbox.BaseModule.Map;
using Mapbox.BaseModule.Unity;
using UnityEngine;

namespace Mapbox.VectorModuleNew.Unity
{
    /// <summary>
    /// Constructs a single <see cref="VectorLayerVisualizer"/> with added vector feature processing stacks.
    /// </summary>
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Layer Visualizer (New)")]
    public class VectorLayerVisualizerObject : ScriptableObject
    {
        [SerializeField] private string _vectorLayerName;
        public string VectorLayerName => _vectorLayerName;

        [SerializeField] private VectorLayerVisualizerSettings _settings;
        //[SerializeField] private List<ModifierStackObject> _modifierStackObjects;
        // TODO: Replace with lazy or instantiated flag?
        private VectorLayerVisualizer _layerVisualizer;
		
        public VectorLayerVisualizer ConstructLayerVisualizer(IMapInformation mapInformation, UnityContext unityContext)
        {
            if (_layerVisualizer != null)
                throw new InvalidOperationException(
                    $"{name} ({nameof(VectorLayerVisualizerObject)}) was asked to construct {nameof(VectorLayerVisualizer)} twice");
            
            _layerVisualizer = new VectorLayerVisualizer(VectorLayerName, mapInformation, unityContext, _settings);
            _layerVisualizer.Active = true;
			
            // foreach (var modifierStackObject in _modifierStackObjects.Where(x => x != null))
            // {
            //     modifierStackObject.Initialize(unityContext);
            // }
            //_layerVisualizer.AddModifierStack(_modifierStackObjects.Select(x => x.GetModifierStack).ToList());
			
            return _layerVisualizer;
        }
    }
}