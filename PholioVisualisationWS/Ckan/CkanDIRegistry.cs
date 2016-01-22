using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ckan.DataTransformation;
using StructureMap.Configuration.DSL;

namespace Ckan
{
    public class CkanDIRegistry : Registry
    {
        public CkanDIRegistry()
        {
            For<ICkanResourceUploader>()
                .Use<CkanResourceUploader>();

            For<IProfileUploader>()
                .Use<ProfileUploader>();

             For<IPackageIdProvider>()
                .Use<PackageIdProvider>();
        }
    }
}
