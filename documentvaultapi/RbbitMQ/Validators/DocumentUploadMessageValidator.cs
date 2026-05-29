using documentvaultapi.RbbitMQ.Models.MQueue;
using FluentValidation;

public class DocumentUploadMessageValidator : AbstractValidator<DocumentUploadMessageDTO>
{
    public DocumentUploadMessageValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("MessageId is required");

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("FileContent is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("ContentType is required");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("FileSize must be greater than 0");

        RuleFor(x => x.ApplicationId)
            .GreaterThan(0)
            .WithMessage("ApplicationId must be greater than 0");
    }
}
