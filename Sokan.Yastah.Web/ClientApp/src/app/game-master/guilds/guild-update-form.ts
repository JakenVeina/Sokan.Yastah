import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { AppValidators } from "../../common/validators";

import { ICharacterGuildIdentityViewModel, ICharacterGuildUpdateModel } from "./models";


@Component({
    selector: "guild-update-form",
    templateUrl: "./guild-update-form.ts.html"
})
export class GuildUpdateForm
        extends FormComponentBase<ICharacterGuildUpdateModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._otherGuildIdentities = null;

        this._form = formBuilder.group(
            {
                name: formBuilder.control(
                    null,
                    [
                        Validators.required,
                        AppValidators.notDuplicated(() => this._otherGuildIdentities && this._otherGuildIdentities.map(x => x.name))
                    ])
            },
            {
                validators: () => (this._otherGuildIdentities == null)
                    ? { "uninitialized": true }
                    : null
            });
    }

    @Input("other-guild-identities")
    public set otherGuildIdentities(value: ICharacterGuildIdentityViewModel[] | null) {
        this._otherGuildIdentities = value;
        this._form.updateValueAndValidity();
    }

    public get form(): FormGroup {
        return this._form;
    }

    protected checkCanSave():
            boolean {
        return super.checkCanSave() && this._form.dirty;
    }

    private readonly _form: FormGroup;

    private _otherGuildIdentities: ICharacterGuildIdentityViewModel[] | null;
}
