using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule
{
  partial class AssignmentsTemplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var properties = _obj.State.Properties;
      var isComponentResolution = _obj.IsCompoundActionItem ?? false;
      var hasNotIndefiniteDeadline = _obj.HasIndefiniteDeadline != true;
      var isUnderControl = _obj.IsUnderControl == true;
      var anyCoAssignees = _obj.CoAssignees.Any();
      
      properties.Count.IsEnabled = hasNotIndefiniteDeadline;
      properties.DaysOrHours.IsEnabled = hasNotIndefiniteDeadline;
      properties.CoAssigneesCount.IsEnabled = hasNotIndefiniteDeadline && anyCoAssignees;
      properties.CoAssigneesDaysOrHours.IsEnabled = hasNotIndefiniteDeadline && anyCoAssignees;
      
      properties.ActionItemParts.IsVisible = isComponentResolution;
      properties.FinalCount.IsVisible = isComponentResolution;
      properties.FinalDaysOrHours.IsVisible = isComponentResolution;
      
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
      properties.ActionItemParts.Properties.CoAssigneesCount.IsEnabled = hasNotIndefiniteDeadline;
      properties.ActionItemParts.Properties.CoAssigneesDaysOrHours.IsEnabled = hasNotIndefiniteDeadline;
    }

  }
}