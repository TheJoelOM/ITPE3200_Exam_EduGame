using System.ComponentModel.DataAnnotations;

namespace GamificationMVC.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectOption { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Difficulty { get; set; } = string.Empty;
    }
}