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
    /// Сохрнаить данные поручения в шаблон
    /// </summary>
    [Public]
    public void ToTemplate(IActionItemExecutionTask task)
    {
      var template = ActionTemplateModule.PublicFunctions.AssignmentsTemplate.Remote.CreateTemplate();
      template.Text = task.ActionItem;
      template.AssignedBy = task.AssignedBy;
      template.IsUnderControl = task.IsUnderControl;
      template.Supervisor = task.Supervisor;
      template.Assignee = task.Assignee;
      foreach (var coAssignees in task.CoAssignees)
      {
        var newCoAssignee = template.CoAssignees.AddNew();
        newCoAssignee.CoAssignee = coAssignees.Assignee;
      }
      template.Show();
    }
    
    /// <summary>
    /// Получить список доступных шаблонов поручений и заполнить данные по выбранному шаблону.
    /// </summary>
    [Public]
    public void FromTemplate(IActionItemExecutionTask task)
    {
      var template = ActionTemplateModule.PublicFunctions.AssignmentsTemplate.Remote.GetAvailableTemplates().ShowSelect();
      if (template != null)
      {
        task.ActionItem = template.Text;
        task.AssignedBy = template.AssignedBy;
        task.IsUnderControl = template.IsUnderControl;
        task.Supervisor = template.Supervisor;
        task.Assignee = template.Assignee;
        
        if (template.DaysOrHours != null && template.Count != null)
        {
          if (template.DaysOrHours == AssignmentsTemplate.DaysOrHours.Days)
            task.Deadline = Calendar.Today.AddWorkingDays(template.Count.Value);
          else
            task.Deadline = Calendar.Now.AddWorkingHours(template.Count.Value);
        }
        
        task.CoAssignees.Clear();
        foreach (var coAssignees in template.CoAssignees)
        {
          var newCoAssignee = task.CoAssignees.AddNew();
          newCoAssignee.Assignee = coAssignees.CoAssignee;
        }
      }
    }
  }
}
