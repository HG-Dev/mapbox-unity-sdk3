using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.BaseModule.Data.DataFetchers;
using Mapbox.BaseModule.Data.Tasks;
using Mapbox.BaseModule.Data.Tiles;
using Mapbox.BaseModule.Map;
using Mapbox.BaseModule.Unity;
using UnityEngine;

namespace Mapbox.VectorModuleNew
{
    /// <summary>
    /// Queries a supplied <c>Source&lt;VectorData&gt;</c> object to process vector tiles via a set of <see cref="VectorLayerVisualizer"/> objects.
    /// </summary>
    public class VectorTileVisualizationModule : ITileVisualizationModule
    {
        // public Action<CanonicalTileId, IEnumerable<GameObject>> OnVectorMeshCreated = (tileId, gameobjects) => {};
        // public Action<CanonicalTileId> OnVectorMeshDestroyed = (tileId) => {};
        
        private bool _isActive = true;
        private UnityContext _unityContext;
        private Dictionary<CanonicalTileId, TaskWrapper> _activeTasks;
        private Dictionary<string, VectorLayerVisualizer> _layerVisualizers;
		
        private Source<VectorData> _vectorSource;
        private VectorSourceSettings _vectorSourceSettings;
        private IMapInformation _mapInformation;
		
        //tiles we need to cover the ideal tile list
        private HashSet<CanonicalTileId> _retainedTiles;
        private HashSet<CanonicalTileId> _readyTiles;
        private List<CanonicalTileId> _tilesToRemove;
        
        public VectorTileVisualizationModule(IMapInformation mapInformation, Source<VectorData> source, UnityContext unityContext,
            Dictionary<string, VectorLayerVisualizer> layerVisualizers, VectorSourceSettings settings = default)
        {
            _unityContext = unityContext;
            _layerVisualizers = layerVisualizers;
            _mapInformation = mapInformation;
            _vectorSource = source;
            _vectorSourceSettings = settings;
            _readyTiles = new HashSet<CanonicalTileId>();
            _vectorSource.CacheItemDisposed += ClearDisposedDataVisual;
            _retainedTiles = new HashSet<CanonicalTileId>();
            _activeTasks = new Dictionary<CanonicalTileId, TaskWrapper>();
            _tilesToRemove = new List<CanonicalTileId>(10);
        }
        
        private void ClearDisposedDataVisual(CanonicalTileId tileId)
        {
            if (_activeTasks.TryGetValue(tileId, out var task))
            {
                task.Cancel();
            }
            _readyTiles.Remove(tileId);
            foreach (var visualizer in _layerVisualizers)
            {
                visualizer.Value.UnregisterTile(tileId);
            }
        }

        /// <summary>
        /// Recycles an existing <see cref="UnityMapTile"/> component found on a tile GameObject under <see cref="UnityContext.BaseTileRoot">UnityContext.BaseTileRoot</see>.
        /// </summary>
        /// <param name="unityTile">A <see cref="UnityMapTile"/> in the scene</param>
        /// <returns>TODO: Figure out what readyTiles is for</returns>
        public bool LoadInstant(UnityMapTile unityTile)
        {
            var targetId = unityTile.CanonicalTileId.ClampZoomToAncestorOrSelf(_vectorSourceSettings.TileDataMaxZoom);
            Debug.Log($"Trying to recycle {unityTile.CanonicalTileId} (clamped to {targetId})   in supported zoom range? {_vectorSource.IsZinSupportedRange(targetId.Z)}");
            // var targetId = GetTargetTileId(unityTile.CanonicalTileId);
            // if (_readyTiles.Contains(targetId))
            //     return true;
			         //
            // //Debug.Log(string.Format("Load Instant {0}, {1}, {2}" ,unityTile.CanonicalTileId, _vectorSource.CheckInstantData(unityTile.CanonicalTileId), _visualCache.ContainsKey(unityTile.CanonicalTileId)));
            // if (!IsZinSupportedRange(targetId.Z)) return true;
            //
            // //this is wrong, it feels wrong
            // //tile doesn't need data, only yhe visual object. why are we checking for data
            // if (_vectorSource.GetInstantData(targetId, out var instantData) && 
            //     unityTile.TerrainContainer.State == TileContainerState.Final)
            // {
            //     if(!IsMeshGenInWork(targetId))
            //     {
            //         CreateVisual(targetId, instantData);
            //     }
            // }

            return false;
        }
        
        /// <summary>
        /// Given a sequence of tile IDs, filter applicable tile IDs into a hashset
        /// and run the LoadAndProcessTileCoroutine against all unique IDs
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerator LoadTiles(IEnumerable<CanonicalTileId> tiles)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerator LoadAndProcessTileCoroutine(CanonicalTileId tile)
        {
            VectorData tileData = null;
            yield return _vectorSource.LoadTileCoroutine(tile, data => tileData = data);
            if (tileData != null)
            {
               // yield return CreateVisualCoroutine(tile, tileData);
            }
        }

        public IEnumerable<IEnumerator> GetTileCoverCoroutines(IEnumerable<CanonicalTileId> tiles)
        {
            throw new NotImplementedException();
        }

        public bool RetainTiles(HashSet<CanonicalTileId> retainedTiles)
        {
            throw new NotImplementedException();
        }

        public IEnumerator Initialize()
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }

        public void UpdatePositioning(IMapInformation mapInfo)
        {
            throw new NotImplementedException();
        }

        public void LoadTempTile(UnityMapTile tile)
        {
            throw new NotImplementedException();
        }
    }
}