using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.RecordManagement;
using Sungero.Docflow;
using Sungero.Workflow;

namespace GD.ActionTemplateModule.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Сохранить данные поручения в шаблон
    /// </summary>
    [Public]
    public void ToTemplate(IActionItemExecutionTask task)
    {
      var template = ActionTemplateModule.PublicFunctions.AssignmentsTemplate.Remote.CreateTemplate();
      template.IsCompoundActionItem = task.IsCompoundActionItem;
      template.Text = task.ActiveText;
      template.AssignedBy = task.AssignedBy;
      template.IsUnderControl = task.IsUnderControl;
      template.Supervisor = task.Supervisor;
      template.HasIndefiniteDeadline = task.HasIndefiniteDeadline;
      
      if (task.IsCompoundActionItem == true)
      {
        foreach (var actionItemPart in task.ActionItemParts)
        {
          var coAssignees = task.PartsCoAssignees.Where(p => p.PartGuid == actionItemPart.PartGuid).Select(p => p.CoAssignee).ToList();
          AddActionItemPart(template, actionItemPart.Number.Value, actionItemPart.Assignee, actionItemPart.ActionItemPart, coAssignees, actionItemPart.Supervisor);
        }
      }
      else
      {
        template.Assignee = task.Assignee;
        foreach (var coAssignees in task.CoAssignees)
        {
          var newCoAssignee = template.CoAssignees.AddNew();
          newCoAssignee.CoAssignee = coAssignees.Assignee;
        }
      }
      
      template.Show();
    }
    
    /// <summary>
    /// Получить список доступных шаблонов поручений и заполнить данные по выбранному шаблону.
    /// </summary>
    [Public]
    public void FromTemplate(IActionItemExecutionTask task, Sungero.Core.IValidationArgs e)
    {
      var template = ActionTemplateModule.PublicFunctions.AssignmentsTemplate.Remote.GetAvailableTemplates().ShowSelect();
      if (template != null)
      {
        if (template.HasIndefiniteDeadline == true && !Sungero.RecordManagement.PublicFunctions.Module.AllowActionItemsWithIndefiniteDeadline())
        {
          e.AddError(ActionItemExecutionTasks.Resources.ActionItemWithoutDeadlineDenied);
          return;
        }
        
        task.IsCompoundActionItem = template.IsCompoundActionItem;
        task.ActiveText = template.Text;
        task.AssignedBy = template.AssignedBy;
        task.IsUnderControl = template.IsUnderControl;
        task.Supervisor = template.Supervisor;
        task.HasIndefiniteDeadline = template.HasIndefiniteDeadline;
        
        if (template.IsCompoundActionItem == true)
          FillCompoundActionItemProperties(task, template);
        else
          FillSimpleActionItemProperties(task, template);
      }
    }
    
    public void FillCompoundActionItemProperties(IActionItemExecutionTask task, IAssignmentsTemplate template)
    {
      var hasNotIndefiniteDeadline = template.HasIndefiniteDeadline == false;
      
      if (hasNotIndefiniteDeadline)
      {
        if (template.FinalDaysOrHours != null && template.FinalCount != null)
        {
          if (template.FinalDaysOrHours == AssignmentsTemplate.FinalDaysOrHours.Days)
            task.FinalDeadline = Calendar.Today.AddWorkingDays(template.FinalCount.Value);
          else
            task.FinalDeadline = Calendar.Now.AddWorkingHours(template.FinalCount.Value);
        }
      }
      
      task.ActionItemParts.Clear();
      task.PartsCoAssignees.Clear();
      foreach (var actionItemPart in template.ActionItemParts)
      {
        DateTime? assigneeDeadline = null;
        if (hasNotIndefiniteDeadline && actionItemPart.DaysOrHours != null && actionItemPart.Count != null)
        {
          if (actionItemPart.DaysOrHours == AssignmentsTemplateActionItemParts.DaysOrHours.Days)
            assigneeDeadline = Calendar.Today.AddWorkingDays(actionItemPart.Count.Value);
          else
            assigneeDeadline = Calendar.Now.AddWorkingHours(actionItemPart.Count.Value);
        }
        
        DateTime? coAssigneesDeadline = null;
        if (hasNotIndefiniteDeadline && actionItemPart.CoAssigneesCount != null && actionItemPart.CoAssigneesDaysOrHours != null)
        {
          if (actionItemPart.CoAssigneesDaysOrHours == AssignmentsTemplateActionItemParts.CoAssigneesDaysOrHours.Days)
            coAssigneesDeadline = Calendar.Today.AddWorkingDays(actionItemPart.CoAssigneesCount.Value);
          else
            coAssigneesDeadline = Calendar.Now.AddWorkingHours(actionItemPart.CoAssigneesCount.Value);
        }
        
        var coAssignees = template.PartsCoAssignees.Where(p => p.PartGuid == actionItemPart.PartGuid).Select(p => p.CoAssignee).ToList();
        
        Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.AddActionItemPart(task,
                                                                                           actionItemPart.Assignee,
                                                                                           assigneeDeadline,
                                                                                           actionItemPart.ActionItemPart,
                                                                                           coAssignees,
                                                                                           coAssigneesDeadline,
                                                                                           actionItemPart.Supervisor);
      }
    }
    
    public void FillSimpleActionItemProperties(IActionItemExecutionTask task, IAssignmentsTemplate template)
    {
      var hasNotIndefiniteDeadline = template.HasIndefiniteDeadline == false;
      
      task.Assignee = template.Assignee;
      if (hasNotIndefiniteDeadline)
      {
        if (template.DaysOrHours != null && template.Count != null)
        {
          if (template.DaysOrHours == AssignmentsTemplate.DaysOrHours.Days)
            task.Deadline = Calendar.Today.AddWorkingDays(template.Count.Value);
          else
            task.Deadline = Calendar.Now.AddWorkingHours(template.Count.Value);
        }
        
        if (template.CoAssigneesCount != null && template.CoAssigneesDaysOrHours != null)
        {
          if (template.CoAssigneesDaysOrHours == AssignmentsTemplate.CoAssigneesDaysOrHours.Days)
            task.CoAssigneesDeadline = Calendar.Today.AddWorkingDays(template.CoAssigneesCount.Value);
          else
            task.CoAssigneesDeadline = Calendar.Now.AddWorkingHours(template.CoAssigneesCount.Value);
        }
      }
      
      task.CoAssignees.Clear();
      foreach (var coAssignees in template.CoAssignees)
      {
        var newCoAssignee = task.CoAssignees.AddNew();
        newCoAssignee.Assignee = coAssignees.CoAssignee;
      }
    }
    
    /// <summary>
    /// Добавить пункт порученияв шаблон.
    /// </summary>
    /// <param name="template">Шаблон.</param>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    /// <param name="supervisor">Контролер.</param>
    public void AddActionItemPart(IAssignmentsTemplate template, int number, Sungero.Company.IEmployee assignee, string actionItemPart,
                                  List<Sungero.Company.IEmployee> coAssignees, Sungero.Company.IEmployee supervisor)
    {
      var actionItem = template.ActionItemParts.AddNew();
      actionItem.Number = number;
      actionItem.ActionItemPart = actionItemPart;
      actionItem.Assignee = assignee;
      actionItem.Supervisor = supervisor;
      AddPartsCoAssignees(template, actionItem, coAssignees);
    }
    
    /// <summary>
    /// Добавить соисполнителей для пункта поручения.
    /// </summary>
    /// <param name="template">Шаблон.</param>
    /// <param name="actionItemPart">Пункт поручения.</param>
    /// <param name="coAssignees">Соисполнители.</param>
    public void AddPartsCoAssignees(IAssignmentsTemplate template, IAssignmentsTemplateActionItemParts actionItemPart, List<Sungero.Company.IEmployee> coAssignees)
    {
      foreach (var coAssignee in coAssignees)
      {
        var item = template.PartsCoAssignees.AddNew();
        item.CoAssignee = coAssignee;
        item.PartGuid = actionItemPart.PartGuid;
      }
      
      actionItemPart.CoAssignees = Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssignees, true);
    }
    
  }
}
