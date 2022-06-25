using System;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converts between ALAR3 and Nodes.
    /// </summary>
    public class Alar3ToNodes : IConverter<Alar3, NodeContainerFormat>, IConverter<NodeContainerFormat, Alar3>
    {
        /// <summary>
        /// Converts Alar3 to NodeContainerFormat.
        /// </summary>
        /// <param name="aar">Alar3 Node.</param>
        /// <returns>NodeContainerFormat Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="aar"/> is <c>null</c>.</exception>
        public NodeContainerFormat Convert(Alar3 aar) => aar == null ? throw new ArgumentNullException(nameof(aar)) : aar.AlarFiles;

        /// <summary>
        /// Converts NodeContainerFormat Node to Alar3 Container.
        /// </summary>
        /// <param name="container">NodeContainerFormat Node.</param>
        /// <returns>Alar3 Container.</returns>
        public Alar3 Convert(NodeContainerFormat container)
        {
            var aar = new Alar3();

            foreach (Node n in container.Root.Children) {
                // New node to avoid Disposal
                var newNode = new Node(n.Name, new Alar3File(n.Stream));

                aar.AlarFiles.Root.Add(newNode);
            }

            return aar;
        }
    }
}
