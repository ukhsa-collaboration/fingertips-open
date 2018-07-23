using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.UserAccess
{
    public static class MetadataTypesRegister
    {
        private static bool _installed = false;
        private static readonly object InstalledLock = new object();
        /// <summary>
        /// Register all metadata classes found in this assembly.
        /// Registration will only be done once.        
        /// </summary>
        public static void InstallForAssembly()
        {
            if (_installed)
            {
                return;
            }
            lock (InstalledLock)
            {
                if (_installed)
                {
                    return;
                }
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    foreach (MetadataTypeAttribute attrib in type.GetCustomAttributes(typeof(MetadataTypeAttribute), true))
                    {
                        TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(type, attrib.MetadataClassType), type);
                    }
                }
                _installed = true;
            }
        }
    }
}
