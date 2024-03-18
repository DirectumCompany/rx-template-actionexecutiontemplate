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
    /// Получить тему поручения.
    /// </summary>
    /// <param name="task">Поручение.</param>
    /// <param name="beginningSubject">Изначальная тема.</param>
    /// <returns>Сформированная тема поручения.</returns>
    //TODO Zaytsev: Стоит ли вообще делать это?
//    public static string GetActionItemExecutionSubject(IAssignmentsTemplate template, CommonLibrary.LocalizedString beginningSubject)
//    {
//      var autoSubject = Sungero.Docflow.Resources.AutoformatTaskSubject.ToString();
//
//      using (TenantInfo.Culture.SwitchTo())
//      {
//        var subject = beginningSubject.ToString();
//        var actionItem = template.Text;
//
//        // Добавить резолюцию в тему.
//        if (!string.IsNullOrWhiteSpace(actionItem))
//        {
//          var hasDocument = task.DocumentsGroup.OfficialDocuments.Any();
//          var formattedResolution = Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.FormatActionItemForSubject(actionItem, hasDocument);
//
//          // Конкретно у уведомления о старте составного поручения - всегда рисуем с кавычками.
//          if (!hasDocument && subject == Sungero.RecordManagement.ActionItemExecutionTasks.Resources.WorkFromActionItemIsCreatedCompound.ToString())
//            formattedResolution = string.Format("\"{0}\"", formattedResolution);
//
//          subject += string.Format(" {0}", formattedResolution);
//        }
//        
//        // Добавить ">> " для тем подзадач.
//        var isNotMainTask = task.ActionItemType != Sungero.RecordManagement.ActionItemExecutionTask.ActionItemType.Main;
//        if (isNotMainTask)
//          subject = string.Format(">> {0}", subject);
//        
//        // Добавить имя документа, если поручение с документом.
//        var document = task.DocumentsGroup.OfficialDocuments.FirstOrDefault();
//        if (document != null)
//          subject += Sungero.RecordManagement.ActionItemExecutionTasks.Resources.SubjectWithDocumentFormat(document.Name);
//
//        subject = Sungero.Docflow.Docflow.PublicFunctions.Module.TrimSpecialSymbols(subject);
//
//        if (subject != beginningSubject)
//          return subject;
//      }
//      
//      return autoSubject;
//    }

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
      actionItemPart.DaysOrHours = deadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ? DaysOrHours.Days : DaysOrHours.Hours;
      actionItemPart.CoAssigneesCount = coAssigneesDeadline;
      actionItemPart.CoAssigneesDaysOrHours = coAssigneesDeadlineDaysOrHourse == _obj.Info.Properties.FinalDaysOrHours.GetLocalizedValue(DaysOrHours.Days) ? DaysOrHours.Days : DaysOrHours.Hours;
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