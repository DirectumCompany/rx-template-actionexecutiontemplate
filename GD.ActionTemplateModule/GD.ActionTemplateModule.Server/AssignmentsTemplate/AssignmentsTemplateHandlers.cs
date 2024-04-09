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
      
      if (_obj.IsCompoundActionItem == true)
      {
        foreach (var itemPart in _obj.ActionItemParts)
        {
          var coAssigneesCount = !itemPart.CoAssigneesCount.HasValue ? null : itemPart.CoAssigneesCount;
          
          if (itemPart.CoAssigneesDaysOrHours.HasValue)
            coAssigneesDaysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(itemPart.CoAssigneesDaysOrHours.Value);
          
          if (itemPart.DaysOrHours.HasValue)
            daysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(itemPart.DaysOrHours.Value);
          
          var error = Functions.AssignmentsTemplate.CheckAssignmentTemplateConditions(_obj, itemPart.Supervisor, itemPart.Assignee, itemPart.Count,
                                                                                      daysOrHours, itemPart.CoAssignees,
                                                                                      coAssigneesCount, coAssigneesDaysOrHours, itemPart);
          if (!string.IsNullOrEmpty(error))
            e.AddError(error);
        }
      }
      else
      {
        var employees = _obj.CoAssignees.Any() ?
          Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(_obj.CoAssignees.Select(co => co.CoAssignee).ToList(), false) : string.Empty;
        
        if (_obj.DaysOrHours.HasValue)
          daysOrHours = _obj.Info.Properties.DaysOrHours.GetLocalizedValue(_obj.DaysOrHours.Value);
        
        if (_obj.CoAssigneesDaysOrHours.HasValue)
          coAssigneesDaysOrHours = _obj.Info.Properties.CoAssigneesDaysOrHours.GetLocalizedValue(_obj.CoAssigneesDaysOrHours.Value);
        
        var error = Functions.AssignmentsTemplate.CheckAssignmentTemplateConditions(_obj, _obj.Supervisor, _obj.Assignee, _obj.Count,
                                                                                    daysOrHours, employees, _obj.CoAssigneesCount,
                                                                                    coAssigneesDaysOrHours, null);
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