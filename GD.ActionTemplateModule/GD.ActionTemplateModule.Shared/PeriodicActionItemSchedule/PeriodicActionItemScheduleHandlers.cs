using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.PeriodicActionItemSchedule;

namespace GD.ActionTemplateModule
{
  partial class PeriodicActionItemScheduleSharedHandlers
  {

    public virtual void BeginningMonthChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.BeginningMonth = Calendar.Today;
    }

    public virtual void BeginningYearChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.BeginningYear = Calendar.Today;
    }

    public virtual void MonthTypeDayChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.MonthTypeDay = PeriodicActionItemSchedule.MonthTypeDay.Date;
      }
      else
      {
        if (e.NewValue != e.OldValue)
        {
          if (e.NewValue == PeriodicActionItemSchedule.MonthTypeDay.DayOfWeek)
          {
            _obj.MonthTypeDayOfWeek = PeriodicActionItemSchedule.MonthTypeDayOfWeek.Monday;
            _obj.MonthTypeDayOfWeekNumber = PeriodicActionItemSchedule.MonthTypeDayOfWeekNumber.First;
          }
          else
          {
            _obj.MonthTypeDayOfWeek = null;
            _obj.MonthTypeDayOfWeekNumber = null;
          }
          
          if (e.NewValue == PeriodicActionItemSchedule.MonthTypeDay.Date)
            _obj.MonthTypeDayValue = 1;
          else
            _obj.MonthTypeDayValue = null;
          
          Functions.PeriodicActionItemSchedule.SetStateProperties(_obj);
        }
      }
    }

    public virtual void YearTypeDayOfWeekNumberChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.YearTypeDayOfWeekNumber = PeriodicActionItemSchedule.YearTypeDayOfWeekNumber.First;
    }

    public virtual void YearTypeDayOfWeekChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.YearTypeDayOfWeek = _obj.YearTypeDayOfWeek = PeriodicActionItemSchedule.YearTypeDayOfWeek.Monday;
    }

    public virtual void YearTypeDayValueChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.YearTypeDayValue = 1;
    }

    public virtual void YearTypeDayChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.YearTypeDay = PeriodicActionItemSchedule.YearTypeDay.Date;
      }
      else
      {
        if (e.NewValue != e.OldValue)
        {
          if (e.NewValue == PeriodicActionItemSchedule.YearTypeDay.DayOfWeek)
          {
            _obj.YearTypeDayOfWeek = PeriodicActionItemSchedule.YearTypeDayOfWeek.Monday;
            _obj.YearTypeDayOfWeekNumber = PeriodicActionItemSchedule.YearTypeDayOfWeekNumber.First;
          }
          else
          {
            _obj.YearTypeDayOfWeek = null;
            _obj.YearTypeDayOfWeekNumber = null;
          }
          
          if (e.NewValue == PeriodicActionItemSchedule.YearTypeDay.Date)
            _obj.YearTypeDayValue = 1;
          else
            _obj.YearTypeDayValue = null;
          
          Functions.PeriodicActionItemSchedule.SetStateProperties(_obj);
        }
      }
    }

    public virtual void YearTypeMonthChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
       if (e.NewValue == null)
        _obj.YearTypeMonth = PeriodicActionItemSchedule.YearTypeMonth.January;
    }

    public virtual void BeginningDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.BeginningDate = Calendar.Today;
    }

    public virtual void RepeatValueChanged(Sungero.Domain.Shared.IntegerPropertyChangedEventArgs e)
    {
      if ((e.NewValue == null || e.NewValue == 1) && _obj.Type.HasValue && _obj.Type.Value == PeriodicActionItemSchedule.Type.Day)
        _obj.CreationDays = 0;
      
      Functions.PeriodicActionItemSchedule.SetStateProperties(_obj);
    }

    public virtual void TypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        _obj.RepeatValue = null;
        _obj.MonthTypeDay = null;
        _obj.WeekTypeFriday = false;
        _obj.WeekTypeMonday = false;
        _obj.WeekTypeThursday = false;
        _obj.WeekTypeTuesday = false;
        _obj.WeekTypeWednesday = false;
        
        if (e.NewValue == PeriodicActionItemSchedule.Type.Year)
        {
          _obj.LabelType = PeriodicActionItemSchedules.Resources.LabelYear;
          _obj.BeginningYear = Calendar.Today.BeginningOfYear();
          _obj.YearTypeMonth = PeriodicActionItemSchedule.YearTypeMonth.January;
          _obj.YearTypeDay = PeriodicActionItemSchedule.YearTypeDay.Date;
          _obj.YearTypeDayValue = 1;
        }
        else
        {
          _obj.BeginningYear = null;
          _obj.YearTypeMonth = null;
          _obj.YearTypeDay = null;
          _obj.YearTypeDayValue = null;
        }
        
        if (e.NewValue == PeriodicActionItemSchedule.Type.Month)
        {
          _obj.LabelType = PeriodicActionItemSchedules.Resources.LabelMonth;
          _obj.BeginningMonth = Calendar.Today.BeginningOfYear();
          _obj.MonthTypeDay = PeriodicActionItemSchedule.MonthTypeDay.Date;
          _obj.MonthTypeDayValue = 1;
        }
        else
        {
          _obj.BeginningMonth = null;
          _obj.MonthTypeDay = null;
          _obj.MonthTypeDayValue = null;
        }

        if (e.NewValue == PeriodicActionItemSchedule.Type.Day)
          _obj.LabelType = PeriodicActionItemSchedules.Resources.LabelDay;
        
        if (e.NewValue == PeriodicActionItemSchedule.Type.Week)
          _obj.LabelType = PeriodicActionItemSchedules.Resources.LabelWeek;
        
        if (e.NewValue == PeriodicActionItemSchedule.Type.Week || e.NewValue == PeriodicActionItemSchedule.Type.Day)
        {
          _obj.BeginningDate = Calendar.Today;
        }
        else
        {
          _obj.BeginningDate = null;
        }
        
        Functions.PeriodicActionItemSchedule.SetStateProperties(_obj);
      }
    }

  }
}