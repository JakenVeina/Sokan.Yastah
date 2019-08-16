import { Component, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { combineLatest, Subject } from "rxjs";
import { map, switchMap, takeUntil, tap, startWith } from "rxjs/operators";

import { IAppState } from "../../state";
import { ApiOperationError } from "../../api/api-operation-error";

import { LoadDescriptionsAction } from "../permissions/actions";
import { IPermissionCategoryDescriptionViewModel } from "../permissions/models";
import { buildPermissionMappingControls } from "../permissions/utils";

import { ReloadIdentitiesAction } from "./actions";
import { roleUpdateFormInitialState } from "./models";
import { RolesService } from "./service";
import { buildRoleUpdateForm, extractRoleUpdate, makeRoleDuplicateNameValidator } from "./utils";

@Component({
    selector: "role-update-form",
    templateUrl: "./role-update-form.ts.html"
})
export class RoleUpdateForm
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
        this._resetRequested = new Subject<void>();
        this._rolesService = rolesService;
        this._router = router;

        this._form = formBuilder.group({
            id: formBuilder.control(null),
            name: formBuilder.control(null, Validators.required, makeRoleDuplicateNameValidator(this._appState)),
            permissionMappings: formBuilder.group({})
        });

        combineLatest(
                this._form.statusChanges,
                this._form.valueChanges)
            .pipe(takeUntil(this._destroying))
            .subscribe(() => {
                this._hasSaved = false;
                this._saveError = null;
            })

        appState.select(x => x.admin.permissions.descriptions)
            .pipe(takeUntil(this._destroying))
            .subscribe(x => {
                buildPermissionMappingControls(x, this.form.controls.permissionMappings as FormGroup, formBuilder, false);
                this._permissionDescriptions = x;
            });

        combineLatest(activatedRoute.paramMap, this._resetRequested)
            .pipe(
                map(([x]) => Number(x.get("id"))),
                switchMap(id => this._rolesService.getDetail(id).pipe(
                    map(d => buildRoleUpdateForm(
                        d,
                        Object.keys(this._form.controls.permissionMappings.value)
                            .map(id => Number(id)))),
                    startWith(null))),
                takeUntil(this._destroying))
            .subscribe(x => {
                if (x == null) {
                    this.form.reset(roleUpdateFormInitialState);
                    this.form.disable();
                }
                else {
                    this.form.enable();
                    this.form.reset(x);
                }
            });

        this._appState.dispatch(new LoadDescriptionsAction());
        this.reset();
    }

    public get canDelete(): boolean {
        return this._form.enabled;
    }

    public get canReset(): boolean {
        return this._form.enabled;
    }

    public get canSave(): boolean {
        return this._form.valid && this._form.dirty && !this._hasSaved;
    }

    public get deleteError(): ApiOperationError {
        return this._deleteError;
    }

    public get form(): FormGroup {
        return this._form;
    }

    public get hasSaved(): boolean {
        return this._hasSaved;
    }

    public get permissionDescriptions(): IPermissionCategoryDescriptionViewModel[] {
        return this._permissionDescriptions;
    }

    public get saveError(): ApiOperationError | null {
        return this._saveError;
    }

    public delete(): void {
        this._form.disable();
        this._rolesService.delete(this._form.controls.id.value)
            .pipe(
                tap(() => this._appState.dispatch(new ReloadIdentitiesAction())),
                takeUntil(this._destroying))
            .subscribe(
                () => {
                    this._deleteError = null;
                    this._router.navigate(["../"], { relativeTo: this._activatedRoute });
                },
                xhr => {
                    this._deleteError = xhr.error;
                    this._form.enable();
                });
    }

    public reset(): void {
        this._resetRequested.next();
    }

    public save(): void {
        this._form.disable();
        this._rolesService.update(this._form.value.id, extractRoleUpdate(this._form.value))
            .pipe(
                tap(() => {
                    this._appState.dispatch(new ReloadIdentitiesAction());
                    this._form.enable();
                }),
                takeUntil(this._destroying))
            .subscribe(
                () => {
                    this._form.enable();
                    this._hasSaved = true;
                    this._saveError = null;
                },
                xhr => {
                    this._form.enable();
                    this._saveError = xhr.error;
                });
    }

    public ngOnDestroy(): void {
        this._destroying.next();
        this._destroying.complete();
        this._resetRequested.complete();
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _appState: Store<IAppState>;
    private readonly _destroying: Subject<void>;
    private readonly _form: FormGroup;
    private readonly _resetRequested: Subject<void>;
    private readonly _rolesService: RolesService;
    private readonly _router: Router;

    private _deleteError: ApiOperationError | null;
    private _hasSaved: boolean;
    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[];
    private _saveError: ApiOperationError | null;
}
