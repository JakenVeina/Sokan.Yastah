import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { AppValidators } from "../../common/validators";

import { ICharacterGuildDivisionIdentityViewModel, ICharacterGuildDivisionUpdateModel } from "./models";


@Component({
    selector: "guild-division-update-form",
    templateUrl: "./guild-division-update-form.ts.html"
})
export class GuildDivisionUpdateForm
        extends FormComponentBase<ICharacterGuildDivisionUpdateModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._otherDivisionIdentities = null;

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
    }

    @Input("other-division-identities")
    public set otherDivisionIdentities(value: ICharacterGuildDivisionIdentityViewModel[] | null) {
        this._otherDivisionIdentities = value;
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

    private _otherDivisionIdentities: ICharacterGuildDivisionIdentityViewModel[] | null;
}
