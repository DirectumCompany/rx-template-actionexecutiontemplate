using System;
using Sungero.Core;

namespace GD.ActionTemplateModule.Constants
{
  public static class AssignmentsTemplate
  {

    /// <summary>
    /// Количество часов в сутках.
    /// </summary>
    [Public]
    public const int DayHours = 24;
      
    /// <summary>
    /// Код диалога добавления пункта поручения.
    /// </summary>
    public const string AddActionItemHelpCode = "Sungero_AddActionItemDialog";
    
    /// <summary>
    /// Код диалога редактирования пункта поручения.
    /// </summary>
    public const string EditActionItemHelpCode = "Sungero_EditActionItemDialog";
  }
}