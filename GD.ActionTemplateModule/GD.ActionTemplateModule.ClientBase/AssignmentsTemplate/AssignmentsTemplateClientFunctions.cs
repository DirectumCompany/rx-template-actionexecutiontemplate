using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;
using Sungero.RecordManagement;

namespace GD.ActionTemplateModule.Client
{
  partial class AssignmentsTemplateFunctions
  {
    /// <summary>
    /// Заполнить пункт составного поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт составного поручения.</param>
    public void FillCompoundActionItemPart(IAssignmentsTemplateActionItemParts itemPart)
    {
      var settings = Sungero.RecordManagement.PublicFunctions.Module.GetSettings();
      var isSupervisorChanges = false;
      var isAssigneeChanges = false;
      var isDeadlineChanges = false;
      var isCoAssigneesChanges = false;
      var isCoAssigneesDeadlineChanges = false;
      var isActionItemTextChanges = false;
      var isAddItemPart = itemPart == null;
      var title = isAddItemPart ? Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddCompoundActionItemPart :
        Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EditCompoundActionItemPartFormat(itemPart.ActionItemPart);
      var supervisorDefault = isAddItemPart ? _obj.Supervisor : itemPart.Supervisor ?? _obj.Supervisor;
      isSupervisorChanges = !isAddItemPart && itemPart.Supervisor == null && _obj.Supervisor != null;
      var assigneeDefault = isAddItemPart ? Sungero.Company.Employees.Null : itemPart.Assignee;
      var deadlineDefault = isAddItemPart ? _obj.FinalCount : itemPart.Count ?? _obj.FinalCount;
      isDeadlineChanges = !isAddItemPart && itemPart.Count == null && _obj.FinalCount != null;
      var coAssigneesDefault = isAddItemPart ? new List<Sungero.Company.IEmployee>() : Functions.AssignmentsTemplate.GetPartCoAssignees(_obj, itemPart.PartGuid);
      int? coAssigneesDeadlineDefault = null;
      if (!isAddItemPart && coAssigneesDefault.Any())
      {
        isCoAssigneesDeadlineChanges = itemPart.CoAssigneesCount == null && deadlineDefault != null;
        coAssigneesDeadlineDefault = itemPart.CoAssigneesCount;
      }
      var itemPartDefault = isAddItemPart ? string.Empty : itemPart.ActionItemPart;
      var titleButton = isAddItemPart ? Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddButtonDialog :
        Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EditButtonDialog;
      
      #region Создание диологового окна
      
      var dialog = Dialogs.CreateInputDialog(title);
      dialog.HelpCode = isAddItemPart ? Constants.AssignmentsTemplate.AddActionItemHelpCode : Constants.AssignmentsTemplate.EditActionItemHelpCode;
      var underControl = _obj.IsUnderControl == true;
      var supervisor = dialog.AddSelect(_obj.Info.Properties.Supervisor.LocalizedName, underControl, supervisorDefault)
        .Where(a => a.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      supervisor.IsEnabled = underControl;
      
      var assignee = dialog.AddSelect(_obj.Info.Properties.Assignee.LocalizedName, true, assigneeDefault)
        .Where(a => a.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      var deadline = dialog.AddInteger(_obj.Info.Properties.Count.LocalizedName, false, deadlineDefault);
      deadline.IsEnabled = _obj.HasIndefiniteDeadline != true;
      deadline.IsRequired = deadline.IsEnabled;
      
      var deadlineDaysOrHourse = dialog.AddSelect(_obj.Info.Properties.FinalDaysOrHours.LocalizedName, false, "Дней").From("Дней", "Часов");
      
      var coAssignees = dialog.AddSelectMany(_obj.Info.Properties.CoAssignees.LocalizedName, false, coAssigneesDefault.ToArray());
      coAssignees.IsEnabled = false;
      coAssignees.IsVisible = false;
      var coAssigneesText = dialog
        .AddMultilineString(_obj.Info.Properties.CoAssignees.LocalizedName,
                            false,
                            Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssigneesDefault, false))
        .WithRowsCount(Sungero.RecordManagement.PublicConstants.Module.CoAssigneesTextRowsCount);
      coAssigneesText.IsEnabled = false;
      
      var addCoAssignees = dialog.AddHyperlink(ActionItemExecutionTasks.Resources.AddCoAssignees);
      var deleteCoAssignees = dialog.AddHyperlink(ActionItemExecutionTasks.Resources.RemoveCoAssignees);
      var coAssigneesDeadline = dialog.AddInteger(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineDialog,
                                                  false,
                                                  coAssigneesDeadlineDefault);
      coAssigneesDeadline.IsEnabled = coAssigneesDefault.Any();
      coAssigneesDeadline.IsRequired = coAssigneesDefault.Any() && _obj.HasIndefiniteDeadline != true;
      
      var coAssigneesDeadlineDaysOrHourse = dialog.AddSelect(_obj.Info.Properties.FinalDaysOrHours.LocalizedName, false, "Дней").From("Дней", "Часов");
      
      var actionItemPartText = dialog
        .AddMultilineString(_obj.Info.Properties.ActionItemParts.Properties.ActionItemPart.LocalizedName, false, itemPartDefault)
        .WithRowsCount(Sungero.RecordManagement.PublicConstants.Module.ActionItemPartTextRowsCount);
      
      var fillButton = dialog.Buttons.AddCustom(titleButton);
      dialog.Buttons.AddCancel();
      
      dialog.SetOnRefresh(
        (args) =>
        {
          if (!deadline.Value.HasValue || deadline.Value.Value <= 0)
            args.AddError(ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday, deadline);
          
          //TODO Zaytsev: В окне при загрузке всплывет окно если сделать <= 0.
          if (coAssigneesDeadline.Value.HasValue && coAssigneesDeadline.Value.Value < 0)
            args.AddError(ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday);
          
          var assigneeDeadline = deadline.Value.HasValue ? deadline.Value : _obj.FinalCount;
          
          if (assigneeDeadline != null && coAssigneesDeadline != null && assigneeDeadline.Value < coAssigneesDeadline.Value)
            args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineError);
          
          fillButton.IsEnabled = isSupervisorChanges || isAssigneeChanges ||
            isDeadlineChanges || isCoAssigneesChanges ||
            isCoAssigneesDeadlineChanges || isActionItemTextChanges;
        });
      
      // Контролер.
      supervisor.SetOnValueChanged(
        (args) =>
        {
          isSupervisorChanges = !Equals(args.NewValue, supervisorDefault);
        });
      
      // Исполнитель.
      assignee.SetOnValueChanged(
        (args) =>
        {
          isAssigneeChanges = !Equals(args.NewValue, assigneeDefault);
        });
      
      // Срок исполнителя.
      deadline.SetOnValueChanged(
        (args) =>
        {
          isDeadlineChanges = !Equals(args.NewValue, deadlineDefault);
        });
      
      // Соисполнители.
      coAssignees.SetOnValueChanged(
        (args) =>
        {
          coAssigneesText.Value = Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssignees.Value.ToList(), false);
          isCoAssigneesChanges = !coAssigneesDefault.SequenceEqual(coAssignees.Value.ToList());

          var coAssigneesExist = coAssignees.Value.Any();
          coAssigneesDeadline.IsRequired = coAssigneesExist && _obj.HasIndefiniteDeadline != true;
          coAssigneesDeadline.IsEnabled = coAssigneesExist;
          
          coAssigneesDeadlineDaysOrHourse.IsEnabled = coAssigneesExist;
          coAssigneesDeadlineDaysOrHourse.IsRequired = coAssigneesExist && _obj.HasIndefiniteDeadline != true;
          
          if (!coAssigneesExist)
            coAssigneesDeadline.Value = null;
        });
      
