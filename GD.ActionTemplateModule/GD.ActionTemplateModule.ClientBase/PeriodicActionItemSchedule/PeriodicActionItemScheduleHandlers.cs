using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.PeriodicActionItemSchedule;

namespace GD.ActionTemplateModule
{
  partial class PeriodicActionItemScheduleClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.PeriodicActionItemSchedule.SetStateProperties(_obj);

      if (_obj.Type == GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Month && _obj.MonthTypeDayValue.HasValue && _obj.MonthTypeDayValue.Value > 28 && _obj.MonthTypeDayValue.Value <= 31)
        e.AddInformation(PeriodicActionItemSchedules.Resources.IncorrectDaysOfMonthFormat(_obj.MonthTypeDayValue.Value));
    }

    public virtual void MonthTypeDayValueValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && (e.NewValue <= 0 || e.NewValue > 31))
        e.AddError(PeriodicActionItemSchedules.Resources.IncorrectDayValueFormat(31));
    }

    public virtual void YearTypeDayValueValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
       if (e.NewValue.HasValue)
      {
        if ((_obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.January ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.March ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.May ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.July ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.August ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.October ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.December) &&
            (e.NewValue <= 0 || e.NewValue > 31))
          e.AddError(PeriodicActionItemSchedules.Resources.IncorrectDayValueFormat(31));
        
        if ((_obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.April ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.June ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.September ||
             _obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.November) &&
            (e.NewValue <= 0 || e.NewValue > 30))
          e.AddError(PeriodicActionItemSchedules.Resources.IncorrectDayValueFormat(30));
        
      	// Игнорируем високосные годы
        if (_obj.YearTypeMonth == PeriodicActionItemSchedule.YearTypeMonth.February && (e.NewValue <= 0 || e.NewValue > 28))
          e.AddError(PeriodicActionItemSchedules.Resources.IncorrectDayValueFormat(28));
      }
    }

    public virtual void CreationDaysValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue < 0)
        e.AddError(PeriodicActionItemSchedules.Resources.IncorrectValue);
    }

    public virtual void RepeatValueValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue <= 0)
        e.AddError(PeriodicActionItemSchedules.Resources.IncorrectValue);
    }

  }
}