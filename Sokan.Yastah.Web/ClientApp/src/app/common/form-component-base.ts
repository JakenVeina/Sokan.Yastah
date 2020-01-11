import { Input, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";

import { filter, map, takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "./subscriber-component-base";
import { IOperationError } from "./types";


export interface FormOnDeletingHandler {
    (): Promise<void>;
}
export interface FormOnResettingHandler<TModel> {
    (isInit: boolean): Promise<TModel>;
}
export interface FormOnSavingHandler<TModel> {
    (model: TModel): Promise<IOperationError | null>;
}


export abstract class FormComponentBase<TModel>
        extends SubscriberComponentBase
        implements OnInit {

    constructor() {
        super();

        this._onDeleting = null;
        this._hasNgInit = false;
        this._hasSaved = false;
        this._onResetting = null;
        this._onSaving = null;
        this._saveError = null;
    }

    @Input("on-deleting")
    public set onDeleting(value: FormOnDeletingHandler | null) {
        this._onDeleting = value;
    }
    @Input("on-resetting")
    public set onResetting(value: FormOnResettingHandler<TModel> | null) {
        this._onResetting = value;

        // If we haven't fully initted yet, don't do it here. Do it below in ngOnInit.
        // Only do it here if this was delayed until after ngOnInit, or if this is being changed
        if (this._hasNgInit && (value != null)) {
            this.resetInternal(true);
        }
    }
    @Input("on-saving")
    public set onSaving(value: FormOnSavingHandler<TModel> | null) {
        this._onSaving = value;
    }

    public get canDelete(): boolean {
        return this.form.enabled && (this._onDeleting != null);
    }
    public get canReset(): boolean {
        return this.form.enabled && (this._onResetting != null);
    }
    public get canSave(): boolean {
        return this.checkCanSave();
    }
    public abstract get form(): FormGroup;
    public get hasSaved(): boolean {
        return this._hasSaved;
    }
    public get saveError(): IOperationError | null {
        return this._saveError;
    }

    public async delete(): Promise<void> {
        this.form.disable();

        await this._onDeleting!();
    }
    public reset(): Promise<void> {
        return this.resetInternal(false);
    }
    public async save(): Promise<void> {
        this.form.disable();

        let error = await this._onSaving!(this.extractModel());

        this.form.enable();
        this.form.markAsPristine();

        this._hasSaved = (error == null);
        this._saveError = error;
    }

    public ngOnInit(): void {
        this.form.statusChanges
            .pipe(
                map(() => this.form.dirty),
                filter(isDirty => isDirty),
                takeUntil(this.destroying))
            .subscribe(() => {
                this._hasSaved = false;
                this._saveError = null;
            });

        if (this._onResetting != null) {
            this.resetInternal(true);
        }

        this._hasNgInit = true;
    }

    protected checkCanSave():
            boolean {
        return this.form.valid && (this._onSaving != null) && !this._hasSaved;
    }
    protected extractModel():
            TModel {
        return this.form.value;
    }
    protected loadModel(
                model: TModel):
            void {
        this.form.reset(model);
    }

    private async resetInternal(isInit: boolean): Promise<void> {
        this.form.disable();

        let model = await this._onResetting!(isInit);

        this.form.enable();
        this.loadModel(model);

        this._hasSaved = false;
        this._saveError = null;
    }

    private _onDeleting: FormOnDeletingHandler | null;
    private _hasNgInit: boolean;
    private _hasSaved: boolean;
    private _onResetting: FormOnResettingHandler<TModel> | null;
    private _onSaving: FormOnSavingHandler<TModel> | null;
    private _saveError: IOperationError | null;
}
