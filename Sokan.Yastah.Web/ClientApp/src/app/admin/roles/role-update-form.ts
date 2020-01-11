import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { FormGroupExtensions } from "../../common/form-group-extensions";
import { AppValidators } from "../../common/validators";

import { IPermissionCategoryDescriptionViewModel, IPermissionDescriptionViewModel, PermissionCategoryDescriptionViewModel } from "../permissions/models";

import { IRoleDetailViewModel, IRoleIdentityViewModel, IRoleUpdateModel } from "./models";


export interface IRoleUpdateFormModel {
    readonly name: string;
    readonly permissionMappings: {
        readonly [id: number]: boolean;
    };
}

export namespace RoleUpdateFormModel {

    export function fromDetail(
                detail: IRoleDetailViewModel,
                permissions: IPermissionDescriptionViewModel[]):
            IRoleUpdateFormModel {
        return {
            name: detail.name,
            permissionMappings: permissions
                .reduce(
                    (mappings, permission) => {
                        mappings[permission.id] = detail.grantedPermissionIds.includes(permission.id);
                        return mappings;
                    },
                    {})
        };
    }

    export function toUpdate(
                form: IRoleUpdateFormModel):
            IRoleUpdateModel {
        return {
            name: form.name,
            grantedPermissionIds: Object.keys(form.permissionMappings)
                .map(x => Number(x))
                .filter(id => form.permissionMappings[id])
        };
    }
}


@Component({
    selector: "role-update-form",
    templateUrl: "./role-update-form.ts.html"
})
export class RoleUpdateForm
        extends FormComponentBase<IRoleUpdateFormModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._formBuilder = formBuilder;

        this._permissionMappings = this._formBuilder.group({});
        this._form = this._formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._otherRoleIdentities && this._otherRoleIdentities.map(x => x.name))
                    ]),
                permissionMappings: this._permissionMappings
            },
            {
                validators: () => (this._otherRoleIdentities == null)
                    ? { "uninitialized": true }
                    : null
            });
    }

    @Input("other-role-identities")
    public set otherRoleIdentities(value: IRoleIdentityViewModel[] | null) {
        this._otherRoleIdentities = value;
        this._form.updateValueAndValidity();
    }
    @Input("permission-descriptions")
    public get permissionDescriptions(): IPermissionCategoryDescriptionViewModel[] | null {
        return this._permissionDescriptions;
    }
    public set permissionDescriptions(value: IPermissionCategoryDescriptionViewModel[] | null) {
        if (value != null) {
            PermissionCategoryDescriptionViewModel.mapPermissions(value)
                .forEach(permission => this.tryAddPermissionMappingControl(permission.id.toString()));
        }
        this._permissionMappings.updateValueAndValidity();

        this._permissionDescriptions = value;
    }

    public get form(): FormGroup {
        return this._form;
    }

    protected checkCanSave():
            boolean {
        return super.checkCanSave() && this._form.dirty;
    }
    protected loadModel(
                model: IRoleUpdateFormModel):
            void {
        Object.keys(model.permissionMappings)
            .forEach(permissionId => this.tryAddPermissionMappingControl(permissionId));
        super.loadModel(model);
    }

    private tryAddPermissionMappingControl(
                name: string):
            void {
        FormGroupExtensions.tryAddControl(this._permissionMappings, name, () => this._formBuilder.control(false));
    }

    private readonly _form: FormGroup;
    private readonly _formBuilder: FormBuilder;
    private readonly _permissionMappings: FormGroup;

    private _otherRoleIdentities: IRoleIdentityViewModel[] | null;
    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[] | null;
}
