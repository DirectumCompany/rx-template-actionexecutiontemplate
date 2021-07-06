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

    public virtual void CountChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
		_obj.State.Properties.DaysOrHours.IsRequired = e.NewValue != null;
    }

    public virtual void IsUnderControlChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
		_obj.State.Properties.Count.IsRequired = e.NewValue == true;
		_obj.State.Properties.DaysOrHours.IsRequired = e.NewValue == true;
    }

  }
}