using System;
using JUSToolkit.Formats.ALAR;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUSToolkit.Converters.Alar
{
    /// <summary>
    /// Converter for ALAR files to Nodes.
    /// </summary>
    public class Alar2ToNodes : IConverter<ALAR2, NodeContainerFormat>, IConverter<NodeContainerFormat, ALAR2>
    {
         /// <summary>
        /// Converta an Alar2 container to a NodeContainerFormat.
        /// </summary>
        /// <param name="source">The Alar2 node.</param>
        /// <returns>The NodeContainerFormat.</returns>
        public NodeContainerFormat Convert(ALAR2 source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var container = new NodeContainerFormat();

            foreach (ALAR2File f in source.AlarFiles)
            {
                container.Root.Add(f.File);
            }

            return container;
        }

        /// <summary>
        /// Converts a NodeContainerFormat to an Alar2 container.
        /// </summary>
        /// <param name="container">The NodeContainerFormat.</param>
        /// <returns>The Alar2 container.</returns>
        public ALAR2 Convert(NodeContainerFormat container)
        {
            var aar = new ALAR2();

            foreach (Node n in container.Root.Children) {
                aar.AlarFiles.Add(new ALAR2File { File = n });
            }

            return aar;
        }
    }
}
