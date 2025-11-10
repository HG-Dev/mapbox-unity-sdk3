using System;
using System.Collections.Generic;
using Mapbox.BaseModule.Data.Interfaces;
using Mapbox.BaseModule.Map;
using Mapbox.BaseModule.Unity;
using Mapbox.BaseModule.Utilities;
using UnityEngine;

namespace Mapbox.VectorModuleNew.Unity
{
    /// <summary>
    /// Constructs a single ITileVisualizationModule (<see cref="ILayerModule"/>) during runtime for usage by
    /// <see cref="Mapbox.Example.Scripts.Map.MapboxMapBehaviour"/> or <see cref="CompositeLayerModuleScript"/>.
    /// MapboxMapBehaviour requires this script to inherit from ModuleConstructorScript so it can be referenced with <c>GetComponents&lt;ModuleConstructorScript&gt;()</c>.
    /// </summary>
    public class VectorTileApiModuleScript : ModuleConstructorScript
    {
        [Tooltip("Settings that govern the overall usage of vector tile data")]
        [SerializeField] private VectorSourceSettingsBuilder settingsBuilder;
        
        [Tooltip("Settings that govern the overall usage of vector tile data")]
        [SerializeField] private List<VectorLayerVisualizerObject> _layerVisualizers;
        
        public ITileVisualizationModule ModuleImplementationInternal { get; protected set; }

        public override ILayerModule ModuleImplementation
        {
            get => ModuleImplementationInternal;
            protected set => throw new NotImplementedException();
        }

        public override ILayerModule ConstructModule(MapService service, IMapInformation mapInformation, UnityContext unityContext)
        {
            var dictionary = new Dictionary<string, VectorLayerVisualizer>();
            foreach (var visualizerObject in _layerVisualizers)
            {
                if (visualizerObject == null)
                {
                    Debug.LogWarning($"{name} ({nameof(VectorTileApiModuleScript)}) possesses an empty layer visualizer");
                    continue;
                }
                
                var visualizer = visualizerObject.ConstructLayerVisualizer(mapInformation, unityContext);
                dictionary.Add(visualizer.VectorLayerName, visualizer);
            }

            var vectorDataSettings = settingsBuilder.BuildSettings();
            
            ModuleImplementationInternal = new VectorTileVisualizationModule(mapInformation, service.GetVectorSource(vectorDataSettings), unityContext, dictionary, vectorDataSettings);
            return ModuleImplementation;
        }
        
        public override void OnDestroy()
        {
            ModuleImplementation?.OnDestroy();
        }
    }
}