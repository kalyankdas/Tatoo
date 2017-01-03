using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tatoo.Azure.BlobStorage
{
    public static class Configuration
    {
        /// <summary>
        /// Azure Storage Connection String. UseDevelopmentStorage=true points to the storage emulator.
        /// </summary>
        public const string StorageConnectionString =
            "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1;";


    }


}
