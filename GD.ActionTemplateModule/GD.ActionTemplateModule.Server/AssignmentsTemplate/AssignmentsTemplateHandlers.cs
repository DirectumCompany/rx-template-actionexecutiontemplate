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
      if (_obj.IsCompoundActionItem == true)
      {
        foreach (var itemPart in _obj.ActionItemParts)
        {
          var coAssigneesCount = !itemPart.CoAssigneesCount.HasValue ? null : itemPart.CoAssigneesCount;
          var coAssigneesDaysOrHours = string.Empty;
          if (itemPart.CoAssigneesDaysOrHours.HasValue)
            coAssigneesDaysOrHours = itemPart.CoAssigneesDaysOrHours.Value.Value;
          var daysOrHours = string.Empty;
          if (itemPart.DaysOrHours.HasValue)
            daysOrHours = itemPart.DaysOrHours.Value.Value;
          var error = Functions.AssignmentsTemplate.CheckConditionsCompoundTemplate(_obj, itemPart.Supervisor, itemPart.Assignee, itemPart.Count,
                                                                                    daysOrHours, itemPart.CoAssignees,
                                                                                    coAssigneesCount, coAssigneesDaysOrHours, itemPart);
          if (!string.IsNullOrEmpty(error))
            e.AddError(error);
        }
      }
      else
      {
        var error = Functions.AssignmentsTemplate.CheckConditionSimpleTemplate(_obj);
        if (!string.IsNullOrEmpty(error))
          e.AddError(error);
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsUnderControl = false;
      _obj.HasIndefiniteDeadline = false;
      _obj.IsCompoundActionItem = false;
      _obj.DaysOrHours = AssignmentsTemplate.DaysOrHours.Days;
      _obj.CoAssigneesDaysOrHours = AssignmentsTemplate.CoAssigneesDaysOrHours.Days;
      _obj.FinalDaysOrHours = AssignmentsTemplate.FinalDaysOrHours.Days;
    }
  }

}