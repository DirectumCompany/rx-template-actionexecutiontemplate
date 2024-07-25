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
      
      // Задать порядковый номер для пункта поручения.
      var lastNumber = _obj.ActionItemParts.OrderBy(j => j.Number).LastOrDefault();
      if (lastNumber.Number.HasValue)
        _added.Number = lastNumber.Number + 1;
      else
        _added.Number = 1;
      
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

    public virtual void IsCompoundActionItemChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.OldValue != e.NewValue)
      {
        // Заполнить данные из составного поручения в обычное и наоборот.
        if (e.NewValue.Value)
        {
          // Составное поручение.
          _obj.ActionItemParts.Clear();
          _obj.FinalCount = _obj.Count;
          _obj.CoAssigneesCount = null;
          
          if (_obj.Assignee != null)
          {
            var newJob = _obj.ActionItemParts.AddNew();
            newJob.Assignee = _obj.Assignee;
          }
          
          foreach (var job in _obj.CoAssignees)
          {
            var newJob = _obj.ActionItemParts.AddNew();
            newJob.Assignee = job.CoAssignee;
          }
          _obj.CoAssignees.Clear();
        }
        else
        {
          // Не составное поручение.
          var actionItemPart = _obj.ActionItemParts.OrderBy(x => x.Number).FirstOrDefault();
          if (_obj.FinalCount != null)
            _obj.Count = _obj.FinalCount;
          else if (actionItemPart != null)
            _obj.Count = actionItemPart.Count;
          else
            _obj.Count = null;
          
          if (actionItemPart != null)
            _obj.Assignee = actionItemPart.Assignee;
          else
            _obj.Assignee = null;
          
          _obj.CoAssignees.Clear();
          
          foreach (var job in _obj.ActionItemParts.OrderBy(x => x.Number).Skip(1))
          {
            if (job.Assignee != null && !_obj.CoAssignees.Select(z => z.CoAssignee).Contains(job.Assignee))
              _obj.CoAssignees.AddNew().CoAssignee = job.Assignee;
          }
          
          if (string.IsNullOrEmpty(_obj.Text) && actionItemPart != null)
          {
            _obj.Text = actionItemPart.ActionItemPart;
            Functions.AssignmentsTemplate.SynchronizeActiveText(_obj);
          }

          if (actionItemPart != null && _obj.Supervisor == null)
            _obj.Supervisor = actionItemPart.Supervisor;
          
          // Чистим грид в составном, чтобы не мешать валидации.
          _obj.ActionItemParts.Clear();
        }
      }
      Functions.AssignmentsTemplate.SetRequiredProperties(_obj);
      // _obj.State.Controls.Control.Refresh();
    }

    public virtual void CoAssigneesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (!_obj.CoAssignees.Any())
      {
        _obj.CoAssigneesCount = null;
        _obj.CoAssigneesDaysOrHours = null;
      }
      Functions.AssignmentsTemplate.SetRequiredProperties(_obj);
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
      Functions.AssignmentsTemplate.SetRequiredProperties(_obj);
    }

  }
}