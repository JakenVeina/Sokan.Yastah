import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { map, take } from "rxjs/operators";

import { IAppState } from "../../state";

import { IRoleCreationFormModel, IRoleCreationModel, IRoleDetailViewModel, IRoleIdentityViewModel, IRoleUpdateFormModel, IRoleUpdateModel } from "./models";

export function buildRoleMappingControls(
    identities: IRoleIdentityViewModel[],
    roleMappings: FormGroup,
    formBuilder: FormBuilder,
    initialValue: any) {
    Object.keys(roleMappings.value)
        .forEach(x => roleMappings.removeControl(x));

    identities
        .forEach(x => roleMappings.addControl(x.id.toString(), formBuilder.control(initialValue)));
}

export function buildRoleUpdateForm(
            detail: IRoleDetailViewModel,
            permissionIds: number[]):
        IRoleUpdateFormModel {
    return {
        id: detail.id,
        name: detail.name,
        permissionMappings: Object.assign({}, ...permissionIds
            .map(id => ({
                [id]: detail.grantedPermissionIds.includes(id)
            })))
    };
}

export function extractRoleCreation(form: IRoleCreationFormModel): IRoleCreationModel {
    return {
        name: form.name,
        grantedPermissionIds: Object.keys(form.permissionMappings)
            .map(x => Number(x))
            .filter(id => form.permissionMappings[id])
    };
}

export function extractRoleUpdate(form: IRoleUpdateFormModel): IRoleUpdateModel {
    return {
        name: form.name,
        grantedPermissionIds: Object.keys(form.permissionMappings)
            .map(x => Number(x))
            .filter(id => form.permissionMappings[id])
    };
}

export function makeRoleDuplicateNameValidator(appState: Store<IAppState>): AsyncValidatorFn {
    return (nameControl: AbstractControl): Observable<{ duplicate: boolean } | null> => {
        let idControl = (nameControl.parent as FormGroup).controls.id;
        let id = idControl && idControl.value;
        let name = nameControl.value;

        return appState.select(x => x.admin.roles.identities).pipe(
            map(x => x.some(y => (y.id !== id) && (y.name === name))
                ? { duplicate: true }
                : null),
            take(1));
    };
}