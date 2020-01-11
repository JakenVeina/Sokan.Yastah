import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";
import { map } from "rxjs/operators";

import { FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";

import { IPermissionCategoryDescriptionViewModel, PermissionCategoryDescriptionViewModel } from "../permissions/models";
import { PermissionsService } from "../permissions/services";

import { IRoleIdentityViewModel } from "./models";
import { IRoleCreationFormModel, RoleCreationFormModel } from "./role-creation-form";
import { RolesService } from "./services";


@Component({
    selector: "role-creation-page",
    templateUrl: "./role-creation-page.ts.html",
    styleUrls: ["./role-creation-page.ts.css"]
})
export class RoleCreationPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            permissionsService: PermissionsService,
            rolesService: RolesService,
            router: Router) {

        this._permissionDescriptions = permissionsService.observeDescriptions();

        this._onResetting = this._permissionDescriptions
            .pipe(map(categories =>
                () => Promise.resolve(RoleCreationFormModel.empty(
                    PermissionCategoryDescriptionViewModel.mapPermissions(categories)))));

        this._onSaving = async (model) => {
            let result = await rolesService.create(RoleCreationFormModel.toCreation(model));

            if (result.roleId != null) {
                router.navigate([`../${result.roleId}`], { relativeTo: activatedRoute })
            }

            return result.error || null;
        };

        this._roleIdentities = rolesService.observeIdentities();
    }

    public get onResetting(): Observable<FormOnResettingHandler<IRoleCreationFormModel>> {
        return this._onResetting;
    }
    public get onSaving(): FormOnSavingHandler<IRoleCreationFormModel> {
        return this._onSaving;
    }
    public get permissionDescriptions(): Observable<IPermissionCategoryDescriptionViewModel[]> {
        return this._permissionDescriptions;
    }
    public get roleIdentities(): Observable<IRoleIdentityViewModel[]> {
        return this._roleIdentities;
    }

    private readonly _onResetting: Observable<FormOnResettingHandler<IRoleCreationFormModel>>;
    private readonly _onSaving: FormOnSavingHandler<IRoleCreationFormModel>;
    private readonly _permissionDescriptions: Observable<IPermissionCategoryDescriptionViewModel[]>;
    private readonly _roleIdentities: Observable<IRoleIdentityViewModel[]>;
}
