using System.Collections.Generic;
using Mapbox.BaseModule.Data.Interfaces;
using Mapbox.BaseModule.Map;
using Mapbox.BaseModule.Unity;
using Mapbox.BaseModule.Utilities;
using Mapbox.VectorModule.MeshGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mapbox.VectorModule.Unity
{
	public class VectorLayerModuleScript : ModuleConstructorScript
	{
		[FormerlySerializedAs("vectorModuleSettings")] 
		[SerializeField] private VectorSourceSettingsBuilder settingsBuilder;
		[FormerlySerializedAs("_layerVisualizers")] 
		[SerializeField] private List<VectorLayerVisualizerObject> layerVisualizers;
		public override ILayerModule ModuleImplementation { get; protected set; }

		public void Start()
		{
			
		}

		public override ILayerModule ConstructModule(MapService service, IMapInformation mapInformation, UnityContext unityContext)
		{
			var dictionary = new Dictionary<string, IVectorLayerVisualizer>();
			foreach (var visualizerObject in layerVisualizers)
			{
				if(visualizerObject == null) continue;
				var visualizer = visualizerObject.ConstructLayerVisualizer(mapInformation, unityContext);
				dictionary.Add(visualizer.VectorLayerName, visualizer);
			}
			ModuleImplementation = GetVectorLayerModule(mapInformation, unityContext, service, dictionary);
			return ModuleImplementation;
		}
		
		private VectorLayerModule GetVectorLayerModule(IMapInformation mapInformation, UnityContext unityContext,
			MapService service, Dictionary<string, IVectorLayerVisualizer> dictionary)
		{
			var vectorDataSettings = settingsBuilder.BuildSettings();
			
			return new VectorLayerModule(mapInformation, service.GetVectorSource(vectorDataSettings), unityContext, dictionary, vectorDataSettings);
		}

		public override void OnDestroy()
		{
			ModuleImplementation?.OnDestroy();
		}
	}
}