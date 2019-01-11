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
        public NodeContainerFormat Convert(ALAR3 aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            var container = new NodeContainerFormat();

            foreach (ALAR3File f in aar.AlarFiles)
            {
                container.Root.Add(f.File);
            }

            return container;
        }

        public ALAR3 Convert(NodeContainerFormat container)
        {
            ALAR3 aar = new ALAR3();

            foreach(Node n in container.Root.Children){

                // New node to avoid Disposal
                Node newNode = new Node(n.Name, new BinaryFormat(n.Stream));

                aar.AlarFiles.Add(new ALAR3File{ File = newNode });
            }

            return aar;
        }
    }
}
