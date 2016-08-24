using System;

namespace YoutubeDownloader
{
  /// <summary>
  /// Аргументы события на изменения прогресса.
  /// </summary>
  public class ProgressChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Значение прогресса.
    /// </summary>
    public int Value { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Новое значение прогресса.</param>
    public ProgressChangedEventArgs(int value)
    {
      this.Value = value;
    }
  }

  /// <summary>
  /// Аргументы события на изменение состояния загрузчика.
  /// </summary>
  public class StateChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Состояние.
    /// </summary>
    public DownloaderState Value { get; private set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Новое значение загрузчика.</param>
    public StateChangedEventArgs(DownloaderState value)
    {
      this.Value = value;
    }
  }
}
