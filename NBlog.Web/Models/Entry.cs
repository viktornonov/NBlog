using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NBlog.Web.Models
{
    public class Entry
    {
        public string Slug { get; set; }

        [DisplayName("Slug")]
        [Required(ErrorMessage = "Please supply a slug for this post")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "That's not a valid slug. Only letters, numbers and hypens are allowed.")]
        public string NewSlug { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please enter the date this post was created.")]
        public string Date { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please enter the title of this post.")]
        public string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please enter some content.")]
        public string Markdown { get; set; }

        [DisplayName("Published")]
        public bool IsPublished { get; set; }

        [DisplayName("Enable code syntax highlighting")]
        public bool IsCodePrettified { get; set; }
    }
}