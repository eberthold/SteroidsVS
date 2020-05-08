using System;
using System.Collections.Generic;

namespace Steroids.CodeStructure.Settings
{
    public class CodeStructureWidthSettings
    {
        /// <summary>
        /// The currently used <see cref="WidthMode"/>.
        /// </summary>
        public WidthMode WidthMode { get; set; } = WidthMode.RestoreWithDefault;

        /// <summary>
        /// The deafult wide for code structure when opening a file.
        /// </summary>
        public double DefaultWidth { get; set; } = 250;

        /// <summary>
        /// Dedicated file width informations.
        /// </summary>
        public List<FileWidthInfo> FileWidthInfos { get; set; } = new List<FileWidthInfo>();
    }
}
