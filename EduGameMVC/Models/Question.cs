using System.ComponentModel.DataAnnotations;

namespace EduGameMVC.Models
{
    public class Question
    {
        public int Id { get; set; }

        // Null means shared content managed by an administrator.
        public string? OwnerId { get; set; }

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
        [RegularExpression("^[ABCD]$", ErrorMessage = "Choose A, B, C, or D.")]
        public string CorrectOption { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Easy|Medium|Hard)$", ErrorMessage = "Choose Easy, Medium, or Hard.")]
        public string Difficulty { get; set; } = string.Empty;
    }
}
