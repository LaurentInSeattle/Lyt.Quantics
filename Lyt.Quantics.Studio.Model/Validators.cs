namespace Lyt.Quantics.Studio.Model;  

public static class Validators
{
    public class Name : AbstractValidator<string>
    {
        public Name()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("A name cannot be left empty.")
                .MinimumLength(4).WithMessage("This is too short, length should be at least 4.")
                .MaximumLength(128).WithMessage("This is too long, length should not exceed 128.");
        }
    }

    public class Description : AbstractValidator<string>
    {
        public Description()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("A description cannot be left empty.")
                .MinimumLength(12).WithMessage("This is too short, length should be at least 12.")
                .MaximumLength(1024).WithMessage("This is too long, length should not exceed 1024.");
        }
    }

}
