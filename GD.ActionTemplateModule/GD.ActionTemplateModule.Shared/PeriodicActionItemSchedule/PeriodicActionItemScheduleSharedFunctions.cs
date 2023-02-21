using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.PeriodicActionItemSchedule;

namespace GD.ActionTemplateModule.Shared
{
  partial class PeriodicActionItemScheduleFunctions
  {

    public void SetStateProperties()
    {
      #region Ежегодно.
      
      var isYear = _obj.Type == GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Year;
      
      _obj.State.Properties.YearTypeDay.IsVisible = isYear;
      _obj.State.Properties.YearTypeDayOfWeek.IsVisible = isYear;
      _obj.State.Properties.YearTypeDayOfWeekNumber.IsVisible = isYear;
      _obj.State.Properties.YearTypeDayValue.IsVisible = isYear;
      _obj.State.Properties.YearTypeMonth.IsVisible = isYear;
      _obj.State.Properties.BeginningYear.IsVisible = isYear;
      _obj.State.Properties.EndYear.IsVisible = isYear;
      
      var isDateYearType = isYear && _obj.YearTypeDay == GD.ActionTemplateModule.PeriodicActionItemSchedule.YearTypeDay.Date;
      _obj.State.Properties.YearTypeDayValue.IsVisible = isDateYearType;
      
      var isDayOfWeekYearType = isYear && _obj.YearTypeDay == GD.ActionTemplateModule.PeriodicActionItemSchedule.YearTypeDay.DayOfWeek;
      _obj.State.Properties.YearTypeDayOfWeek.IsVisible = isDayOfWeekYearType;
      _obj.State.Properties.YearTypeDayOfWeekNumber.IsVisible = isDayOfWeekYearType;
      
      #endregion
      
      #region Ежемесячно.
      
      var isMonth = _obj.Type == GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Month;
      
      _obj.State.Properties.MonthTypeDay.IsVisible = isMonth;
      _obj.State.Properties.MonthTypeDayOfWeek.IsVisible = isMonth;
      _obj.State.Properties.MonthTypeDayOfWeekNumber.IsVisible = isMonth;
      _obj.State.Properties.MonthTypeDayValue.IsVisible = isMonth;
      _obj.State.Properties.BeginningMonth.IsVisible = isMonth;
      _obj.State.Properties.EndMonth.IsVisible = isMonth;
      
      var isDateMonthType = isMonth && _obj.MonthTypeDay == GD.ActionTemplateModule.PeriodicActionItemSchedule.MonthTypeDay.Date;
      _obj.State.Properties.MonthTypeDayValue.IsVisible = isDateMonthType;
      _obj.State.Properties.LabelDayValue.IsVisible = isDateMonthType;
      
      var isDayOfWeekMonthType = isMonth && _obj.MonthTypeDay == GD.ActionTemplateModule.PeriodicActionItemSchedule.MonthTypeDay.DayOfWeek;
      _obj.State.Properties.MonthTypeDayOfWeek.IsVisible = isDayOfWeekMonthType;
      _obj.State.Properties.MonthTypeDayOfWeekNumber.IsVisible = isDayOfWeekMonthType;
      
      #endregion
      
      #region Еженедельно.
      
      var isWeek = _obj.Type == GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Week;

      _obj.State.Properties.WeekTypeFriday.IsVisible = isWeek;
      _obj.State.Properties.WeekTypeMonday.IsVisible = isWeek;
      _obj.State.Properties.WeekTypeThursday.IsVisible = isWeek;
      _obj.State.Properties.WeekTypeTuesday.IsVisible = isWeek;
      _obj.State.Properties.WeekTypeWednesday.IsVisible = isWeek;
      
      #endregion
      
      #region Ежедневно.
      
      var isDay = _obj.Type == GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Day;

      #endregion
      
      _obj.State.Properties.BeginningDate.IsVisible = isWeek || isDay;
      _obj.State.Properties.EndDate.IsVisible = isWeek || isDay;
      
      _obj.State.Properties.CreationDays.IsEnabled = _obj.Type != GD.ActionTemplateModule.PeriodicActionItemSchedule.Type.Day || !(!_obj.RepeatValue.HasValue || _obj.RepeatValue.Value == 1);
      
    }
  }
}