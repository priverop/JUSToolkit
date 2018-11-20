namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using JUSToolkit.Formats;

    public class Alar2Nodes : 
        IConverter<ALAR, NodeContainerFormat>,
        IConverter<NodeContainerFormat, ALAR>
    {
        public NodeContainerFormat Convert(ALAR aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            var container = new NodeContainerFormat();

            foreach (Node f in aar.Files)
            {
                container.Root.Add(f);
            }

            return container;
        }

        public ALAR Convert(NodeContainerFormat container)
        {
            return new ALAR();
        }
    }
}
