using documentvaultapi.RbbitMQ.Models.MQueue.FromUM;
using FluentValidation;

public class  ApplicationMapValidator : AbstractValidator<ConsumeApplicationMapDTO>
{
    public ApplicationMapValidator()
    {
        //RuleFor(x => x.OrderId)
        //    .NotEmpty()
        //    .WithMessage("OrderId is required");

        //RuleFor(x => x.Amount)
        //    .GreaterThan(0)
        //    .WithMessage("Amount must be greater than 0");

        //RuleFor(x => x.CustomerEmail)
        //    .NotEmpty()
        //    .EmailAddress()
        //    .WithMessage("A valid customer email is required");
    }
}