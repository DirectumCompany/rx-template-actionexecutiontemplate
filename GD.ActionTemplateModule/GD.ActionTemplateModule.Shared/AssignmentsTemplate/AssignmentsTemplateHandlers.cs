using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule
{
  partial class AssignmentsTemplateActionItemPartsSharedCollectionHandlers
  {

    public virtual void ActionItemPartsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      Functions.AssignmentsTemplate.DeletePartsCoAssignees(_obj, _deleted);
    }

    public virtual void ActionItemPartsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.PartGuid = Guid.NewGuid().ToString();
      
      if (_added.State.IsCopied)
      {
        var coAssigneesCopy = Functions.AssignmentsTemplate.GetPartCoAssignees(_obj, _source.PartGuid);
        Functions.AssignmentsTemplate.AddPartsCoAssignees(_obj, _added, coAssigneesCopy);
      }
    }
  }

  partial class AssignmentsTemplateActionItemPartsSharedHandlers
  {

    public virtual void ActionItemPartsAssigneeChanged(GD.ActionTemplateModule.Shared.AssignmentsTemplateActionItemPartsAssigneeChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.Assignee.Department.BusinessUnit = null;
      
      if (e.NewValue != null && e.NewValue.Department != null)
        _obj.Assignee.Department.BusinessUnit = e.NewValue.Department.BusinessUnit;
    }
  }

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