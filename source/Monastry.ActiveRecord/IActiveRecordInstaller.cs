using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Monastry.ActiveRecord
{
    public interface IActiveRecordInstaller
    {
        IWindsorContainer GetConfiguredContainer();
    }
}
