import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


const integerTester
    = /^[-]?[0-9]*$/;

export namespace AppValidators {

    export function notDuplicated(
                getExistingValues: () => string[] | null):
            ValidatorFn {
        return control => {
            let existingValues = getExistingValues();

            return (existingValues == null) || (existingValues.indexOf(control.value) === -1)
                ? null
                : { "duplicate": true };
        }
    }

    export function integer(
                control: AbstractControl):
            ValidationErrors | null {
        return integerTester.test(control.value)
            ? null
            : { "integer" : true };
    }
}
