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
      GrantRightsForAllUsers();
    }
    
    /// <summary>
    /// Выдать права всем пользователям.
    /// </summary>
    public static void GrantRightsForAllUsers()
    {
      InitializationLogger.Debug("Init: Grant rights to all users.");
      var allUsers = Roles.AllUsers;
      
      AssignmentsTemplates.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      AssignmentsTemplates.AccessRights.Save();
    }
  }
}
