using System;
using JUSToolkit.Formats.ALAR;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUSToolkit.Converters.Alar
{
    public class Alar3ToNodes : IConverter<Alar3, NodeContainerFormat>, IConverter<NodeContainerFormat, Alar3>
    {
        public NodeContainerFormat Convert(Alar3 aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            return aar.AlarFiles;
        }

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
