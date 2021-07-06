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
      if (_obj.CoAssCount != null)
      {
        if (_obj.CoAssDaysOrHours == AssignmentsTemplate.CoAssDaysOrHours.Days && _obj.DaysOrHours == AssignmentsTemplate.DaysOrHours.Hours ||
            _obj.CoAssCount > _obj.Count && _obj.Info.Properties.CoAssDaysOrHours.GetLocalizedValue(_obj.CoAssDaysOrHours.Value) == 
                                            _obj.Info.Properties.DaysOrHours.GetLocalizedValue(_obj.DaysOrHours.Value))
          e.AddError(GD.ActionTemplateModule.AssignmentsTemplates.Resources.CoAssigneesDeadlineError);        
      }
      if (e.IsValid && Sungero.Company.Employees.Current != null)
        _obj.AccessRights.Grant(Sungero.Company.Employees.Current.Department, DefaultAccessRightsTypes.Read);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsUnderControl = false;
      _obj.DaysOrHours = AssignmentsTemplate.DaysOrHours.Days;
      _obj.CoAssDaysOrHours = AssignmentsTemplate.CoAssDaysOrHours.Days;
    }
  }

}