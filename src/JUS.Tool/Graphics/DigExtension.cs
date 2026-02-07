namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Bpp of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigBpp
    {
        /// <summary>
        /// 4 bpp mode.
        /// </summary>
        Bpp4 = 0,

        /// <summary>
        /// 8 bpp mode.
        /// </summary>
        Bpp8 = 1,

        /// <summary>
        /// 2 bpp mode.
        /// </summary>
        Bpp2 = 2,
    }

    /// <summary>
    /// Swizzling of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigSwizzling
    {
        /// <summary>
        /// Tiled swizzling
        /// </summary>
        Tiled = 1,

        /// <summary>
        /// Linear swizzling
        /// </summary>
        Linear = 2,
    }
}
