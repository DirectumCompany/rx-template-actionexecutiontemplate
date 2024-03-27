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
        if (_obj.ActionItemParts.Any(p => p.CoAssigneesDaysOrHours == null))
          e.AddError(AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours);
        
        if (_obj.ActionItemParts.Any(p => p.CoAssigneesCount == null && !string.IsNullOrEmpty(p.CoAssignees)))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyActionItemPartCoAssigneesDeadline);
        
        if (_obj.ActionItemParts.Any(p => p.CoAssigneesCount != null && string.IsNullOrEmpty(p.CoAssignees)))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyActionitemPartCoAssigneesdeadline);
        
        if (_obj.ActionItemParts.Any(p => p.Count == null || p.DaysOrHours == null))
          e.AddError(AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours);
      }
      
      // Проверить корректность сроков соисполнителей для простого поручения.
      if (_obj.CoAssigneesCount != null)
      {
        if (_obj.CoAssigneesDaysOrHours == AssignmentsTemplate.CoAssigneesDaysOrHours.Days && _obj.DaysOrHours == AssignmentsTemplate.DaysOrHours.Hours ||
            _obj.CoAssigneesCount > _obj.Count && _obj.Info.Properties.CoAssigneesDaysOrHours.GetLocalizedValue(_obj.CoAssigneesDaysOrHours.Value) ==
            _obj.Info.Properties.DaysOrHours.GetLocalizedValue(_obj.DaysOrHours.Value))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.CoAssigneesDeadlineError);
      }
      
      // Проверить корректность сроков соисполнителей относительно основного исполнителя в таблице.
      if (_obj.ActionItemParts.Where(p => p.Count.HasValue && p.CoAssigneesCount.HasValue)
          .Any(p => p.CoAssigneesDaysOrHours == AssignmentsTemplateActionItemParts.CoAssigneesDaysOrHours.Days &&
               p.DaysOrHours == AssignmentsTemplateActionItemParts.DaysOrHours.Hours ||
               p.CoAssigneesCount > p.Count &&
               _obj.Info.Properties.ActionItemParts.Properties.CoAssigneesDaysOrHours.GetLocalizedValue(p.CoAssigneesDaysOrHours.Value) ==
               _obj.Info.Properties.ActionItemParts.Properties.DaysOrHours.GetLocalizedValue(p.DaysOrHours.Value)))
      {
        e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.CoAssigneesDeadlineError);
      }
      
      if (_obj.IsCompoundActionItem == false)
      {
        if (_obj.ActionItemParts.Any(p => p.Count == null && p.CoAssigneesCount != null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyActionItemPartDeadline);
        
        if (_obj.ActionItemParts.Any(p => p.Count != null && p.DaysOrHours == null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours);
        
        if (_obj.ActionItemParts.Any(p => p.CoAssigneesCount != null && p.CoAssigneesDaysOrHours == null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours);
        
        if (_obj.ActionItemParts.Any(p => p.Count != null && p.DaysOrHours == null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours);
        
        if (_obj.ActionItemParts.Any(p => p.Count != null && p.CoAssigneesDaysOrHours == null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours);
        
        if (e.IsValid && Sungero.Company.Employees.Current != null)
          _obj.AccessRights.Grant(Sungero.Company.Employees.Current.Department, DefaultAccessRightsTypes.Read);
        
        if (_obj.ActionItemParts.Any(p => p.Count != null && p.CoAssigneesCount == null))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyActionItemPartCoAssigneesDeadline);
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