import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { FormGroupExtensions } from "../../common/form-group-extensions";
import { INativeHashTable } from "../../common/types";
import { AppValidators } from "../../common/validators";

import { IPermissionCategoryDescriptionViewModel, PermissionCategoryDescriptionViewModel, IPermissionDescriptionViewModel } from "../permissions/models";

import { IRoleCreationModel, IRoleIdentityViewModel } from "./models";


export interface IRoleCreationFormModel {
    readonly name: string;
    readonly permissionMappings: {
        readonly [id: number]: boolean;
    };
}

export namespace RoleCreationFormModel {

    export function empty(
                permissions: IPermissionDescriptionViewModel[]):
            IRoleCreationFormModel {
        return {
            name: "New Role",
            permissionMappings: permissions
                .reduce(
                    (mappings, permission) => {
                        mappings[permission.id] = false;
                        return mappings;
                    },
                    <INativeHashTable<boolean>>{})
        };
    }

    export function toCreation(
                form: IRoleCreationFormModel):
            IRoleCreationModel {
        return {
            name: form.name,
            grantedPermissionIds: Object.keys(form.permissionMappings)
                .map(x => Number(x))
                .filter(id => form.permissionMappings[id])
        };
    }
}


@Component({
    selector: "role-creation-form",
    templateUrl: "./role-creation-form.ts.html"
})
export class RoleCreationForm
        extends FormComponentBase<IRoleCreationFormModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._formBuilder = formBuilder;

        this._permissionDescriptions = null;
        this._roleIdentities = null;

        this._permissionMappings = this._formBuilder.group({});
        this._form = this._formBuilder.group(
            {
                name: this._formBuilder.control(
                    null,
                    [
                        Validators.required, 
                        AppValidators.notDuplicated(() => this._roleIdentities && this._roleIdentities.map(x => x.name))
                    ]),
                permissionMappings: this._permissionMappings
            },
            {
                validators: () => (this._roleIdentities == null)
                    ? { "uninitialized": true }
                    : null
            });
    }

    @Input("role-identities")
    public set roleIdentities(value: IRoleIdentityViewModel[] | null) {
        this._roleIdentities = value;
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

    protected loadModel(
                model: IRoleCreationFormModel):
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

    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[] | null;
    private _roleIdentities: IRoleIdentityViewModel[] | null;
}
