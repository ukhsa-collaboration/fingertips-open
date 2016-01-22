using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileData
{
    class ProfileDataRegistry : Registry
    {
        public ProfileDataRegistry()
        {
            // Override injections and any custom parameters constructors if required
            //For<IContentReader>()
            //    .Use<IContentReader>(x => ReaderFactory.GetContentReader());
        }
    }
}
