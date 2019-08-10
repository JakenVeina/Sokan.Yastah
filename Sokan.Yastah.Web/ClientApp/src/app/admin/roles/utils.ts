import { AsyncValidatorFn, FormGroup, AbstractControl } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { map, take } from "rxjs/operators";

import { IAppState } from "../../state";

import { IRoleCreationFormModel, IRoleCreationModel, IRoleDetailViewModel, IRoleUpdateFormModel, IRoleUpdateModel } from "./models";

export function buildRoleUpdateForm(detail: IRoleDetailViewModel): IRoleUpdateFormModel {
    return {
        id: detail.id,
        name: detail.name,
        permissionMappings: Object.assign({}, ...detail.grantedPermissionIds
            .map(id => ({ [id]: true })))
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