namespace Lyt.Quantics.Studio.Utilities;  

public static class SharedValidators
{
    public static readonly FieldValidatorParameters<string> NameValidatorParameters =
        new(
            Validator: new Validators.Name(),
            SourcePropertyName: "Name",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidatorParameters<string> DescriptionValidatorParameters =
        new(
            Validator: new Validators.Description(),
            SourcePropertyName: "Description",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> NameValidator =
        new(NameValidatorParameters);

    public static readonly FieldValidator<string> DescriptionValidator =
        new(DescriptionValidatorParameters);

}
