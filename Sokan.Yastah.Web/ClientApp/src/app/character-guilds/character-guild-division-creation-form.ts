import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { combineLatest } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../subscriber-component-base";
import { AppValidators } from "../validators";

import { ApiOperationError } from "../api/api-operation-error";

import { ICharacterGuildDivisionIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-division-creation-form",
    templateUrl: "./character-guild-division-creation-form.ts.html"
})
export class CharacterGuildDivisionCreationForm
        extends SubscriberComponentBase {

    public constructor(
            characterGuildsService: CharacterGuildsService,
            formBuilder: FormBuilder) {
        super();

        this._saved = new EventEmitter<number>();

        this._characterGuildsService = characterGuildsService;

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._divisions && this._divisions.map(x => x.name))
                    ])
            },
            {
                validators: () => (this._guildId == null)
                    ? { "uninitialized": true }
                    : null
            });

        combineLatest(
                this._form.statusChanges,
                this._form.valueChanges)
            .pipe(takeUntil(this.destroying))
            .subscribe(() => {
                this._saveError = null;
            });

        this.reset();
    }

    @Input()
    public set divisions(value: ICharacterGuildDivisionIdentityViewModel[] | null) {
        this._divisions = value;
    }
    @Input()
    public set guildId(value: number | null) {
        this._guildId = value;
    }

    @Output()
    public get saved(): EventEmitter<number> {
        return this._saved;
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
    public get saveError(): ApiOperationError | null {
        return this._saveError;
    }

    public reset(): void {
        this.form.reset({
            name: "New Division"
        });
    }

    public save(): void {
        this._form.disable();
        this._characterGuildsService.createDivision(this._guildId, this._form.value)
            .pipe(takeUntil(this.destroying))
            .subscribe(
                guildId => {
                    this._saveError = null;
                    this._saved.emit(guildId);
                },
                xhr => {
                    this._form.enable();
                    this._saveError = xhr.error;
                });
    }

    private readonly _characterGuildsService: CharacterGuildsService;
    private readonly _form: FormGroup;
    private readonly _saved: EventEmitter<number>;

    private _divisions: ICharacterGuildDivisionIdentityViewModel[] | null;
    private _guildId: number | null;
    private _saveError: ApiOperationError | null;
}
