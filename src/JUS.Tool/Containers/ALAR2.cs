using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Alar2 Container Format.
    /// </summary>
    public class Alar2 : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2" /> class with an empty list of <see cref="Alar2File" />.
        /// </summary>
        public Alar2()
        {
            AlarFiles = new List<Alar2File>();
        }

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        public char[] Header { get; set; }

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public byte Unk { get; set; }

        /// <summary>
        /// Gets or sets the number of files for the container.
        /// </summary>
        public ushort Num_files { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the files.
        /// </summary>
        public byte[] IDs { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="Alar2File" />.
        /// </summary>
        public List<Alar2File> AlarFiles { get; set; }

        /// <summary>
        /// Replaces current <see cref="AlarFiles" /> with another list of <see cref="AlarFiles" />.
        /// </summary>
        /// <param name="newAlarContainer">Alar2 NodeContainerFormat.</param>
        public void InsertModification(Alar2 newAlarContainer)
        {
            for (int i = 0; i < AlarFiles.Count; i++) {
                foreach (Alar2File newAlarNode in newAlarContainer.AlarFiles) {
                    if (newAlarNode.File.Name == AlarFiles[i].File.Name) {
                        AlarFiles[i] = newAlarNode;
                    }
                }
            }
        }
    }
}
