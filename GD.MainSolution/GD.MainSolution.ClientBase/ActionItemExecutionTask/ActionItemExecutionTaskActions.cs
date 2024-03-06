using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.MainSolution.ActionItemExecutionTask;

namespace GD.MainSolution.Client
{
  partial class ActionItemExecutionTaskActions
  {
    public virtual bool CanInTemplateGD(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void InTemplateGD(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      GD.ActionTemplateModule.PublicFunctions.Module.ToTemplate(_obj);
    }

    public virtual void FromeTemplateGD(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      GD.ActionTemplateModule.PublicFunctions.Module.FromTemplate(_obj, e);
    }

    public virtual bool CanFromeTemplateGD(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}