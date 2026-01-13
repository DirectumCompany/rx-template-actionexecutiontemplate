using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule
{

  partial class AssignmentsTemplateFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      var curEmployee = Sungero.Company.Employees.Current;
      if (curEmployee != null)
      {
        var newQuery = query.ToList().Where(x => x.AccessRights.Current.Any(a => a.Recipient.Equals(curEmployee) ||
                                                                            (Groups.Is(a.Recipient) && curEmployee.IncludedIn(Groups.As(a.Recipient)))));
        query = query.Where(x => x.AssignedBy.Equals(curEmployee) || newQuery.Contains(x));
      }
      
      return query;
    }
  }

  partial class AssignmentsTemplateServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var daysOrHours = string.Empty;
      var coAssigneesDaysOrHours = string.Empty;
      var error = string.Empty;
      
      if (_obj.IsCompoundActionItem == true)
      {
        foreach (var itemPart in _obj.ActionItemParts)
        {
          coAssigneesDaysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(itemPart.CoAssigneesDaysOrHours);
          daysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(itemPart.DaysOrHours);
          
          error = Functions.AssignmentsTemplate.CheckConditions(_obj, itemPart.Supervisor ?? _obj.Supervisor, itemPart.Count,
                                                                daysOrHours, !string.IsNullOrEmpty(itemPart.CoAssignees),
                                                                itemPart.CoAssigneesCount, coAssigneesDaysOrHours, itemPart);
          if (!string.IsNullOrEmpty(error))
            e.AddError(error);
        }
      }
      else
      {
        daysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(_obj.DaysOrHours);
        coAssigneesDaysOrHours = _obj.Info.Properties.CoAssigneesDaysOrHours.GetLocalizedValue(_obj.CoAssigneesDaysOrHours);
        
        error = Functions.AssignmentsTemplate.CheckConditions(_obj, _obj.Supervisor, _obj.Count,
                                                              daysOrHours, _obj.CoAssignees.Any(), _obj.CoAssigneesCount,
                                                              coAssigneesDaysOrHours, null);
        if (!string.IsNullOrEmpty(error))
          e.AddError(error);
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (!_obj.State.IsCopied)
      {
        _obj.IsUnderControl = false;
        _obj.HasIndefiniteDeadline = false;
        _obj.IsCompoundActionItem = false;
        _obj.Importance = AssignmentsTemplate.Importance.Normal;
        _obj.IsHighImportance = false;
      }
      _obj.DaysOrHours = AssignmentsTemplate.DaysOrHours.Days;
      _obj.CoAssigneesDaysOrHours = AssignmentsTemplate.CoAssigneesDaysOrHours.Days;
      _obj.FinalDaysOrHours = AssignmentsTemplate.FinalDaysOrHours.Days;
    }
  }
}