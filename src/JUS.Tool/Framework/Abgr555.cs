using System.Buffers.Binary;
using Texim.Colors;

namespace JUS.Tool.Framework;

/// <summary>
/// ABGR555 color encoding: 5 bits per channel in blue, green, red order, with
/// one bit for alpha chanel. Encoded in little-endian format.
/// </summary>
public class Abgr555Encoding : LinealByteColorEncoding
{
    /// <summary>
    /// Gets a singleton instance of this class.
    /// </summary>
    public static Abgr555Encoding Instance { get; } = new();

    /// <inheritdoc/>
    public override int BytesPerColor => 2;

    /// <summary>
    /// Decodes a color from an unsigned integer of 16-bits.
    /// </summary>
    /// <param name="value">The binary encoded color.</param>
    /// <returns>The decoded color.</returns>
    public static Rgb FromUInt16(ushort value)
    {
        Rgb bgr555 = Bgr555Encoding.FromUInt16(value);
        byte alpha = (byte)((value >> 15) == 0 ? 0 : 255);
        return new Rgb(bgr555, alpha);
    }

    /// <summary>
    /// Encodes a color as an unsigned integer of 16-bits.
    /// </summary>
    /// <param name="color">The color to binary encode.</param>
    /// <returns>The binary encoded color.</returns>
    public static ushort ToUInt16(Rgb color)
    {
        ushort encoded = Bgr555Encoding.ToUInt16(color);
        encoded |= (ushort)((color.Alpha == 0 ? 0 : 1) << 15);
        return encoded;
    }

    /// <inheritdoc />
    protected override Rgb ReadColor(ReadOnlySpan<byte> data)
    {
        ushort value = BinaryPrimitives.ReadUInt16LittleEndian(data);
        return FromUInt16(value);
    }

    /// <inheritdoc />
    protected override void WriteColor(Span<byte> output, Rgb color)
    {
        ushort value = ToUInt16(color);
        BinaryPrimitives.WriteUInt16LittleEndian(output, value);
    }
}
