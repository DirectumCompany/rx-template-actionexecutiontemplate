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
    /// Проверка условий для сохранения шаблона поручений.
    /// </summary>
    /// <param name="supervisor">Контролер.</param>
    /// <param name="assigneeCount">Срок исполнителя</param>
    /// <param name="assigneeDaysOrHours">Дней/Часов исполнителя.</param>
    /// <param name="isCoAssignees">Существуют соисполнители.</param>
    /// <param name="coAssigneesCount">Срок соисполнителей.</param>
    /// <param name="coAssigneesDaysOrHours">Дней/Часов соисполнителей.</param>
    /// <param name="itemPart">Пункт поручения.</param>
    /// <returns>Строка с ошибкой.</returns>
    public string CheckConditions(Sungero.Company.IEmployee supervisor,
                                  int? assigneeCount, string assigneeDaysOrHours,
                                  bool isCoAssignees, int? coAssigneesCount,
                                  string coAssigneesDaysOrHours, IAssignmentsTemplateActionItemParts itemPart)
    {
      // Шаблон без срока.
      var hasIndefiniteDeadline = _obj.HasIndefiniteDeadline == true;

      // Проверить шаблон стоит на контроле.
      if (_obj.IsUnderControl == true && supervisor == null)
        return AssignmentsTemplates.Resources.EmptySupervisor;
      
      // Проверка сроков исполнителя.
      if (!hasIndefiniteDeadline)
      {
        int? finalCount = null;
        if (_obj.FinalDaysOrHours != null && _obj.FinalDaysOrHours.HasValue)
          finalCount = ConvertToHours(_obj.FinalDaysOrHours, _obj.FinalCount);
        
        var assigneeDeadline = ConvertToHours(assigneeDaysOrHours, assigneeCount);

        var error = CheckAssigneeConditions(assigneeDeadline, assigneeDaysOrHours, finalCount);
        if (!string.IsNullOrEmpty(error))
          return error;

        if (isCoAssignees)
        {
          var coAssigneesDeadline = ConvertToHours(coAssigneesDaysOrHours, coAssigneesCount);
          error = CheckCoAssigneeConditions(coAssigneesDeadline, coAssigneesDaysOrHours, assigneeDeadline, assigneeDaysOrHours, finalCount);
          if (!string.IsNullOrEmpty(error))
            return error;
        }
      }

      return null;
    }

    /// <summary>
    /// Проверка условий исполнителя для сохранения шаблона получения.
    /// </summary>
    /// <param name="assigneeDeadline">Срок исполнителя.</param>
    /// <param name="assigneeDaysOrHours">Дней/Часов исполнителя.</param>
    /// <param name="finalCount">Общий срок в часах.</param>
    /// <returns>Строка с ошибкой.</returns>
    private string CheckAssigneeConditions(int? assigneeDeadline, string assigneeDaysOrHours, int? finalCount)
    {
      if (assigneeDeadline.HasValue && assigneeDeadline.Value <= 0)
        return Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday;

      if (finalCount.HasValue && assigneeDeadline > finalCount)
        return AssignmentsTemplates.Resources.AssigneeFinalDaysOrHours;

      if (string.IsNullOrEmpty(assigneeDaysOrHours) && assigneeDeadline != null && assigneeDeadline.HasValue)
        return AssignmentsTemplates.Resources.EmptyAssigneeDaysOrHours;

      if(_obj.HasIndefiniteDeadline == false && !finalCount.HasValue && !assigneeDeadline.HasValue)
        return AssignmentsTemplates.Resources.EmptyFinalDeadline;
      
      return null;
    }

    /// <summary>
    /// Проверка условий соисполнителя для сохранения шаблона получения
    /// </summary>
    /// <param name="coAssigneesDeadline">Срок соисполнителя.</param>
    /// <param name="coAssigneesDaysOrHours">Дней/Часов соисполнетеля.</param>
    /// <param name="assigneeDeadline">Срок соисполнителя в часах.</param>
    /// <param name="assigneeDaysOrHours">Дней/Часов исполнителя.</param>
    /// <param name="finalCount">Общий срок в часах.</param>
    /// <returns>Строка с ошибкой.</returns>
    private string CheckCoAssigneeConditions(int? coAssigneesDeadline, string coAssigneesDaysOrHours,
                                             int? assigneeDeadline, string assigneeDaysOrHours, int? finalCount)
    {
      if (string.IsNullOrEmpty(coAssigneesDaysOrHours) && coAssigneesDeadline != null && coAssigneesDeadline.HasValue)
        return AssignmentsTemplates.Resources.EmptyCoAssigneesDaysOrHours;

      if (coAssigneesDeadline.HasValue && coAssigneesDeadline.Value <= 0)
        return Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday;

      if (coAssigneesDeadline.HasValue && coAssigneesDeadline > assigneeDeadline)
        return AssignmentsTemplates.Resources.CoAssigneesDeadlineError;

      if (finalCount.HasValue && coAssigneesDeadline > finalCount)
        return AssignmentsTemplates.Resources.CoAssigneeFinalDaysOrHours;
      
      return null;
    }

    
    /// <summary>
    /// Преобразовать дни в часы.
    /// </summary>
    /// <param name="daysOrHours">Тип времени</param>
    /// <param name="count">Количество дней.</param>
    public int? ConvertToHours(Enumeration? daysOrHours, int? count)
    {
      if (count.HasValue && daysOrHours.Value.Value == DaysOrHours.Days.Value)
        count *= PublicConstants.AssignmentsTemplate.DayHours;
      
      return count;
    }
    
    /// <summary>
    /// Преобразовать дни в часы.
    /// </summary>
    /// <param name="daysOrHours">Локализованнаое значение свойства Дни/Часы.</param>
    /// <param name="count">Количество дней.</param>
    public int? ConvertToHours(string daysOrHours, int? count)
    {
      if (count.HasValue && daysOrHours == _obj.Info.Properties.DaysOrHours.GetLocalizedValue(DaysOrHours.Days))
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
      if (deadline != null && deadline.HasValue)
        actionItem.Count = deadline.Value;
      else
        actionItem.Count = null;
      actionItem.DaysOrHours = deadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ? DaysOrHours.Days :
        DaysOrHours.Hours;
      if (coAssignees.Any())
      {
        actionItem.CoAssigneesCount = coAssigneesDeadline;
        actionItem.CoAssigneesDaysOrHours = coAssigneesDeadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ?
          DaysOrHours.Days : DaysOrHours.Hours;
      }
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
      if (deadline != null && deadline.HasValue)
        actionItemPart.Count = deadline.Value;
      else
        actionItemPart.Count = null;
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