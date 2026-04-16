namespace Lyt.Quantics.Studio.Utilities;  

public static class SharedValidators
{
    public static readonly FieldValidator<string> NameValidator =
        new(validator: new Validators.Name(), sourcePropertyName: "Name");

    public static readonly FieldValidator<string> DescriptionValidator =
        new(validator: new Validators.Description(), sourcePropertyName: "Description");
}
