using System;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converter between Alar2 and NodeFormatContainer.
    /// </summary>
    public class Alar2ToNodes : IConverter<Alar2, NodeContainerFormat>, IConverter<NodeContainerFormat, Alar2>
    {
         /// <summary>
        /// Converta an Alar2 container to a NodeContainerFormat.
        /// </summary>
        /// <param name="source">The Alar2 node.</param>
        /// <returns>The NodeContainerFormat.</returns>
        public NodeContainerFormat Convert(Alar2 source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var container = new NodeContainerFormat();

            foreach (Alar2File f in source.AlarFiles)
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
        public Alar2 Convert(NodeContainerFormat container)
        {
            var aar = new Alar2();

            foreach (Node n in container.Root.Children) {
                aar.AlarFiles.Add(new Alar2File { File = n });
            }

            return aar;
        }
    }
}
