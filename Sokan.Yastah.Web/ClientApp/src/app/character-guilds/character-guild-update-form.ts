import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { combineLatest } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../subscriber-component-base";
import { AppValidators } from "../validators";

import { ApiOperationError } from "../api/api-operation-error";

import { CharacterGuildModelConverter, ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-update-form",
    templateUrl: "./character-guild-update-form.ts.html"
})
export class CharacterGuildUpdateForm
        extends SubscriberComponentBase {

    public constructor(
            characterGuildsService: CharacterGuildsService,
            formBuilder: FormBuilder) {
        super();

        this._deleted = new EventEmitter();
        this._resetting = new EventEmitter();

        this._characterGuildsService = characterGuildsService;

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._guildIdentities && this._guildIdentities
                            .filter(x => x.id !== this._guildId)
                            .map(x => x.name))
                    ])
            },
            {
                validators: () => (this._guildIdentities == null)
                    ? { "uninitialized": true }
                    : null
            }
        );

        combineLatest(
                this._form.statusChanges,
                this._form.valueChanges)
            .pipe(takeUntil(this.destroying))
            .subscribe(() => {
                this._hasSaved = false;
                this._saveError = null;
            })
    }

    @Input("guild-identity")
    public set guildIdentity(value: ICharacterGuildIdentityViewModel | null) {
        if (value == null) {
            this._form.reset(CharacterGuildModelConverter.toUpdate(null));
            this._form.disable();

            this._guildId = null;
        }
        else {
            this._guildId = value.id;

            this._form.enable();
            this._form.reset(CharacterGuildModelConverter.toUpdate(value));
        }
    }
    @Input("guild-identities")
    public set guildIdentities(value: ICharacterGuildIdentityViewModel[] | null) {
        this._guildIdentities = value;
        this._form.updateValueAndValidity();
    }

    @Output()
    public get deleted(): EventEmitter<void> {
        return this._deleted;
    }
    @Output()
    public get resetting(): EventEmitter<void> {
        return this._resetting;
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
    public get saveError(): ApiOperationError | null {
        return this._saveError;
    }

    public delete(): void {
        this._form.disable();
        this._characterGuildsService.delete(this._guildId)
            .pipe(takeUntil(this.destroying))
            .subscribe(
                () => {
                    this._deleteError = null;
                    this._deleted.emit();
                },
                xhr => {
                    this._deleteError = xhr.error;
                    this._form.enable();
                });
    }

    public reset(): void {
        this._resetting.emit();
    }

    public save(): void {
        this._form.disable();

        this._characterGuildsService.update(this._guildId, this._form.value)
            .pipe(takeUntil(this.destroying))
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

    private readonly _characterGuildsService: CharacterGuildsService;
    private readonly _deleted: EventEmitter<void>;
    private readonly _form: FormGroup;
    private readonly _resetting: EventEmitter<void>;

    private _deleteError: ApiOperationError | null;
    private _guildId: number | null;
    private _guildIdentities: ICharacterGuildIdentityViewModel[] | null;
    private _hasSaved: boolean;
    private _saveError: ApiOperationError | null;
}
