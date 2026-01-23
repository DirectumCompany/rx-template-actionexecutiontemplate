using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule
{
  partial class AssignmentsTemplateActionItemPartsClientHandlers
  {

    public virtual void ActionItemPartsNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      // Проверить число на положительность.
      if (e.NewValue < 1)
        e.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.NumberIsNotPositive);
    }
  }


  partial class AssignmentsTemplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var properties = _obj.State.Properties;
      var isComponentResolution = _obj.IsCompoundActionItem ?? false;
      var hasNotIndefiniteDeadline = _obj.HasIndefiniteDeadline != true;
      var isUnderControl = _obj.IsUnderControl == true;
      var anyCoAssignees = _obj.CoAssignees.Any();
      var isComponentCoAssignees = hasNotIndefiniteDeadline && isComponentResolution && _obj.ActionItemParts.Any(p => !string.IsNullOrEmpty(p.CoAssignees));
      
      properties.Count.IsEnabled = hasNotIndefiniteDeadline;
      properties.Count.IsRequired = hasNotIndefiniteDeadline && !isComponentResolution;
      properties.DaysOrHours.IsEnabled = hasNotIndefiniteDeadline;
      properties.DaysOrHours.IsRequired = hasNotIndefiniteDeadline && !isComponentResolution;
      properties.CoAssigneesCount.IsEnabled = hasNotIndefiniteDeadline && anyCoAssignees;
      properties.CoAssigneesCount.IsRequired = hasNotIndefiniteDeadline && !isComponentResolution && anyCoAssignees;
      properties.CoAssigneesDaysOrHours.IsEnabled = hasNotIndefiniteDeadline && anyCoAssignees;
      properties.CoAssigneesDaysOrHours.IsRequired = hasNotIndefiniteDeadline && !isComponentResolution && anyCoAssignees;
      
      properties.ActionItemParts.IsVisible = isComponentResolution;
      properties.FinalCount.IsVisible = isComponentResolution;
      properties.FinalCount.IsEnabled = hasNotIndefiniteDeadline;
      properties.FinalDaysOrHours.IsVisible = isComponentResolution;
      properties.FinalDaysOrHours.IsEnabled = hasNotIndefiniteDeadline;
            
      properties.Assignee.IsVisible = !isComponentResolution;
      properties.Count.IsVisible = !isComponentResolution;
      properties.DaysOrHours.IsVisible = !isComponentResolution;
      properties.CoAssignees.IsVisible = !isComponentResolution;
      properties.CoAssigneesCount.IsVisible = !isComponentResolution;
      properties.CoAssigneesDaysOrHours.IsVisible = !isComponentResolution;
      
      properties.Assignee.IsRequired = !isComponentResolution;
      
      properties.Supervisor.IsEnabled = isUnderControl;
      properties.ActionItemParts.Properties.Supervisor.IsEnabled = isUnderControl;
      
      properties.ActionItemParts.Properties.Count.IsEnabled = hasNotIndefiniteDeadline;
      properties.ActionItemParts.Properties.DaysOrHours.IsEnabled = hasNotIndefiniteDeadline;
      properties.ActionItemParts.Properties.CoAssigneesCount.IsEnabled = isComponentCoAssignees;
      properties.ActionItemParts.Properties.CoAssigneesDaysOrHours.IsEnabled = isComponentCoAssignees;
    }

  }
}