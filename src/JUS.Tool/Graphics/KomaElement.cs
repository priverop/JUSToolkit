// Copyright (c) 2021 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Image Format - Individual Koma OAM (square).
    /// </summary>
    public class KomaElement
    {
        /// <summary>
        /// Gets or sets the ImageId.
        /// </summary>
        public int ImageId { get; set; }

        /// <summary>
        /// Gets or sets the Unknown2.
        /// </summary>
        public int Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the name of the koma.
        /// </summary>
        public string KomaName { get; set; }

        /// <summary>
        /// Gets or sets the Unknown6.
        /// </summary>
        public byte Unknown6 { get; set; }

        /// <summary>
        /// Gets or sets the Unknown7.
        /// </summary>
        public byte Unknown7 { get; set; }

        /// <summary>
        /// Gets or sets the KShapeGroupId.
        /// </summary>
        public byte KShapeGroupId { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public byte KShapeElementId { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public byte UnknownA { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public byte UnknownB { get; set; }
    }
}
