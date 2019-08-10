import { Component, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { Store } from "@ngrx/store";
import { combineLatest, Subject } from "rxjs";
import { takeUntil, tap } from "rxjs/operators";

import { IAppState } from "../../state";
import { ApiOperationError } from "../../api/api-operation-error";

import { LoadDescriptionsAction } from "../permissions/actions";
import { IPermissionCategoryDescriptionViewModel } from "../permissions/models";
import { buildPermissionMappingControls } from "../permissions/utils";

import { ReloadIdentitiesAction } from "./actions";
import { roleCreationFormInitialState } from "./models";
import { RolesService } from "./service";
import { makeRoleDuplicateNameValidator, extractRoleCreation } from "./utils";

@Component({
    templateUrl: "./role-creation-form.ts.html"
})
export class RoleCreationForm
        implements OnDestroy {

    public constructor(
            activatedRoute: ActivatedRoute,
            appState: Store<IAppState>,
            formBuilder: FormBuilder,
            rolesService: RolesService,
            router: Router) {

        this._activatedRoute = activatedRoute;
        this._appState = appState;
        this._destroying = new Subject<void>();
        this._rolesService = rolesService;
        this._router = router;

        this._form = formBuilder.group({
            name: formBuilder.control(null, Validators.required, makeRoleDuplicateNameValidator(this._appState)),
            permissionMappings: formBuilder.group({})
        });

        combineLatest(
                this._form.statusChanges,
                this._form.valueChanges)
            .pipe(takeUntil(this._destroying))
            .subscribe(x => {
                this._saveError = null;
            });

        appState.select(x => x.admin.permissions.descriptions)
            .pipe(takeUntil(this._destroying))
            .subscribe(x => {
                this._permissionDescriptions = x;
                buildPermissionMappingControls(x, this.form.controls.permissionMappings as FormGroup, formBuilder);
            });

        this._appState.dispatch(new LoadDescriptionsAction());
        this.reset();
    }

    public get canReset(): boolean {
        return this._form.enabled;
    }

    public get canSave(): boolean {
        return this._form.valid;
    }

    public get form(): FormGroup {
        return this._form;
    }

    public get permissionDescriptions(): IPermissionCategoryDescriptionViewModel[] {
        return this._permissionDescriptions;
    }

    public get saveError(): ApiOperationError | null {
        return this._saveError;
    }

    public reset(): void {
        this.form.reset(roleCreationFormInitialState);
    }

    public save(): void {
        this._form.disable();
        this._rolesService.create(extractRoleCreation(this._form.value))
            .pipe(
                tap(() => this._appState.dispatch(new ReloadIdentitiesAction())),
                takeUntil(this._destroying))
            .subscribe(
                roleId => {
                    this._saveError = null;
                    this._router.navigate([`../${roleId}`], { relativeTo: this._activatedRoute });
                },
                xhr => {
                    this._form.enable();
                    this._saveError = xhr.error;
                });
    }

    public ngOnDestroy(): void {
        this._destroying.next();
        this._destroying.complete();
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _appState: Store<IAppState>;
    private readonly _destroying: Subject<void>;
    private readonly _form: FormGroup;
    private readonly _rolesService: RolesService;
    private readonly _router: Router;

    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[];
    private _saveError: ApiOperationError | null;
}
