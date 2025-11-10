using Mapbox.BaseModule.Data.Interfaces;

namespace Mapbox.VectorModuleNew
{
    /// <summary>
    /// RECOMMENDATION: Rename ILayerModule to ITileVisualizationModule or ITileRenderer.
    /// Tiles possess layers, so it may be confusing to realize that ILayerModule handles tiles, not tile layers.
    /// </summary>
    public interface ITileVisualizationModule : ILayerModule
    {
    }
}