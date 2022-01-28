using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Models
{
    public class MediaDto
    {
        public string Name      { get; set; }
        public string Path      { get; set; }
        public string Thumbnail { get; set; }

        public MediaDto(Media media)
        {
            Name      = media.Name;
            Path      = media.Path;
            Thumbnail = media.Thumbnail;
        }
    }
}
