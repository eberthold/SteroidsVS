using System.Collections.Generic;

namespace Steroids.CodeStructure.Settings
{
    public class CodeStructureWidthSettings
    {
        /// <summary>
        /// The currently used <see cref="WidthMode"/>.
        /// </summary>
        public WidthMode WidthMode { get; set; }

        /// <summary>
        /// Dedicated file width informations.
        /// </summary>
        public List<FileWidthInfo> FileWidthInfos { get; set; }
    }
}
