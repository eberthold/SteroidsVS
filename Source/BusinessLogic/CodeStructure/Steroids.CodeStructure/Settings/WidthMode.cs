using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steroids.CodeStructure.Settings
{
    public enum WidthMode
    {
        /// <summary>
        /// Width uses a default when file is opened.
        /// </summary>
        RestoreWithDefault,

        /// <summary>
        /// The last used width is stored per file.
        /// </summary>
        StorePerFile,

        /// <summary>
        /// All widths are synced
        /// </summary>
        SyncGlobally
    }
}
