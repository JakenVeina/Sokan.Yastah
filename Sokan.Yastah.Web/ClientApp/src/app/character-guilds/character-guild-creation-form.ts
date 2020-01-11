import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../common/form-component-base";
import { AppValidators } from "../common/validators";

import { ICharacterGuildCreationModel, ICharacterGuildIdentityViewModel } from "./models";


@Component({
    selector: "character-guild-creation-form",
    templateUrl: "./character-guild-creation-form.ts.html"
})
export class CharacterGuildCreationForm
        extends FormComponentBase<ICharacterGuildCreationModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._guildIdentities && this._guildIdentities.map(x => x.name))
                    ])
            },
            {
                validators: () => (this._guildIdentities == null)
                    ? { "uninitialized": true }
                    : null
            });
    }

    @Input("guild-identities")
    public set guildIdentities(value: ICharacterGuildIdentityViewModel[]) {
        this._guildIdentities = value;
        this._form.updateValueAndValidity();
    }

    public get form(): FormGroup {
        return this._form;
    }

    private readonly _form: FormGroup;

    private _guildIdentities: ICharacterGuildIdentityViewModel[] | null;
}
