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
                .Length(7).WithMessage("IMO must be 7 characters.");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(v => v.Mmsi)
                .NotEmpty().WithMessage("MMSI is required.")
                .Length(9).WithMessage("MMSI must be 9 characters.");
        }
    }
}
