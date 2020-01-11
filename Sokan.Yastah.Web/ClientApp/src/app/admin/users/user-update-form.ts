import { Component, Input } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { FormGroupExtensions } from "../../common/form-group-extensions";
import { INativeHashTable } from "../../common/types";

import { IPermissionCategoryDescriptionViewModel, PermissionCategoryDescriptionViewModel } from "../permissions/models";

import { IRoleIdentityViewModel } from "../roles/models";

import { IUserDetailViewModel, IUserUpdateModel } from "./models";


export type UserPermissionMapping
    = "granted" | "unmapped" | "denied"

export interface IUserUpdateFormModel {
    readonly permissionMappings: {
        readonly [id: number]: UserPermissionMapping;
    };
    readonly roleMappings: {
        readonly [id: number]: boolean;
    };
}

export namespace UserUpdateFormModel {

    export function fromDetail(
                detail: IUserDetailViewModel,
                permissionIds: number[],
                roleIds: number[]):
            IUserUpdateFormModel {
        return {
            permissionMappings: permissionIds
                .reduce(
                    (mappings, id) => {
                        mappings[id] = detail.grantedPermissionIds.includes(id) ? "granted"
                            : detail.deniedPermissionIds.includes(id) ? "denied"
                            : "unmapped";
                        return mappings;
                    },
                    <INativeHashTable<UserPermissionMapping>>{}),
            roleMappings: roleIds
                .reduce(
                    (mappings, id) => {
                        mappings[id] = detail.assignedRoleIds.includes(id);
                        return mappings;
                    },
                    <INativeHashTable<boolean>>{})
        };
    }

    export function toUpdate(
                form: IUserUpdateFormModel):
            IUserUpdateModel {
        return {
            grantedPermissionIds: Object.keys(form.permissionMappings)
                .map(x => Number(x))
                .filter(id => form.permissionMappings[id] === "granted"),
            deniedPermissionIds: Object.keys(form.permissionMappings)
                .map(x => Number(x))
                .filter(id => form.permissionMappings[id] === "denied"),
            assignedRoleIds: Object.keys(form.roleMappings)
                .map(x => Number(x))
                .filter(id => form.roleMappings[id])
        };
    }
}


@Component({
    selector: "user-update-form",
    templateUrl: "./user-update-form.ts.html",
    styleUrls: ["./user-update-form.ts.css"]
})
export class UserUpdateForm
        extends FormComponentBase<IUserUpdateFormModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._formBuilder = formBuilder;

        this._permissionDescriptions = null;
        this._roleIdentities = null;

        this._permissionMappings = this._formBuilder.group({});
        this._roleMappings = this._formBuilder.group({});

        this._form = formBuilder.group(
            {
                permissionMappings: this._permissionMappings,
                roleMappings: this._roleMappings
            });
    }

    @Input("role-identities")
    public get roleIdentities(): IRoleIdentityViewModel[] | null {
        return this._roleIdentities;
    }
    public set roleIdentities(value: IRoleIdentityViewModel[] | null) {
        if (value != null) {
            value.forEach(identity => this.tryAddRoleMappingControl(identity.id.toString()));
        }
        this._roleMappings.updateValueAndValidity();

        this._roleIdentities = value;
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
                model: IUserUpdateFormModel):
            void {
        Object.keys(model.permissionMappings)
            .forEach(permissionId => this.tryAddPermissionMappingControl(permissionId.toString()));
        Object.keys(model.roleMappings)
            .forEach(roleId => this.tryAddRoleMappingControl(roleId.toString()));

        super.loadModel(model);
    }

    private tryAddPermissionMappingControl(
                name: string):
            void {
        FormGroupExtensions.tryAddControl(this._permissionMappings, name, () => this._formBuilder.control("unmapped"));
    }
    private tryAddRoleMappingControl(
                name: string):
            void {
        FormGroupExtensions.tryAddControl(this._roleMappings, name, () => this._formBuilder.control(false));
    }

    private readonly _form: FormGroup;
    private readonly _formBuilder: FormBuilder;
    private readonly _permissionMappings: FormGroup;
    private readonly _roleMappings: FormGroup;

    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[] | null;
    private _roleIdentities: IRoleIdentityViewModel[] | null;
}
