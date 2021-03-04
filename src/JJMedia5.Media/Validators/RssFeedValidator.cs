using FluentValidation;
using JJMedia5.Core.Entities;
using System;

namespace JJMedia5.Media.Validators {

    public class RssFeedValidator : AbstractValidator<RssFeed> {

        public RssFeedValidator() {
            RuleFor(x => x.Info).NotEmpty();
            RuleFor(x => x.Url).NotEmpty().MinimumLength(10);
            RuleFor(x => x.StartDate).NotNull().LessThan(DateTimeOffset.UtcNow.AddMonths(5))
                .WithMessage("Cannot use a start date that exceeds 5 months in the future.");
        }
    }
}