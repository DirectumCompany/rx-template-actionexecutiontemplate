# rx-template-actionexecutiontemplate
Репозиторий с шаблоном разработки для реализации шаблонов поручений.

## Описание
Шаблон позволяет:
* Заполнить в справочнике "Шаблоны поручений" значения реквизитов повторяющихся поручений.
* Заполнять реквизиты задачи по исполнению поручений из справочника шаблонов поручений по кнопке "Из шаблона".
* Создавать новый шаблонов поручения прямо из задачи по исполнению поручения по кнопке "В шаблон".

Состав объектов разработки:
* Справочник "Шаблоны поручений"
* Клиентская функция модуля ToTemplate, позволяет сохрнаить данные поручения в шаблон
* Клиентская функция модуля FromTemplate, позволяет получить список доступных шаблонов поручений и заполнить данные по выбранному шаблону.

## Варианты расширения функциональности на проектах
1. Перекрыть задачу по исполнению поручений для добавления кнопок "Из шаблона" и "В шаблон":  
* Пример вызова FromTemplate в кнопке "Из шаблона":
```
GD.ActionTemplateModule.PublicFunctions.Module.FromTemplate(_obj, e);
```  
* Пример вызова ToTemplate в кнопке "В шаблон":
```
GD.ActionTemplateModule.PublicFunctions.Module.ToTemplate(_obj);
```
2. Добавление реквизитов в справочник "Шаблоны поручений".

## Порядок установки

### Установка для ознакомления
1. Склонировать репозиторий Reports в папку.
2. Указать в _ConfigSettings.xml DDS:
```xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" />
  <repository folderName="RX" solutionType="Base" url="<адрес локального репозитория>" />
  <repository folderName="<Папка из п.1>" solutionType="Work" 
     url="https://github.com/DirectumCompany/rx-template-actionexecutiontemplate" />
</block>
```

### Установка для использования на проекте
Возможные варианты:

**A. Fork репозитория**
1. Сделать fork репозитория Reports для своей учетной записи.
2. Склонировать созданный в п. 1 репозиторий в папку.
3. Указать в _ConfigSettings.xml DDS:
``` xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.2>" solutionType="Work" 
     url="<Адрес репозитория gitHub учетной записи пользователя из п. 1>" />
</block>
```

**B. Подключение на базовый слой.**

Вариант не рекомендуется, так как при выходе версии шаблона разработки не гарантируется обратная совместимость.
1. Склонировать репозиторий Reports в папку.
2. Указать в _ConfigSettings.xml DDS:
``` xml
<block name="REPOSITORIES">
  <repository folderName="Base" solutionType="Base" url="" /> 
  <repository folderName="<Папка из п.1>" solutionType="Base" 
     url="<Адрес репозитория gitHub>" />
  <repository folderName="<Папка для рабочего слоя>" solutionType="Work" 
     url="https://github.com/DirectumCompany/rx-template-actionexecutiontemplate" />
</block>
```

**C. Копирование репозитория в систему контроля версий.**

Рекомендуемый вариант для проектов внедрения.
1. В системе контроля версий с поддержкой git создать новый репозиторий.
2. Склонировать репозиторий Reports в папку с ключом `--mirror`.
3. Перейти в папку из п. 2.
4. Импортировать клонированный репозиторий в систему контроля версий командой:

`git push –mirror <Адрес репозитория из п. 1>`

