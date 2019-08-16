import { Component, Input, OnDestroy } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";
import { Store } from "@ngrx/store";
import { BehaviorSubject, combineLatest, Subject, of } from "rxjs";
import { map, startWith, switchMap, takeUntil, tap } from "rxjs/operators";

import { IAppState } from "../../state";
import { ApiOperationError } from "../../api/api-operation-error";

import { LoadDescriptionsAction } from "../permissions/actions";
import { IPermissionCategoryDescriptionViewModel } from "../permissions/models";
import { buildPermissionMappingControls } from "../permissions/utils";

import { LoadIdentitiesAction } from "../roles/actions";
import { IRoleIdentityViewModel } from "../roles/models";
import { buildRoleMappingControls } from "../roles/utils";

import { userUpdateFormInitialState } from "./models";
import { UsersService } from "./service";
import { buildUserUpdateForm, extractUserUpdate } from "./utils";

@Component({
    selector: "user-update-form",
    templateUrl: "./user-update-form.ts.html",
    styleUrls: ["./user-update-form.ts.css"]
})
export class UserUpdateForm
        implements OnDestroy {

    public constructor(
            appState: Store<IAppState>,
            formBuilder: FormBuilder,
            usersService: UsersService) {

        this._appState = appState;
        this._destroying = new Subject<void>();
        this._resetRequested = new Subject<void>();
        this._userId = new BehaviorSubject<number | null>(null);
        this._usersService = usersService;

        this._form = formBuilder.group({
            id: formBuilder.control(null),
            permissionMappings: formBuilder.group({}),
            roleMappings: formBuilder.group({})
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
                buildPermissionMappingControls(x, this.form.controls.permissionMappings as FormGroup, formBuilder, "unmapped");
                this._permissionDescriptions = x;
            });

        appState.select(x => x.admin.roles.identities)
            .pipe(takeUntil(this._destroying))
            .subscribe(x => {
                buildRoleMappingControls(x, this.form.controls.roleMappings as FormGroup, formBuilder, false);
                this._roleIdentities = x;
            })

        combineLatest(this._userId, this._resetRequested)
            .pipe(
                switchMap(([id]) => (id == null)
                    ? of(null)
                    : this._usersService.getDetail(id).pipe(
                        map(d => buildUserUpdateForm(
                            d,
                            Object.keys(this._form.controls.permissionMappings.value)
                                .map(id => Number(id)),
                            Object.keys(this._form.controls.roleMappings.value)
                                .map(id => Number(id)))),
                        startWith(null))),
                takeUntil(this._destroying))
            .subscribe(x => {
                if (x == null) {
                    this._form.reset(userUpdateFormInitialState);
                    this._form.disable();
                }
                else {
                    this._form.enable();
                    this._form.reset(x);
                }
            });

        this._appState.dispatch(new LoadDescriptionsAction());
        this._appState.dispatch(new LoadIdentitiesAction());
        this.reset();
    }

    public get canReset(): boolean {
        return this._form.enabled;
    }

    public get canSave(): boolean {
        return this._form.valid && this._form.dirty && !this._hasSaved;
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

    public get roleIdentities(): IRoleIdentityViewModel[] {
        return this._roleIdentities;
    }

    public get saveError(): ApiOperationError {
        return this._saveError;
    }

    public get userId(): number {
        return this._userId.getValue();
    }
    @Input()
    public set userId(value: number) {
        this._userId.next(value);
    }

    public reset(): void {
        this._resetRequested.next();
    }

    public save(): void {
        this._form.disable();
        this._usersService.update(this._form.value.id, extractUserUpdate(this._form.value))
            .pipe(
                tap(() => this._form.enable()),
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

    private readonly _appState: Store<IAppState>;
    private readonly _destroying: Subject<void>;
    private readonly _form: FormGroup;
    private readonly _resetRequested: Subject<void>;
    private readonly _userId: BehaviorSubject<number | null>;
    private readonly _usersService: UsersService;

    private _hasSaved: boolean;
    private _permissionDescriptions: IPermissionCategoryDescriptionViewModel[];
    private _roleIdentities: IRoleIdentityViewModel[];
    private _saveError: ApiOperationError | null;
}
