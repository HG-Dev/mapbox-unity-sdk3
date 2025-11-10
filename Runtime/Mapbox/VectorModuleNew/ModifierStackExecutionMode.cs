namespace Mapbox.VectorModuleNew
{
    public enum ModifierStackExecutionMode
    {
        /// <summary>
        /// Execute all modifier stacks available
        /// </summary>
        All,
        /// <summary>
        /// Execute only the first modifier stack that passes its filter
        /// </summary>
        FirstHit
    }
}