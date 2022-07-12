using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace GD.ActionTemplateModule.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      AssignmentsTemplates.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.FullAccess);
      AssignmentsTemplates.AccessRights.Save();
    }
  }
}
