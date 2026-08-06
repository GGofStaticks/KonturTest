# KonturTest

Тестовое задание. WPF-приложение: XSLT превращает Data1.xml / Data2.xml в Employees.xml, считает total по сотрудникам и месяцам, для Data1 ещё пишет total в Pay и даёт добавить запись через форму.

## Требования

Windows, .NET 10 SDK, Rider или Visual Studio с WPF.

Открыть KonturTest.sln, Clean + Rebuild, запустить проект KonturTest.

## Использование

Выбрать файл в комбобоксе источник (Data1.xml или Data2.xml), нажать запустить.

Список файлов обновляется при открытии комбобокса и перед каждым запуском.

После запуска:

- таблица сотрудников (имя, фамилия, total)
- таблица выплат по месяцам
- блок результат (Employees.xml) текст сгенерированного файла

Блок с Employees.xml добавил для проверки результата глазами.

Если выбран Data1.xml, внизу активируется форма добавления записи (имя, фамилия, сумма, месяц). Кнопка добавить и пересчитать дописывает item в Data1.xml и запускает тот же пересчёт, что и запустить. Форма после успеха очищается.

Сумму можно вводить с точкой или запятой, например 1000 или 3001,10.

## Структура

Resources/ исходные xml и xslt из задания, в git не меняются.

Helpers/ мелкие утилиты (суммы, месяцы).

Models/ dto для таблиц и результата pipeline.

Services/ вся логика: xslt, правка Data1, расчёт total, пути к файлам. Рядом интерфейсы.

ViewModels/ mvvm, команды, данные для ui.

MainWindow.xaml - ui. MainWindow.xaml.cs - подключение ViewModel и создание сервисов через new.

При запуске рабочие копии лежат в bin/Debug/net10.0-windows/Resources/:

- Data1.xml копия, сюда пишутся новые item и total на Pay
- Employees.xml результат преобразования
- остальное читается из Resources/ проекта, если в bin нет



## ТЗ

1. XSLT, группировка по сотруднику. Data1: `@mount`, Data2: имя родителя.
  `Resources/PayToEmployees.xslt`
2. Запуск XSLT из C#.
  `XsltTransformService`
3. Атрибут `total` на каждом Employee.
  `EmployeeDocumentService`
4. Атрибут `total` на Pay в Data1.
  `Data1Repository.UpdatePayTotal`
5. GUI: кнопка, таблицы.
  `MainWindow.xaml`, `MainViewModel`
6. Добавить item в Data1 и пересчитать.
  `Data1Repository.AddItem`, `PayrollService.AddItemAndProcess`

Порядок в `PayrollService`: подготовка файлов, total в Pay (для Data1), XSLT, total у Employee, запись `Employees.xml`, данные для таблиц.

Если AddItem упал, `Data1.xml` откатывается в начальное состояние из `.bak`.

## Заметки по данным

- атрибут mount как в исходниках задания, не переименовывал
- Data2: месяц из тега (february и т.д.), не из @mount; в примере у february mount="january", XSLT смотрит на родителя
- суммы: запятая и точка как в примере, при добавлении пишется как ввели
- месяцы в UI: january, february, как в XML



## Если доделывать в полноценное решение

- CopyToOutputDirectory для Resources/**
- тесты на AmountParser, XSLT, сортировку месяцев
- нормализовать суммы через AmountParser.Format если нормализация необходима
- async и IsBusy для ускорения работы и избегания подвисаний

