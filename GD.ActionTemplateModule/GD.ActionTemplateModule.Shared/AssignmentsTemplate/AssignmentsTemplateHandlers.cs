using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule
{
  partial class AssignmentsTemplateSharedHandlers
  {

    public virtual void CoAssigneesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (!_obj.CoAssignees.Any())
      {
        _obj.CoAssigneesCount = null;
        _obj.CoAssigneesDaysOrHours = null;
      }
    }

    public virtual void FinalCountChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      _obj.State.Properties.FinalDaysOrHours.IsRequired = e.NewValue != null;
    }

    public virtual void CoAssigneesCountChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      _obj.State.Properties.CoAssigneesDaysOrHours.IsRequired = e.NewValue != null;
    }

    public virtual void CountChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      _obj.State.Properties.DaysOrHours.IsRequired = e.NewValue != null;
    }

    public virtual void IsUnderControlChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == false)
      {
        _obj.Supervisor = null;
        foreach (var actiomItemPart in _obj.ActionItemParts)
          actiomItemPart.Supervisor = null;
      }
    }

    public virtual void HasIndefiniteDeadlineChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.Count = null;
        _obj.DaysOrHours = null;
        _obj.CoAssigneesCount = null;
        _obj.CoAssigneesDaysOrHours = null;
        _obj.FinalCount = null;
        _obj.FinalDaysOrHours = null;
        foreach (var actionItemPart in _obj.ActionItemParts)
        {
          actionItemPart.Count = null;
          actionItemPart.DaysOrHours = null;
          actionItemPart.CoAssigneesCount = null;
          actionItemPart.CoAssigneesDaysOrHours = null;
        }
      }
    }

  }
}