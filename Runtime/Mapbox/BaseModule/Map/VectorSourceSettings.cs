using System;
using Mapbox.BaseModule.Data.Interfaces;
using Mapbox.BaseModule.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mapbox.BaseModule.Map
{
    [Serializable]
    public class VectorSourceSettingsBuilder
    {
        [SerializeField]
        private VectorSourceType sourceType;
        [SerializeField]
        private string customSourceId;
        
        [FormerlySerializedAs("CacheSize")] 
        [SerializeField]
        private int cacheSize = 100;
        
        [Tooltip("Tile outside this range will be rejected.")]
        [SerializeField]
        private Vector2Int processTileDataInZoomRange = new Vector2Int(12, 16);
        
        [FormerlySerializedAs("ClampDataLevelToMax")] 
        [Tooltip("Maximum data level that'll be used for this module. Tiles can be higher zoom level but data will be lower level.")]
        [SerializeField]
        // Recommendation: "clampTileDataToMaxZoom" or "tileDataMaxZoom" -- 'data level' is only heard of in this context
        private int clampTileDataToMaxZoom = 15;

        public VectorSourceSettings BuildSettings() => sourceType is VectorSourceType.Custom
            ? WithCustomVectorSource(customSourceId)
            : WithCommonVectorSource(sourceType);
        
        private VectorSourceSettings WithCommonVectorSource(VectorSourceType srcType) =>
            new VectorSourceSettings(MapboxDefaultVector.GetParameters(srcType).Id, cacheSize,
                processTileDataInZoomRange.x, processTileDataInZoomRange.y, clampTileDataToMaxZoom);
        private VectorSourceSettings WithCustomVectorSource(string src) =>
            new VectorSourceSettings(src, cacheSize,
                processTileDataInZoomRange.x, processTileDataInZoomRange.y, clampTileDataToMaxZoom);
    }

    public readonly struct VectorSourceSettings
    {
        public VectorSourceSettings(string tilesetId, int cacheSize, int minZoom, int maxZoom, int clampTileDataToMaxZoom) => 
            (TilesetId, CacheSize, MinZoom, MaxZoom, TileDataMaxZoom) = (tilesetId, cacheSize, minZoom, maxZoom, clampTileDataToMaxZoom);
        /// <summary>
        /// TODO: Explain tileset ID value
        /// </summary>
        public readonly string TilesetId;
        /// <summary>
        /// TODO: Explain cache size value
        /// </summary>
        public readonly int CacheSize;
        /// <summary>
        /// Minimum zoom required to begin processing tiles
        /// </summary>
        public readonly int MinZoom;
        /// <summary>
        /// Maximum zoom (inclusive) before tile processing stops
        /// </summary>
        public readonly int MaxZoom;
        /// <summary>
        /// If the current zoom level exceeds this value, and this value is lower than MaxZoom,
        /// the tile data used for visualization will bne clamped to this value
        /// </summary>
        public readonly int TileDataMaxZoom;
        
        /// <summary>
        /// Validate a given zoom level against min and max zoom levels (inclusive).
        /// </summary>
        /// <param name="zoomLevel">A zoom level to be validated</param>
        /// <returns>True if the value falls between min/max zoom levels (inclusive), otherwise false</returns>
        public bool ValidateZoomLevel(int zoomLevel) => zoomLevel >= MinZoom && zoomLevel <= MaxZoom;
    }
}