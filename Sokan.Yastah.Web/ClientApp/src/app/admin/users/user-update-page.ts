import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, from, Observable } from "rxjs";
import { filter, map } from "rxjs/operators";

import { FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";
import { isNotNullOrUndefined } from "../../common/types";

import { IPermissionCategoryDescriptionViewModel, PermissionCategoryDescriptionViewModel } from "../permissions/models";
import { PermissionsService } from "../permissions/services";

import { IRoleIdentityViewModel } from "../roles/models";
import { RolesService } from "../roles/services";

import { UsersService } from "./services";
import { IUserUpdateFormModel, UserUpdateFormModel } from "./user-update-form";


@Component({
    selector: "user-update-page",
    templateUrl: "./user-update-page.ts.html",
    styleUrls: ["./user-update-page.ts.css"]
})
export class UserUpdatePage {

    public constructor(
            activatedRoute: ActivatedRoute,
            permissionsService: PermissionsService,
            rolesService: RolesService,
            usersService: UsersService) {

        let userId = activatedRoute.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._permissionDescriptions = permissionsService.observeDescriptions();

        this._roleIdentities = rolesService.observeIdentities();

        this._onResetting = combineLatest(
                userId,
                this._permissionDescriptions,
                this._roleIdentities)
            .pipe(map(([userId, permissionDescriptions, roleIdentities]) =>
                () => from(usersService.fetchDetail(userId))
                    .pipe(
                        filter(isNotNullOrUndefined),
                        map(detail => UserUpdateFormModel.fromDetail(
                            detail,
                            PermissionCategoryDescriptionViewModel.mapPermissions(permissionDescriptions)
                                .map(permission => permission.id),
                            roleIdentities
                                .map(role => role.id))))
                    .toPromise()));

        this._onSaving = userId
            .pipe(map(userId =>
                (model) => usersService.update(userId, UserUpdateFormModel.toUpdate(model))));
    }

    public get onResetting(): Observable<FormOnResettingHandler<IUserUpdateFormModel>> {
        return this._onResetting;
    }
    public get onSaving(): Observable<FormOnSavingHandler<IUserUpdateFormModel>> {
        return this._onSaving;
    }
    public get permissionDescriptions(): Observable<IPermissionCategoryDescriptionViewModel[]> {
        return this._permissionDescriptions;
    }
    public get roleIdentities(): Observable<IRoleIdentityViewModel[]> {
        return this._roleIdentities;
    }

    private readonly _onResetting: Observable<FormOnResettingHandler<IUserUpdateFormModel>>;
    private readonly _onSaving: Observable<FormOnSavingHandler<IUserUpdateFormModel>>;
    private readonly _permissionDescriptions: Observable<IPermissionCategoryDescriptionViewModel[]>;
    private readonly _roleIdentities: Observable<IRoleIdentityViewModel[]>;
}