      // Срок соисполнителей.
      coAssigneesDeadline.SetOnValueChanged(
        (args) =>
        {
          isCoAssigneesDeadlineChanges = !Equals(args.NewValue, coAssigneesDeadlineDefault);
        });
      
      // Текст поручения.
      actionItemPartText.SetOnValueChanged(
        (args) =>
        {
          isActionItemTextChanges = !Equals(args.NewValue, itemPartDefault ?? string.Empty);
        });
      
      #region Гиперссылки на добавление и удаление соисполнителей

      addCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = Sungero.Company.PublicFunctions.Employee.Remote.GetEmployees()
            .Where(ca => ca.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .ShowSelectMany(ActionItemExecutionTasks.Resources.ChooseCoAssigneesForAdd).ToList();

          if (selectedEmployees != null && selectedEmployees.Any())
          {
            var sourceCoAssignees = coAssignees.Value.ToList();
            sourceCoAssignees.AddRange(selectedEmployees);
            coAssignees.Value = sourceCoAssignees.Distinct();

            if (!coAssigneesDeadline.Value.HasValue && coAssignees.Value.Any())
              coAssigneesDeadline.Value = deadline.Value;
          }
        });

      deleteCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = coAssignees.Value.ShowSelectMany(ActionItemExecutionTasks.Resources.ChooseCoAssigneesForDelete);
          if (selectedEmployees != null && selectedEmployees.Any())
          {
            var currentCoAssignees = coAssignees.Value.ToList();

            foreach (var employee in selectedEmployees)
            {
              currentCoAssignees.Remove(employee);
            }

            coAssignees.Value = currentCoAssignees;
          }
        });

      #endregion
      
      dialog.SetOnButtonClick(
        args =>
        {
          if (args.Button == fillButton)
          {
            if (deadline.Value.HasValue && deadline.Value.Value <= 0)
            {
              args.AddError(ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday, deadline);
              return;
            }

            if (coAssigneesDeadline.Value.HasValue && coAssigneesDeadline.Value.Value <= 0)
            {
              args.AddError(ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday);
              return;
            }

            var assigneeDeadline = deadline.Value.HasValue ? deadline.Value : _obj.FinalCount;

            if (assigneeDeadline != null && coAssigneesDeadline.Value.HasValue && deadline.Value.Value < coAssigneesDeadline.Value.Value)
            {
              args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineError);
              return;
            }

            if (args.IsValid)
            {
              if (isAddItemPart)
                Functions.AssignmentsTemplate.AddActionItemPart(_obj, assignee.Value,
                                                                deadline.Value,
                                                                deadlineDaysOrHourse.Value,
                                                                actionItemPartText.Value,
                                                                coAssignees.Value.ToList(),
                                                                coAssigneesDeadline.Value,
                                                                coAssigneesDeadlineDaysOrHourse.Value,
                                                                supervisor.Value);
              else
                Functions.AssignmentsTemplate.EditActionItemPart(_obj, itemPart,
                                                                 assignee.Value, deadline.Value,
                                                                 deadlineDaysOrHourse.Value,
                                                                 actionItemPartText.Value,
                                                                 coAssignees.Value.ToList(),
                                                                 coAssigneesDeadline.Value,
                                                                 coAssigneesDeadlineDaysOrHourse.Value,
                                                                 supervisor.Value);
            }
          }
        });

      dialog.Show();
      
      #endregion
      
    }
    
    /// <summary>
    /// Проверка, что хотя бы одно доступное свойство пункта составного поручения заполнено.
    /// </summary>
    /// <param name="itemPart">Пункт составного поручения.</param>
    /// <returns>True - корректировка возможна, иначе - false.</returns>
    /// <remarks>Проверка добавлена, так как платформа при сохранении задачи удаляет пустые пункты (187563).</remarks>
    [Public]
    public virtual bool CanChangeActionItemPart(IAssignmentsTemplateActionItemParts itemPart)
    {
      return itemPart.Assignee != null || itemPart.ActionItemPart != null ||
        itemPart.Count != null || itemPart.Supervisor != null;
    }

    /// <summary>
    /// Проверить возможность корректировки поручения.
    /// </summary>
    /// <returns>True - корректировка возможна, иначе - false.</returns>
    [Public]
    public virtual bool CanChangeActionItem()
    {
      // Корректировать можно только поручения, созданные вручную, либо пункты составного поручения.
      // Простые поручения соисполнителям корректировать нельзя.
      //TODO Zaytsev: Надо будет доделать при добавлении кнопки "равноправные исполнители".
      // if (_obj. != ActionItemType.Main && _obj.ActionItemType != ActionItemType.Component)
      // return false;
      
      // Корректировать можно, только если есть права на изменение поручения.
      if (!_obj.AccessRights.CanUpdate())
        return false;
      
      // Корректировка недоступна в десктоп-клиенте.
      if (ClientApplication.ApplicationType == ApplicationType.Desktop)
        return false;
      
      // Возможность корректировки появилась только в 3 версии схемы
      // и только для поручений, находящихся в работе.
      // return _obj.GetStartedSchemeVersion() >= LayerSchemeVersions.V3 &&
      // _obj.Status == Sungero.Workflow.Task.Status.InProcess;
      return true;
    }
  }
}