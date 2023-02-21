using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.PeriodicActionItemSchedule;

namespace GD.ActionTemplateModule.Client
{
  partial class PeriodicActionItemScheduleActions
  {
    public virtual void ShowTasksList(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanShowTasksList(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}