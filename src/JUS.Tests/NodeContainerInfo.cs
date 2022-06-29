// Copyright (c) 2021 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JUSToolkit.Tests
{
    /// <summary>
    /// Containers yaml file information.
    /// </summary>
    public class NodeContainerInfo
    {
        public string Name { get; set; }

        public string FormatType { get; set; }

        public BinaryInfo Stream { get; set; }

        public Dictionary<string, object> Tags { get; set; }

        public bool CheckChildren { get; set; }

        public Collection<NodeContainerInfo> Children { get; set; }

        /// <summary>
        /// Gets the info from the Yaml file into the NodeContainerInfo object.
        /// </summary>
        /// <param name="path">Path to the Yaml file.</param>
        /// <returns>NodeContainerInfo.</returns>
        public static NodeContainerInfo FromYaml(string path)
        {
            string yaml = File.ReadAllText(path);
            return new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<NodeContainerInfo>(yaml);
        }
    }
}
