namespace YoutubeDownloader
{
  /// <summary>
  /// Сниппет (краткая информация) о видео.
  /// </summary>
  public class VideoSnippet
  {
    /// <summary>
    /// ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Имя видео.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Опубликовано.
    /// </summary>
    public string Published { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public VideoSnippet(string videoId, string name, string description, string published)
    {
      this.Id = videoId;
      this.Name = name;
      this.Description = description;
      this.Published = published;
    }
  }
}
