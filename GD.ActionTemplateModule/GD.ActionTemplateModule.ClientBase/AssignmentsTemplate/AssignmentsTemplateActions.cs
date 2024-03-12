using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule.Client
{


  partial class AssignmentsTemplateActionItemPartsActions
  {

    public virtual bool CanAddCompoundActionItemPart(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var assignmentTemplate = AssignmentsTemplates.As(e.Entity);
      return (assignmentTemplate.State.IsInserted || Locks.GetLockInfo(assignmentTemplate).IsLockedByMe) && assignmentTemplate != null;
    }

    public virtual void AddCompoundActionItemPart(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var assignmentTemplate = AssignmentsTemplates.As(e.Entity);
      Functions.AssignmentsTemplate.FillCompoundActionItemPart(assignmentTemplate, null);
    }

    public virtual bool CanChangeCompoundActionItemPart(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return (_obj.AssignmentsTemplate.State.IsInserted || Locks.GetLockInfo(_obj.AssignmentsTemplate).IsLockedByMe) &&
        Functions.AssignmentsTemplate.CanChangeActionItem(_obj.AssignmentsTemplate) ||
        Functions.AssignmentsTemplate.CanChangeActionItemPart(_obj.AssignmentsTemplate, _obj);
    }

    public virtual void ChangeCompoundActionItemPart(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      Functions.AssignmentsTemplate.FillCompoundActionItemPart(_obj.AssignmentsTemplate, _obj);
    }
  }

}