using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.PeriodicActionItemSchedule;

namespace GD.ActionTemplateModule
{
  partial class PeriodicActionItemScheduleServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if ((_obj.Type == PeriodicActionItemSchedule.Type.Day || _obj.Type == PeriodicActionItemSchedule.Type.Week) && _obj.BeginningDate >= _obj.EndDate ||
			    _obj.Type == PeriodicActionItemSchedule.Type.Month && _obj.BeginningMonth >= _obj.EndMonth ||
			    _obj.Type == PeriodicActionItemSchedule.Type.Year && _obj.BeginningYear >= _obj.EndYear)
			{
				e.AddError(_obj.Info.Properties.BeginningDate, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.EndDate);
				e.AddError(_obj.Info.Properties.EndDate, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.BeginningDate);
				e.AddError(_obj.Info.Properties.BeginningMonth, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.EndMonth);
				e.AddError(_obj.Info.Properties.EndMonth, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.BeginningMonth);
				e.AddError(_obj.Info.Properties.BeginningYear, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.EndYear);
				e.AddError(_obj.Info.Properties.EndYear, PeriodicActionItemSchedules.Resources.IncorrectValidDate, _obj.Info.Properties.BeginningYear);
			}
			
			if (_obj.Type == PeriodicActionItemSchedule.Type.Year)
			{
				_obj.BeginningDate = _obj.BeginningYear;
				_obj.EndDate = _obj.EndYear.HasValue ? _obj.EndYear.Value.EndOfYear() : _obj.EndYear;
			}
			
			if (_obj.Type == PeriodicActionItemSchedule.Type.Month)
			{
				_obj.BeginningDate = _obj.BeginningMonth;
				_obj.EndDate = _obj.EndMonth.HasValue ? _obj.EndMonth.Value.EndOfMonth() : _obj.EndMonth;
			}
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.LabelCreationsDays = _obj.Info.Properties.LabelCreationsDays.LocalizedName;
			_obj.LabelDayValue = _obj.Info.Properties.LabelDayValue.LocalizedName;
			_obj.Type = PeriodicActionItemSchedule.Type.Day;
			
			if (!_obj.State.IsCopied)
			{
				// Настройки повторений
				_obj.WeekTypeMonday = false;
				_obj.WeekTypeFriday = false;
				_obj.WeekTypeThursday = false;
				_obj.WeekTypeTuesday = false;
				_obj.WeekTypeWednesday = false;
			}
    }
  }

}