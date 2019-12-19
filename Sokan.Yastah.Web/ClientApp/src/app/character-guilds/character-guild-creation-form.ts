import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { combineLatest } from "rxjs";
import { takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../subscriber-component-base";
import { AppValidators } from "../validators";

import { ApiOperationError } from "../api/api-operation-error";

import { ICharacterGuildCreationModel, ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-creation-form",
    templateUrl: "./character-guild-creation-form.ts.html"
})
export class CharacterGuildCreationForm
        extends SubscriberComponentBase {

    public constructor(
            characterGuildsService: CharacterGuildsService,
            formBuilder: FormBuilder) {
        super();

        this._saved = new EventEmitter<number>();

        this._characterGuildsService = characterGuildsService;

        this._form = formBuilder.group({
            name: formBuilder.control(
                null,
                [
                    Validators.required,
                    AppValidators.notDuplicated(() => this._guilds && this._guilds.map(x => x.name))
                ])
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
    public set guilds(value: ICharacterGuildIdentityViewModel[]) {
        this._guilds = value;
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
        this.form.reset(<ICharacterGuildCreationModel>{
            name: "New Guild"
        });
    }

    public save(): void {
        this._form.disable();
        this._characterGuildsService.create(this._form.value)
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

    private _guilds: ICharacterGuildIdentityViewModel[] | null;
    private _saveError: ApiOperationError | null;
}
