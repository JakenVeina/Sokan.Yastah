import { ValidatorFn } from "@angular/forms";

export class AppValidators {
    public static notDuplicated(
                getExistingValues: () => string[] | null):
            ValidatorFn {
        return control => {
            let existingValues = getExistingValues();

            return (existingValues == null) || (existingValues.indexOf(control.value) === -1)
                ? null
                : { "duplicate": true };
        }
    }
}