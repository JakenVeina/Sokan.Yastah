import { AbstractControl, FormGroup } from "@angular/forms";


export namespace FormGroupExtensions {

    export function tryAddControl(
                group: FormGroup,
                name: string,
                control: () => AbstractControl):
            void {
        if (group.controls[name] == null) {
            group.addControl(name, control());
        }
    }
}
