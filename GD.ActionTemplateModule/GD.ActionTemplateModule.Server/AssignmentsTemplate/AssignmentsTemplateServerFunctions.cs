using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using GD.ActionTemplateModule.AssignmentsTemplate;

namespace GD.ActionTemplateModule.Server
{
  partial class AssignmentsTemplateFunctions
  {
    /// <summary>
    /// Создать шаблон поручения/резолюции.
    /// </summary>
    [Public, Remote(PackResultEntityEagerly = true)]
    public static IAssignmentsTemplate CreateTemplate()
    {
      return  AssignmentsTemplates.Create();
    }
    
    /// <summary>
    /// Получить список доступных шаблонов поручений/резолюций.
    /// </summary>
    /// <returns>Список шаблонов.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<IAssignmentsTemplate> GetAvailableTemplates()
    {
      return AssignmentsTemplates.GetAll();
    }
  }
}