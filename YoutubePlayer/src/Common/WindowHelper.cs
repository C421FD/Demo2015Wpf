using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Common
{
  // Заюзано из другого проекта.
  /// <summary>
  /// Помошник работы с окнами приложения.
  /// </summary>
  public static class WindowHelper
  {
    #region Поля и свойства

    /// <summary>
    /// Текущее активное окно.
    /// </summary>
    public static Window ActiveWindow
    {
      get
      {
        var activeWindow = GetCustomActiveWindow != null ? GetCustomActiveWindow() : null;
        if (activeWindow != null)
          return activeWindow;

        return Application.Current != null && Application.Current.CheckAccess()
          ? Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive)
          : null;
      }
    }

    /// <summary>
    /// Делегат для кастомного получения активного окна.
    /// </summary>
    public static Func<Window> GetCustomActiveWindow { get; set; }

    #endregion

    #region Методы

    /// <summary>
    /// Получение родительского контрола определенного типа по логическому дереву.
    /// </summary>
    /// <typeparam name="T">Тип искомого родительского контрола.</typeparam>
    /// <param name="sourceControl">Исходный контрол.</param>
    /// <returns>Найденный родительский контрол.</returns>
    public static T GetParentControlFromLogicalTree<T>(DependencyObject sourceControl) where T : class
    {
      return GetParentControlFromLogicalTree(new[] { typeof(T) }, sourceControl, false) as T;
    }

    /// <summary>
    /// Получение родительского контрола определенного типа по визуальному дереву.
    /// </summary>
    /// <typeparam name="T">Тип искомого родительского контрола.</typeparam>
    /// <param name="sourceControl">Исходный контрол.</param>
    /// <returns>Найденный родительский контрол.</returns>
    public static T GetParentControlFromVisualTree<T>(DependencyObject sourceControl)
      where T : class
    {
      return GetParentControlFromVisualTree<T>(sourceControl, null, false);
    }

    /// <summary>
    /// Получение родительского контрола определенного типа по визуальному дереву.
    /// </summary>
    /// <typeparam name="T">Тип искомого родительского контрола.</typeparam>
    /// <param name="sourceControl">Исходный контрол.</param>
    /// <param name="parentName">Имя родительского контрола.</param>
    /// <param name="includeSelf">Признак, нужно ли включать в поиск сам исходный контрол.</param>
    /// <returns>Найденный родительский контрол.</returns>
    public static T GetParentControlFromVisualTree<T>(DependencyObject sourceControl, string parentName, bool includeSelf)
      where T : class
    {
      return GetParentControlFromVisualTree(new[] { typeof(T) }, sourceControl, parentName, includeSelf) as T;
    }

    /// <summary>
    /// Получение родительского контрола определенного типа по визуальному дереву.
    /// </summary>
    /// <param name="parentControlTypes">Тип искомого родительского контрола.</param>
    /// <param name="sourceControl">Исходный контрол.</param>
    /// <param name="parentName">Имя родительского контрола.</param>
    /// <param name="includeSelf">Признак, нужно ли включать в поиск сам исходный контрол.</param>
    /// <returns>Найденный родительский контрол.</returns>
    public static DependencyObject GetParentControlFromVisualTree(IEnumerable<Type> parentControlTypes, DependencyObject sourceControl, string parentName, bool includeSelf)
    {
      if (sourceControl == null)
        return null;

      // Иначе падает InvalidOperationException при обращении к VisualTreeHelper.GetParent().
      if (!(sourceControl is Visual || sourceControl is Visual3D))
        return null;

      Func<DependencyObject, bool> isParentTypeMatched = control => parentControlTypes.Any(type => type.IsAssignableFrom(control.GetType()));

      Func<DependencyObject, bool> isParentNameMatched = control =>
      {
        var frameworkElement = control as FrameworkElement;
        return string.IsNullOrEmpty(parentName) || (frameworkElement != null && frameworkElement.Name == parentName);
      };

      DependencyObject parentControl = includeSelf ? sourceControl : VisualTreeHelper.GetParent(sourceControl);
      while (parentControl != null && (!isParentTypeMatched(parentControl) || !isParentNameMatched(parentControl)))
        parentControl = VisualTreeHelper.GetParent(parentControl);
      return parentControl;
    }

    /// <summary>
    /// Получение родительского контрола определенного типа по логическому дереву.
    /// </summary>
    /// <param name="parentControlTypes">Тип искомого родительского контрола.</param>
    /// <param name="sourceControl">Исходный контрол.</param>
    /// <param name="includeSelf">Признак, нужно ли включать в поиск сам исходный контрол.</param>
    /// <returns>Найденный родительский контрол.</returns>
    public static DependencyObject GetParentControlFromLogicalTree(IEnumerable<Type> parentControlTypes, DependencyObject sourceControl, bool includeSelf)
    {
      if (sourceControl == null)
        return null;

      Func<DependencyObject, bool> isParentTypeMatched = control => parentControlTypes.Any(type => type.IsAssignableFrom(control.GetType()));

      DependencyObject parentControl = includeSelf ? sourceControl : LogicalTreeHelper.GetParent(sourceControl);
      while (parentControl != null && !isParentTypeMatched(parentControl))
        parentControl = LogicalTreeHelper.GetParent(parentControl);
      return parentControl;
    }

    /// <summary>
    /// Найти дочерний элемент определённого типа.
    /// </summary>
    /// <typeparam name="T">Тип искомого элемента.</typeparam>
    /// <param name="rootObject">Элемент в котором происходит поиск.</param>
    /// <returns>Список найденых дочерних элементов определённого типа.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject) where T : DependencyObject
    {
      return FindChildren<T>(rootObject,
        element => Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(element)).Select(i => VisualTreeHelper.GetChild(element, i)));
    }

    /// <summary>
    /// Найти дочерний элемент определённого типа с определенным именем.
    /// </summary>
    /// <typeparam name="T">Тип искомого элемента.</typeparam>
    /// <param name="rootObject">Элемент в котором происходит поиск.</param>
    /// <param name="name">Искомое имя.</param>
    /// <returns>Список найденых дочерних элементов определённого типа.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject, string name) where T : DependencyObject
    {
      return FindVisualChildren<T>(rootObject).OfType<FrameworkElement>().Where(c => c.Name == name).Cast<T>();
    }

    /// <summary>
    /// Найти дочерний элемент определённого типа в логическом дереве элементов формы.
    /// </summary>
    /// <typeparam name="T">Тип искомого элемента.</typeparam>
    /// <param name="rootObject">Элемент в котором происходит поиск.</param>
    /// <returns>Список найденых дочерних элементов определённого типа.</returns>
    public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject rootObject) where T : DependencyObject
    {
      return FindChildren<T>(rootObject,
        element => LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>());
    }

    /// <summary>
    /// Найти дочерний элемент определённого типа. Результат будет возвращен в порядке следования элементов в визуальном дереве.
    /// </summary>
    /// <typeparam name="T">Тип искомого элемента.</typeparam>
    /// <param name="rootObject">Элемент в котором происходит поиск.</param>
    /// <returns>Список найденых дочерних элементов определённого типа.</returns>
    public static IEnumerable<T> FindVisualChildrenOrderByVisualTree<T>(DependencyObject rootObject) where T : DependencyObject
    {
      if (rootObject != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(rootObject, i);

          if (child != null && child is T)
            yield return (T)child;

          foreach (T childOfChild in FindVisualChildrenOrderByVisualTree<T>(child))
            yield return childOfChild;
        }
      }
    }

    /// <summary>
    /// Найти дочерний элемент определённого типа в дереве элементов формы.
    /// </summary>
    /// <typeparam name="T">Тип искомого элемента.</typeparam>
    /// <param name="rootObject">Элемент в котором происходит поиск.</param>
    /// <param name="getChildren">Делегат для получения списка непосредсвенных дочерних элементов.</param>
    /// <returns>Список найденых дочерних элементов определённого типа.</returns>
    private static IEnumerable<T> FindChildren<T>(DependencyObject rootObject, Func<DependencyObject, IEnumerable<DependencyObject>> getChildren)
      where T : DependencyObject
    {
      var queue = new Queue<DependencyObject>();
      if (rootObject != null)
        queue.Enqueue(rootObject);
      while (queue.Count > 0)
      {
        var element = queue.Dequeue();
        if (element is T && !ReferenceEquals(element, rootObject))
          yield return (T)element;

        foreach (var child in getChildren(element))
          queue.Enqueue(child);
      }
    }

    /// <summary>
    /// Получить редактор по объекту, который является частью редактора.
    /// </summary>
    /// <param name="childElement">Объект, который является частью редактора.</param>
    /// <param name="parentControlTypes">Типы родительских элементов.</param>
    /// <returns>Редактор или null, если объект не является частью редактора.</returns>
    private static FrameworkElement GetEditorByChildElement(DependencyObject childElement, IEnumerable<Type> parentControlTypes)
    {
      // Не все контролы наследники от Visual (например Hyperlink),
      // поэтому поиск по визуальному дереву иногда не приносит результата, 
      // нужно искать по логическому.
      return (FrameworkElement)GetParentControlFromVisualTree(parentControlTypes, childElement, null, true) ??
        (FrameworkElement)GetParentControlFromLogicalTree(parentControlTypes, childElement, true);
    }

    #endregion
  }
}