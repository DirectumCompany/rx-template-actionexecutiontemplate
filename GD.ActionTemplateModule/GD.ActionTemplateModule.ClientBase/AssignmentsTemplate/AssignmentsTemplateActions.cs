using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule.Client
{
  internal static class AssignmentsTemplateActionItemPartsStaticActions
  {

    public static bool CanAddCompoundActionItemPart(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var assignmentTemplate = AssignmentsTemplates.As(e.Entity);
      return (assignmentTemplate.State.IsInserted || Locks.GetLockInfo(assignmentTemplate).IsLockedByMe) && assignmentTemplate != null;
    }

    public static void AddCompoundActionItemPart(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var assignmentTemplate = AssignmentsTemplates.As(e.Entity);
      Functions.AssignmentsTemplate.FillCompoundActionItemPart(assignmentTemplate, null);
    }
  }

  partial class AssignmentsTemplateActions
  {

    public virtual void ChangeCompoundMode(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.IsCompoundActionItem == true)
      {
        if (_obj.ActionItemParts.Count(ip => ip.Assignee != null) > 1 ||
            _obj.ActionItemParts.Any(ip => ip.Count != null || !string.IsNullOrEmpty(ip.ActionItemPart)))
        {
          var dialog = Dialogs.CreateTaskDialog(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChangeCompoundModeQuestion,
                                                Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChangeCompoundModeDescription,
                                                MessageType.Question);
          dialog.Buttons.AddYesNo();
          dialog.Buttons.Default = DialogButtons.No;
          var yesResult = dialog.Show() == DialogButtons.Yes;
          if (yesResult)
            _obj.IsCompoundActionItem = false;
        }
        else
          _obj.IsCompoundActionItem = false;
      }
      else
        _obj.IsCompoundActionItem = true;
    }

    public virtual bool CanChangeCompoundMode(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.State.IsInserted || Locks.GetLockInfo(_obj).IsLockedByMe;
    }

  }




  partial class AssignmentsTemplateActionItemPartsActions
  {
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