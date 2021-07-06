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
      _obj.State.Properties.Count.IsRequired = _obj.CoAssCount.HasValue;
    }

    public virtual void CoAssCountValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
        _obj.State.Properties.CoAssDaysOrHours.IsRequired = e.NewValue != null;      
    }

  }
}