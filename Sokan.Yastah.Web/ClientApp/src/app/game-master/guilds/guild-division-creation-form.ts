import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { AppValidators } from "../../common/validators";

import { ICharacterGuildDivisionCreationModel, ICharacterGuildDivisionIdentityViewModel } from "./models";


@Component({
    selector: "guild-division-creation-form",
    templateUrl: "./guild-division-creation-form.ts.html"
})
export class GuildDivisionCreationForm
        extends FormComponentBase<ICharacterGuildDivisionCreationModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._divisionIdentities = null;

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
    }

    @Input("division-identities")
    public set divisionIdentities(value: ICharacterGuildDivisionIdentityViewModel[] | null) {
        this._divisionIdentities = value;
        this._form.updateValueAndValidity();
    }

    public get form(): FormGroup {
        return this._form;
    }

    private readonly _form: FormGroup;

    private _divisionIdentities: ICharacterGuildDivisionIdentityViewModel[] | null;
}
