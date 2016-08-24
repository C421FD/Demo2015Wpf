namespace YoutubeDownloader
{
  /// <summary>
  /// Константы, используемые в проекте.
  /// </summary>
  public class Constants
  {
    /// <summary>
    /// Публичный ключ для работы с Youtube API.
    /// </summary>
    public const string ApiKey = "AIzaSyBxDJeodQQQiQRSs86hseCMAJfGrXLiByE";

    /// <summary>
    /// Название тэга для поиска видео.
    /// </summary>
    public const string VideoElementKind = "youtube#video";

    /// <summary>
    /// Название тега для загрузки видео (а не каналов и пользователей).
    /// </summary>
    public const string SearchElementKind = "snippet";
  }
}
