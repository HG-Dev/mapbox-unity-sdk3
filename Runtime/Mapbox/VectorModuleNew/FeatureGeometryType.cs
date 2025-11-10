namespace Mapbox.VectorModuleNew
{
    /// <summary>
    /// The kind of geometrical data used to define a single feature.
    /// Refer to https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto
    /// </summary>
    public enum FeatureGeometryType
    {
        Unknown,
        Point,
        Line,
        Polygon
    }
}