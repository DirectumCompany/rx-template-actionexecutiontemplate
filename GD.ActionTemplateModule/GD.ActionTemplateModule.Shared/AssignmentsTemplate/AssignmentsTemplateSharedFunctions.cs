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
    /// Добавить пункт поручения.
    /// </summary>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="deadline">Срок исполнителя.</param>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    /// <param name="coAssigneesDeadline">Срок соисполнителей.</param>
    /// <param name="supervisor">Контролер.</param>
    [Public]
    public void AddActionItemPart(Sungero.Company.IEmployee assignee, int? deadline, string deadlineDaysOrHourse, string actionItemPart, List<Sungero.Company.IEmployee> coAssignees, int? coAssigneesDeadline, string coAssigneesDeadlineDaysOrHourse, Sungero.Company.IEmployee supervisor)
    {
      var actionItem = _obj.ActionItemParts.AddNew();
      actionItem.ActionItemPart = actionItemPart;
      actionItem.Assignee = assignee;
      actionItem.Count = deadline.Value;
      actionItem.DaysOrHours = coAssigneesDeadlineDaysOrHourse == "Дней" ? DaysOrHours.Days : DaysOrHours.Hours;
      actionItem.CoAssigneesCount = coAssigneesDeadline;
      actionItem.CoAssigneesDaysOrHours = coAssigneesDeadlineDaysOrHourse == "Дней" ? DaysOrHours.Days : DaysOrHours.Hours;
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
    /// Редактировать пункт поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="deadline">Срок исполнителя.</param>
    /// <param name="actionItemPartText">Текст поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
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
      actionItemPart.DaysOrHours = coAssigneesDeadlineDaysOrHourse == "Дней" ? DaysOrHours.Days : DaysOrHours.Hours;
      actionItemPart.CoAssigneesCount = coAssigneesDeadline.Value;
      actionItemPart.CoAssigneesDaysOrHours = coAssigneesDeadlineDaysOrHourse == "Дней" ? DaysOrHours.Days : DaysOrHours.Hours;
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