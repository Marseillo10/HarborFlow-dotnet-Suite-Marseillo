using FluentValidation;
using HarborFlow.Core.Models;

namespace HarborFlow.Wpf.Validators
{
    public class VesselValidator : AbstractValidator<Vessel>
    {
        public VesselValidator()
        {
            RuleFor(v => v.IMO)
                .NotEmpty().WithMessage("IMO is required.")
                .Length(7).WithMessage("IMO must be 7 characters.")
                .Matches("^[0-9]*$").WithMessage("IMO must contain only digits.");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(v => v.Mmsi)
                .NotEmpty().WithMessage("MMSI is required.")
                .Length(9).WithMessage("MMSI must be 9 characters.")
                .Matches("^[0-9]*$").WithMessage("MMSI must contain only digits.");

            RuleFor(v => v.FlagState)
                .NotEmpty().WithMessage("Flag State is required.");

            RuleFor(v => v.LengthOverall)
                .GreaterThan(0).WithMessage("Length Overall must be greater than 0.");

            RuleFor(v => v.Beam)
                .GreaterThan(0).WithMessage("Beam must be greater than 0.");

            RuleFor(v => v.GrossTonnage)
                .GreaterThan(0).WithMessage("Gross Tonnage must be greater than 0.");
        }
    }
}
