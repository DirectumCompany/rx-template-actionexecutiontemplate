using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule.Shared
{
  partial class AssignmentsTemplateFunctions
  {
    /// <summary>
    /// Проверить условия для сохранения карточки шаблона поручения.
    /// </summary>
    /// <param name="itemPart">Строка табличной части.</param>
    /// <returns name="string">Строка с ошибкой.</param>
    public string CheckConditionSimpleTemplate()
    {
      // Проверить есть ли соисполнители в пункте поручения.
      var isCoAssigneesExist = _obj.CoAssignees.Any();
      
      // Шаблон без срока.
      var hasIndefiniteDeadline = _obj.HasIndefiniteDeadline == true;
      
      // На контроле.
      var isUnderControl = _obj.IsUnderControl == true;
      
      // Проверить шаблон стоит на контроле.
      if (_obj.IsUnderControl == true && isUnderControl)
        return AssignmentsTemplates.Resources.EmptySupervisor;
      
      var assigneeCount = Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, _obj.DaysOrHours.Value, _obj.Count);
      var coAssigneesCount = Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, _obj.DaysOrHours.Value, _obj.CoAssigneesCount);
      
      if (!hasIndefiniteDeadline && !_obj.DaysOrHours.HasValue)
        return AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours;

      if (!hasIndefiniteDeadline && !_obj.Count.HasValue)
        return GD.ActionTemplateModule.AssignmentsTemplates.Resources.EmptyTemplateDeadline;
      
      if (hasIndefiniteDeadline && !_obj.CoAssigneesDaysOrHours.HasValue)
        return AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours;

      if (assigneeCount < coAssigneesCount)
        return Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineError;
      
      return null;
    }
    
    /// <summary>
    /// Проверить условия для сохранения карточки шаблона поручения.
    /// </summary>
    /// <param name="supervisor">Контролер.</param>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="assigneeCount">Срок исполнителя.</param>
    /// <param name="assigneeDaysOrHours">Дней/Часов исполнетеля.</param>
    /// <param name="coAssignees">Соисполнитель.</param>
    /// <param name="coAssigneesCount">Срок соисполнителей.</param>
    /// <param name="coAssigneesDaysOrHours">Дней/Часов соисполнетелей.</param>
    /// <param name="itemPart">Пункт поручения.</param>
    public string CheckConditionsCompoundTemplate(Sungero.Company.IEmployee supervisor, Sungero.Company.IEmployee assignee, Nullable<int> assigneeCount,
                                                  string assigneeDaysOrHours, string coAssignees, Nullable<int> coAssigneesCount,
                                                  string coAssigneesDaysOrHours, IAssignmentsTemplateActionItemParts itemPart)
    {
      // Проверить есть ли соисполнители в пункте поручения.
      var isCoAssigneesExist = !string.IsNullOrEmpty(coAssignees);
      
      // Шаблон без срока.
      var hasIndefiniteDeadline = _obj.HasIndefiniteDeadline == true;
      
      // Проверить шаблон стоит на контроле.
      if (_obj.IsUnderControl == true && supervisor == null)
        return AssignmentsTemplates.Resources.EmptySupervisor;
      
      if (itemPart == null)
      {
        int? finalCount = !hasIndefiniteDeadline && _obj.FinalDaysOrHours.HasValue ?
          Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, _obj.FinalDaysOrHours.Value, _obj.FinalCount) : 1;
        
        int? assigneeDeadline = !hasIndefiniteDeadline && assigneeCount.HasValue ?
          Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, assigneeDaysOrHours, assigneeCount.Value) : 1;
        
        // Проверка сроков исполнителя.
        if (!hasIndefiniteDeadline)
        {
          if (assigneeCount.HasValue)
          {
            // Проверить не превышают ли срок исполнителя конечного срока.
            if (finalCount < assigneeDeadline)
              return AssignmentsTemplates.Resources.AssigneeFinalDaysOrHours;
          }
          else
            return AssignmentsTemplates.Resources.EmptyActionItemPartDeadline;
          
          // Проверка на заполненость Дней/Часов у исполнителей.
          if (!hasIndefiniteDeadline && string.IsNullOrEmpty(assigneeDaysOrHours))
            return AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours;
        }
        
        // Проверка сроков соисполнителей.
        if (!hasIndefiniteDeadline)
        {
          if (isCoAssigneesExist)
          {
            // Проверка на заполненость Дней/Часов у соисполнителей.
            if (string.IsNullOrEmpty(coAssigneesDaysOrHours))
              return AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours;
            
            if (coAssigneesCount.HasValue)
            {
              var coAssigneesDeadline = Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, coAssigneesDaysOrHours, coAssigneesCount.Value);
              
              if (coAssigneesDeadline > assigneeDeadline)
                return AssignmentsTemplates.Resources.CoAssigneesDeadlineError;
              if (coAssigneesDeadline > finalCount)
                return AssignmentsTemplates.Resources.CoAssigneeFinalDaysOrHours;
            }
            else
              return AssignmentsTemplates.Resources.EmptyActionItemPartCoAssigneesDeadline;
          }
        }
      }
      else
      {
        int? finalCount = !hasIndefiniteDeadline && _obj.FinalDaysOrHours.HasValue ?
          Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, _obj.FinalDaysOrHours.Value, _obj.FinalCount) : 1;
        
        int? assigneeDeadline = !hasIndefiniteDeadline && assigneeCount.HasValue ?
          Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, assigneeDaysOrHours, assigneeCount.Value) : 1;
        
        // Проверка сроков исполнителей.
        if (!hasIndefiniteDeadline)
        {
          if (itemPart.Count.HasValue)
          {
            // Проверить не превышают ли срок исполнителя текущей даты.
            if (assigneeDaysOrHours == AssignmentsTemplates.Info.Properties.Status.GetLocalizedValue(DaysOrHours.Days) && assigneeDeadline.Value <= 0)
              return Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday;
            
            // Проверить не превышают ли срок исполнителя конечного срока.
            if (finalCount < assigneeDeadline)
              return AssignmentsTemplates.Resources.AssigneeFinalDaysOrHours;
            
            // Проверка на заполненость Дней/Часов у исполнителей.
            if (!itemPart.DaysOrHours.HasValue)
              return AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours;
          }
          else
            return AssignmentsTemplates.Resources.EmptyActionItemPartDeadline;
        }
        
        // Проверка сроков соисполнителей.
        if (!hasIndefiniteDeadline)
        {
          if (isCoAssigneesExist)
          {
            // Проверить не превышают ли срок соисполнителя текущей даты.
            if (coAssigneesDaysOrHours == AssignmentsTemplates.Info.Properties.Status.GetLocalizedValue(DaysOrHours.Days) && assigneeDeadline.Value <= 0)
              return Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday;
            
            // Проверка на заполненость Дней/Часов у соисполнителей.
            if (!itemPart.CoAssigneesDaysOrHours.HasValue)
              return AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours;
            
            if (itemPart.Count.HasValue)
            {
              var coAssigneesDeadline = Functions.AssignmentsTemplate.ConvertDaysToHours(_obj, coAssigneesDaysOrHours, coAssigneesCount);
              
              if (coAssigneesDeadline > assigneeDeadline)
                return AssignmentsTemplates.Resources.CoAssigneesDeadlineError;
              if (coAssigneesDeadline > finalCount)
                return AssignmentsTemplates.Resources.CoAssigneesDeadlineError;
            }
            else
              return AssignmentsTemplates.Resources.EmptyActionItemPartCoAssigneesDeadline;
          }
        }
      }
      return null;
    }
    
    /// <summary>
    /// Преобразовать дни в часы.
    /// </summary>
    /// <param name="daysOrHours">Тип времени</param>
    /// <param name="count">Количество дней.</param>
    public int? ConvertDaysToHours(Enumeration daysOrHours, int? count)
    {
      if (!count.HasValue)
        count = 1;
      if (daysOrHours.Value == DaysOrHours.Days.Value)
        count *= PublicConstants.AssignmentsTemplate.DayHours;
      
      return count;
    }
    
    /// <summary>
    /// Преобразовать дни в часы.
    /// </summary>
    /// <param name="daysOrHours">Локализованнаое значение свойства Дни/Часы.</param>
    /// <param name="count">Количество дней.</param>
    public int? ConvertDaysToHours(string daysOrHours, int? count)
    {
      if (!count.HasValue)
        count = 1;
      if (daysOrHours == _obj.Info.Properties.DaysOrHours.GetLocalizedValue(DaysOrHours.Days))
        count *= PublicConstants.AssignmentsTemplate.DayHours;
      
      return count;
    }

    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public virtual void SetRequiredProperties()
    {
      var isComponentResolution = _obj.IsCompoundActionItem ?? false;
      
      _obj.State.Properties.Count.IsRequired = (_obj.Info.Properties.Count.IsRequired || !isComponentResolution)
        && _obj.HasIndefiniteDeadline != true;
      _obj.State.Properties.Assignee.IsRequired = _obj.Info.Properties.Assignee.IsRequired || !isComponentResolution;
      _obj.State.Properties.CoAssigneesCount.IsRequired = _obj.CoAssignees.Any() && !isComponentResolution
        && _obj.HasIndefiniteDeadline != true;
      
      // Проверить заполненность контролера, если поручение на контроле.
      _obj.State.Properties.Supervisor.IsRequired = (_obj.Info.Properties.Supervisor.IsRequired || _obj.IsUnderControl == true) && !isComponentResolution;
    }
    
    /// <summary>
    /// Синхронизировать первые 1000 символов текста поручения в прикладное поле.
    /// </summary>
    /// <remarks>Нужно для корректного отображения поручения в списках и папках.</remarks>
    public virtual void SynchronizeActiveText()
    {
      var actionItemPropertyMaxLength = _obj.Text.Length;
      var cutActiveText = _obj.Text != null && _obj.Text.Length > actionItemPropertyMaxLength
        ? _obj.Text.Substring(0, actionItemPropertyMaxLength)
        : _obj.Text;
      
      if (_obj.Text != cutActiveText)
        _obj.Text = cutActiveText;
    }
    
    /// <summary>
    /// Добавить пункт шаблона поручения.
    /// </summary>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="deadline">Срок исполнителя.</param>
    /// <param name="deadlineDaysOrHourse">Дни/Часы исполнителя.</param>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    /// <param name="coAssigneesDeadline">Срок соисполнителей.</param>
    /// <param name="coAssigneesDeadlineDaysOrHourse">Днич/Часы соисполнителей.</param>
    /// <param name="supervisor">Контролер.</param>
    [Public]
    public void AddActionItemPart(Sungero.Company.IEmployee assignee, int? deadline, string deadlineDaysOrHourse, string actionItemPart, List<Sungero.Company.IEmployee> coAssignees, int? coAssigneesDeadline, string coAssigneesDeadlineDaysOrHourse, Sungero.Company.IEmployee supervisor)
    {
      var actionItem = _obj.ActionItemParts.AddNew();
      actionItem.ActionItemPart = actionItemPart;
      actionItem.Assignee = assignee;
      actionItem.Count = deadline.Value;
      actionItem.DaysOrHours = deadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ? DaysOrHours.Days :
        DaysOrHours.Hours;
      actionItem.CoAssigneesCount = coAssigneesDeadline;
      actionItem.CoAssigneesDaysOrHours = coAssigneesDeadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ?
        DaysOrHours.Days : DaysOrHours.Hours;
      actionItem.Supervisor = supervisor;
      AddPartsCoAssignees(actionItem, coAssignees);
    }

    /// <summary>
    /// Удалить всех соисполнителей для пункта поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    [Public]
    public void DeletePartsCoAssignees(Sungero.RecordManagement.IActionItemExecutionTaskActionItemParts actionItemPart)
    {
      var partsCoAssignees = _obj.PartsCoAssignees.Where(p => p.PartGuid == actionItemPart.PartGuid).ToList();
      
      foreach (var partCoAssignees in partsCoAssignees)
      {
        _obj.PartsCoAssignees.Remove(partCoAssignees);
      }
      
      actionItemPart.CoAssignees = null;
    }

    /// <summary>
    /// Добавить соисполнителей для пункта поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    [Public]
    public void AddPartsCoAssignees(Sungero.RecordManagement.IActionItemExecutionTaskActionItemParts actionItemPart, List<Sungero.Company.IEmployee> coAssignees)
    {
      foreach (var coAssignee in coAssignees)
      {
        var item = _obj.PartsCoAssignees.AddNew();
        item.CoAssignee = coAssignee;
        item.PartGuid = actionItemPart.PartGuid;
      }
      
      actionItemPart.CoAssignees = Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssignees, true);
    }

    /// <summary>
    /// Редактировать пункт шаблона поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="deadlineDaysOrHourse">Дни/Часы исполнителя.</param>
    /// <param name="deadline">Срок исполнителя.</param>
    /// <param name="actionItemPartText">Текст поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    /// <param name="coAssigneesDeadlineDaysOrHourse">Днич/Часы соисполнителей.</param>
    /// <param name="coAssigneesDeadline">Срок соисполнителей.</param>
    /// <param name="supervisor">Контролер.</param>
    [Public]
    public void EditActionItemPart(IAssignmentsTemplateActionItemParts actionItemPart, Sungero.Company.IEmployee assignee,
                                   int? deadline, string deadlineDaysOrHourse,
                                   string actionItemPartText, List<Sungero.Company.IEmployee> coAssignees,
                                   int? coAssigneesDeadline, string coAssigneesDeadlineDaysOrHourse,
                                   Sungero.Company.IEmployee supervisor)
    {
      actionItemPart.ActionItemPart = actionItemPartText;
      actionItemPart.Assignee = assignee;
      actionItemPart.Count = deadline;
      actionItemPart.DaysOrHours = deadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ?
        DaysOrHours.Days : DaysOrHours.Hours;
      actionItemPart.CoAssigneesCount = coAssigneesDeadline;
      var daysOrHours = coAssigneesDeadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ?
        DaysOrHours.Days : DaysOrHours.Hours;
      if (coAssignees.Any())
        actionItemPart.CoAssigneesDaysOrHours = daysOrHours;
      else
        actionItemPart.CoAssigneesDaysOrHours = null;
      actionItemPart.Supervisor = supervisor;
      DeletePartsCoAssignees(actionItemPart);
      AddPartsCoAssignees(actionItemPart, coAssignees);
    }
    
    /// <summary>
    /// Удалить всех соисполнителей для пункта поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    [Public]
    public void DeletePartsCoAssignees(GD.ActionTemplateModule.IAssignmentsTemplateActionItemParts actionItemPart)
    {
      var partsCoAssignees = _obj.PartsCoAssignees.Where(p => p.PartGuid == actionItemPart.PartGuid).ToList();
      
      foreach (var partCoAssignees in partsCoAssignees)
      {
        _obj.PartsCoAssignees.Remove(partCoAssignees);
      }
      
      actionItemPart.CoAssignees = null;
    }
    
    /// <summary>
    /// Добавить соисполнителей для пункта поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    [Public]
    public void AddPartsCoAssignees(GD.ActionTemplateModule.IAssignmentsTemplateActionItemParts actionItemPart, List<Sungero.Company.IEmployee> coAssignees)
    {
      foreach (var coAssignee in coAssignees)
      {
        var item = _obj.PartsCoAssignees.AddNew();
        item.CoAssignee = coAssignee;
        item.PartGuid = actionItemPart.PartGuid;
      }
      
      actionItemPart.CoAssignees = Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssignees, true);
    }

    /// <summary>
    /// Получить соисполнителей по пункту поручения.
    /// </summary>
    /// <param name="partGuid">Идентификатор пункта поручения.</param>
    /// <returns>Список соисполнителей.</returns>
    public virtual List<Sungero.Company.IEmployee> GetPartCoAssignees(string partGuid)
    {
      return _obj.PartsCoAssignees.Where(p => p.PartGuid == partGuid).Select(p => p.CoAssignee).ToList();
    }
  }
}