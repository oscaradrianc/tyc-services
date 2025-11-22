using FluentValidation;

namespace Tyc.ws.Features.Consentimientos.Validators;
internal class ConsentimientoRQValidator : AbstractValidator<CreateConsentimientoCommand>
{
    public ConsentimientoRQValidator()
    {
        RuleFor(x => x.Request.Nombres)
            .NotEmpty().WithMessage("El camponNombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Request.Apellidos)
                .NotEmpty().WithMessage("El campo apellido es obligatorio.")
                .MaximumLength(200).WithMessage("El apellido no puede exceder 200 caracteres.");

        RuleFor(x => x.Request.Identificacion)
                .NotEmpty().WithMessage("El campo identificación es obligatorio.")
                .MaximumLength(50).WithMessage("La identificación no puede exceder 50 caracteres.");

        RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("El campo email es obligatorio.")
                .MaximumLength(50).WithMessage("El email no puede exceder 50 caracteres.");

        RuleFor(x => x.Request.Telefono)
                .NotEmpty().WithMessage("El campo teléfono movil es obligatoria.")
                .MaximumLength(50).WithMessage("El teléfono no puede exceder 50 caracteres.");

        RuleFor(x => x.Request.Medio)
                .NotEmpty().WithMessage("El campo medio es obligatorio.")
                .MaximumLength(20).WithMessage("El medio no puede exceder 20 caracteres.");
    }
}
