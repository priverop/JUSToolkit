namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using JUSToolkit.Formats.ALAR;

    public class Alar2ToNodes : 
        IConverter<ALAR2, NodeContainerFormat>,
        IConverter<NodeContainerFormat, ALAR2>
    {
        public NodeContainerFormat Convert(ALAR2 aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            var container = new NodeContainerFormat();

            foreach (ALAR2File f in aar.AlarFiles)
            {
                container.Root.Add(f.File);
            }

            return container;
        }

        public ALAR2 Convert(NodeContainerFormat container)
        {
            ALAR2 aar = new ALAR2();

            foreach(Node n in container.Root.Children){
                aar.AlarFiles.Add(new ALAR2File{ File = n });
            }

            return aar;
        }
    }
}
