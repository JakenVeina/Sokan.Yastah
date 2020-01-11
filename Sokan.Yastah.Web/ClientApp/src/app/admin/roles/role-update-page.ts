import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, from, Observable } from "rxjs";
import { filter, map, switchMap, takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../../common/subscriber-component-base";
import { FormOnDeletingHandler, FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";

import { IPermissionCategoryDescriptionViewModel, PermissionCategoryDescriptionViewModel } from "../permissions/models";
import { PermissionsService } from "../permissions/services";

import { IRoleIdentityViewModel, IRoleUpdateModel } from "./models";
import { IRoleUpdateFormModel, RoleUpdateFormModel } from "./role-update-form";
import { RolesService } from "./services";


@Component({
    selector: "role-update-page",
    templateUrl: "./role-update-page.ts.html",
    styleUrls: ["./role-update-page.ts.css"]
})
export class RoleUpdatePage
        extends SubscriberComponentBase {

    public constructor(
            activatedRoute: ActivatedRoute,
            permissionsService: PermissionsService,
            rolesService: RolesService,
            router: Router) {
        super();

        let roleId = activatedRoute.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._otherRoleIdentities = combineLatest(
                roleId,
                rolesService.observeIdentities()
                    .pipe(filter(identities => identities != null)))
            .pipe(map(([roleId, identities]) => identities.filter(identity => identity.id !== roleId)));

        this._onDeleting = roleId
            .pipe(map(roleId =>
                () => rolesService.delete(roleId)));

        this._permissionDescriptions = permissionsService.observeDescriptions();

        this._onResetting = combineLatest(
                roleId,
                this._permissionDescriptions)
            .pipe(map(([roleId, permissionDescriptions]) =>
                () => from(rolesService.fetchDetail(roleId))
                    .pipe(
                        filter(detail => detail != null),
                        map(detail => RoleUpdateFormModel.fromDetail(
                            detail,
                            PermissionCategoryDescriptionViewModel.mapPermissions(permissionDescriptions))))
                    .toPromise()));

        this._onSaving = roleId
            .pipe(map(roleId =>
                (model) => rolesService.update(roleId, RoleUpdateFormModel.toUpdate(model))));

        roleId
            .pipe(
                switchMap(roleId => rolesService.onDeleted(roleId)),
                takeUntil(this.destroying))
            .subscribe(() => router.navigate(["../"], { relativeTo: activatedRoute }));
    }

    public get otherRoleIdentities(): Observable<IRoleIdentityViewModel[]> {
        return this._otherRoleIdentities;
    }
    public get onDeleting(): Observable<FormOnDeletingHandler> {
        return this._onDeleting;
    }
    public get onResetting(): Observable<FormOnResettingHandler<IRoleUpdateFormModel>> {
        return this._onResetting;
    }
    public get onSaving(): Observable<FormOnSavingHandler<IRoleUpdateFormModel>> {
        return this._onSaving;
    }
    public get permissionDescriptions(): Observable<IPermissionCategoryDescriptionViewModel[]> {
        return this._permissionDescriptions;
    }

    private readonly _otherRoleIdentities: Observable<IRoleIdentityViewModel[]>;
    private readonly _onDeleting: Observable<FormOnDeletingHandler>;
    private readonly _onResetting: Observable<FormOnResettingHandler<IRoleUpdateFormModel>>;
    private readonly _onSaving: Observable<FormOnSavingHandler<IRoleUpdateFormModel>>;
    private readonly _permissionDescriptions: Observable<IPermissionCategoryDescriptionViewModel[]>;
}
