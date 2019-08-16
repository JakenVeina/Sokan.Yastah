import { FormGroup, FormBuilder } from "@angular/forms";

import { IPermissionCategoryDescriptionViewModel } from "./models";

export function buildPermissionMappingControls(
        definitions: IPermissionCategoryDescriptionViewModel[],
        permissionMappings: FormGroup,
        formBuilder: FormBuilder,
        initialValue: any) {
    Object.keys(permissionMappings.value)
        .forEach(x => permissionMappings.removeControl(x));

    definitions.map(x => x.permissions)
        .reduce((x, y) => x.concat(y), [])
        .forEach(x => permissionMappings.addControl(x.id.toString(), formBuilder.control(initialValue)));
}
