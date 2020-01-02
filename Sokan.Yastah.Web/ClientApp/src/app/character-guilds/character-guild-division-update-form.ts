import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { combineLatest } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { FormOnDeletingHandler, FormOnResettingHandler, FormOnSavingHandler, IOperationError } from "../common/types";
import { SubscriberComponentBase } from "../subscriber-component-base";
import { AppValidators } from "../validators";

import { ICharacterGuildDivisionIdentityViewModel, ICharacterGuildDivisionUpdateModel } from "./models";


@Component({
    selector: "character-guild-division-update-form",
    templateUrl: "./character-guild-division-update-form.ts.html"
})
export class CharacterGuildDivisionUpdateForm
        extends SubscriberComponentBase {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._hasSaved = false;

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._otherDivisionIdentities && this._otherDivisionIdentities.map(x => x.name))
                    ])
            },
            {
                validators: () => (this._otherDivisionIdentities == null)
                    ? { "uninitialized": true }
                    : null
            });

        combineLatest(
                this._form.statusChanges,
                this._form.valueChanges)
            .pipe(takeUntil(this.destroying))
            .subscribe(() => {
                this._hasSaved = false;
                this._saveError = null;
            })
    }

    @Input("other-division-identities")
    public set otherDivisionIdentities(value: ICharacterGuildDivisionIdentityViewModel[] | null) {
        this._otherDivisionIdentities = value;
        this._form.updateValueAndValidity();
    }
    @Input("on-deleting")
    public set onDeleting(value: FormOnDeletingHandler | null) {
        this._onDeleting = value;
    }
    @Input("on-resetting")
    public set onResetting(value: FormOnResettingHandler<ICharacterGuildDivisionUpdateModel> | null) {
        this._onResetting = value;
        if (value != null) {
            this.reset(true);
        }
    }
    @Input("on-saving")
    public set onSaving(value: FormOnSavingHandler<ICharacterGuildDivisionUpdateModel> | null) {
        this._onSaving = value;
        this._hasSaved = false;
    }

    public get canDelete(): boolean {
        return this._form.enabled && (this._onDeleting != null);
    }
    public get canReset(): boolean {
        return this._form.enabled && (this._onResetting != null);
    }
    public get canSave(): boolean {
        return this._form.valid && this._form.dirty && !this._hasSaved && (this._onSaving != null);
    }
    public get form(): FormGroup {
        return this._form;
    }
    public get hasSaved(): boolean {
        return this._hasSaved;
    }
    public get saveError(): IOperationError | null {
        return this._saveError;
    }

    public async delete(): Promise<void> {
        this._form.disable();

        await this._onDeleting();
    }
    public async reset(isInit: boolean = false): Promise<void> {
        this._form.disable();

        let model = await this._onResetting(isInit);

        this._form.enable();
        this._form.reset(model);
    }
    public async save(): Promise<void> {
        this._form.disable();

        this._saveError = await this._onSaving(this._form.value);
        this._hasSaved = (this._saveError != null);

        this._form.enable();
    }

    private readonly _form: FormGroup;

    private _hasSaved: boolean;
    private _onDeleting: FormOnDeletingHandler | null;
    private _onResetting: FormOnResettingHandler<ICharacterGuildDivisionUpdateModel> | null;
    private _onSaving: FormOnSavingHandler<ICharacterGuildDivisionUpdateModel> | null;
    private _otherDivisionIdentities: ICharacterGuildDivisionIdentityViewModel[] | null;
    private _saveError: IOperationError | null;
}
