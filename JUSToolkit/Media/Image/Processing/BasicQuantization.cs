// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace Texim.Media.Image.Processing
{
    using System.Collections.Generic;
    using System.Drawing;

    public class BasicQuantization : ColorQuantization
    {
        readonly List<Color> listColor;
        NearestNeighbour<Color> nearestNeighbour;
        Bitmap image;

        public BasicQuantization()
        {
            listColor = new List<Color>();
            MaxColors = 256;
        }

        public int MaxColors {
            get;
            set;
        }

        protected override void PreQuantization(Bitmap image)
        {
            listColor.Clear();
            nearestNeighbour = null;
            this.image = image;
        }

        protected override Pixel QuantizatePixel(int x, int y)
        {
            // Get the color and add to the list
            Color color = image.GetPixel(x, y);

            int colorIndex;
            if (listColor.Count < MaxColors) {
                if (!listColor.Contains(color))
                    listColor.Add(color);
                colorIndex = listColor.IndexOf(color);
            } else {
                // Create the labpalette if so
                if (nearestNeighbour == null) {
                    Color[] labPalette = listColor.ToArray();
                    nearestNeighbour = new ExhaustivePaletteSearch();
                    nearestNeighbour.Initialize(labPalette);
                }

                Color labNoTrans = color;
                colorIndex = nearestNeighbour.Search(labNoTrans);
            }

            return new Pixel((uint)colorIndex, color.A, true);
        }

        protected override void PostQuantization()
        {
            Palette = listColor.ToArray();
        }
    }
}

