namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using JUSToolkit.Formats.ALAR;

    public class Alar3ToNodes : 
        IConverter<ALAR3, NodeContainerFormat>,
        IConverter<NodeContainerFormat, ALAR3>
    {

        // *** TODO: Replace this to return aar.AlarFiles?
        public NodeContainerFormat Convert(ALAR3 aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            var container = new NodeContainerFormat();

            foreach (Node n in aar.AlarFiles.Root.Children)
            {
                container.Root.Add(n);
            }

            return container;
        }

        public ALAR3 Convert(NodeContainerFormat container)
        {
            ALAR3 aar = new ALAR3();

            foreach(Node n in container.Root.Children){

                // New node to avoid Disposal
                Node newNode = new Node(n.Name, new ALAR3File(n.Stream));

                aar.AlarFiles.Root.Add(newNode);
            }

            return aar;
        }
    }
}
