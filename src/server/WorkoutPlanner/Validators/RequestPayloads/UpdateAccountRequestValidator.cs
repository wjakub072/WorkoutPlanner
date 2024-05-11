using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using WorkoutPlanner.Models.Requests;

namespace ProfitNest.Api.Validators.RequestPayloads;

public class UpdateAccountRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(request => request.Email)
            .EmailAddress()
            .When(request => !string.IsNullOrEmpty(request.Email))
            .WithMessage("Invalid email format.");

        When(request => !string.IsNullOrEmpty(request.Name), () => {
            RuleFor(request => request.Name)
            .MinimumLength(4).WithMessage("FirstName be between 4 and 26 characters long.")
            .MaximumLength(26).WithMessage("FirstName be between 4 and 26 characters long.");
        });
            
        When(request => !string.IsNullOrEmpty(request.SurName), () => {
            RuleFor(request => request.SurName)
            .MinimumLength(2).WithMessage("SurName be between 2 and 48 characters long.")
            .MaximumLength(48).WithMessage("SurName be between 2 and 48 characters long.");
        });

        When(request => !string.IsNullOrEmpty(request.Email), () => {
            RuleFor(request => request.Email)
            .EmailAddress().WithMessage("Invalid email format.");
        });

       When(request => request.Age is not null, () => {
            RuleFor(request => request.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0.");
        });

        When(request => request.Weight is not null, () => {
            RuleFor(request => request.Weight!)
            .Must(BeAValidDecimal)
            .WithMessage("Weight must be valid decimal.");
        });
        
        When(request => request.Height is not null, () => {
            RuleFor(request => request.Height!)
            .Must(BeAValidDecimal)
            .WithMessage("Height must be valid decimal.");
        });
    }

    private bool BeAValidDecimal(string value)
    {
        var regex = new Regex(@"^[+-]?([0-9]+\.?[0-9]*|\.[0-9]+)$");
        return regex.IsMatch(value.ToString());
    }
}
