import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { combineLatest } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { FormOnSavingHandler, IOperationError } from "../common/types";
import { SubscriberComponentBase } from "../subscriber-component-base";
import { AppValidators } from "../validators";

import { ICharacterGuildDivisionCreationModel, ICharacterGuildDivisionIdentityViewModel } from "./models";


@Component({
    selector: "character-guild-division-creation-form",
    templateUrl: "./character-guild-division-creation-form.ts.html"
})
export class CharacterGuildDivisionCreationForm
        extends SubscriberComponentBase {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._divisionIdentities && this._divisionIdentities.map(x => x.name))
                    ])
            },
            {
                validators: () => (this._divisionIdentities == null)
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
            });

        this.reset();
    }

    @Input("division-identities")
    public set divisionIdentities(value: ICharacterGuildDivisionIdentityViewModel[] | null) {
        this._divisionIdentities = value;
    }
    @Input("on-saving")
    public set onSaving(value: FormOnSavingHandler<ICharacterGuildDivisionCreationModel>) {
        this._onSaving = value;
    }

    public get canReset(): boolean {
        return this._form.enabled;
    }
    public get canSave(): boolean {
        return this._form.valid && (this._onSaving != null);
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

    public reset(): void {
        this.form.reset({
            name: "New Division"
        });
    }
    public async save(): Promise<void> {
        this._form.disable();

        this._saveError = await this._onSaving(this._form.value);
        (this._saveError == null)
            ? this._hasSaved = true
            : this._form.enable();
    }

    private readonly _form: FormGroup;

    private _divisionIdentities: ICharacterGuildDivisionIdentityViewModel[] | null;
    private _hasSaved: boolean;
    private _onSaving: FormOnSavingHandler<ICharacterGuildDivisionCreationModel> | null;
    private _saveError: IOperationError | null;
}
