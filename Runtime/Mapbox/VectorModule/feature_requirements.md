# Vector layer requirements
The vector layer module must support visualizations of [vector map data.](https://www.mapbox.com/insights/vector-maps)
## What is vector map data?
Vector map data encapsulates everything necessary to render maps in the form of points, lines, and polygons, and derive their overarching from GeoJSON.
> Vector tiles are encoded as Google Protobufs (PBF)... For clarity, Mapbox Vector Tiles use the [.mvt file suffix](https://gdal.org/en/stable/drivers/vector/mvt.html).
> Mapbox Vector Tiles and OpenStreetMap PBFs are protobuf files, but conform to completely different specifications and are used in different ways.

[From the standards page](https://docs.mapbox.com/data/tilesets/guides/vector-tiles-standards/)
> Vector tiles hold no concept of geographic information. They encode points, lines, and polygons as x/y pairs relative to the top left of the grid in a right-down manner.
> * Note: Points, line \[strings\], and polygons are the same geometry types as defined in GeoJSON.
> For the sake of compression, attributes are encoded in a series of tags
 
See also: [vector_tile.proto](https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto)

## VectorLayerModule
We have seen that there are three main geometry types, or more loosely 'features', that require visualization. Thus, the VectorLayerModule is said to be responsible for directing the visualization of features in currently viewed tiles at their current zoom levels, obtained from a single API source.
* One VectorLayerModule = one vector tile API

## VectorLayerVisualizer
One tile can have multiple layers of features. Vector layer visualizers handle one layer as found in the tile data supplied from the VectorLayerModule.
* One VectorLayerVisualizer = one layer amongst multiple vector tiles; one layer can have multiple features

### Types of features
1. Points
    * POI textured models
        * These could include important landmarks like the Eiffel Tower, Tokyo Skytree, Space Needle, Big Ben, etc.
        * Must be placed at terrain heights, prefabs chosen through vector feature attribute data
    * POI labels
        * In addition to the POI models themselves, we may wish to show information about these POIs on-screen in the form of worldspace or screenspace text
2. Lines
    * Roads
    * Routing
    * Labels
        * As with POIs, there should be demand to render UI visualizations of information displayed as lines
            * Street names, routing distances / ETAs
3. Polygons
    * Buildings
        * must be placed on terrain heights, must be extruded upwards to match building height
    * Labels
        * See above

## ModifierStack
Conceptually, ModifierStack exists to visualize feature data by processing it in stages. However, there is much variety in the stages actually required for each feature.
* Filtering remains relevant for all visualizations
  * One new idea: preventing road labels from rendering multiple times once for each tile
* Mesh generation is relevant for physical representations of lines and polygons _only_
* GameObject modifiers, being something of a catch-all, are relevant for most use cases but are vague in their intention and correct step order 
