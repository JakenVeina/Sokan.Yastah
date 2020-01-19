import { Component } from "@angular/core";
import { FormBuilder, FormGroup, FormArray, Validators } from "@angular/forms";

import { FormComponentBase } from "../../common/form-component-base";
import { AppValidators } from "../../common/validators";

import { ICharacterLevelDefinitionViewModel } from "./models";


export interface ICharacterLevelDefinitionsUpdateFormModel {
    experienceThresholds: number[];
}

export namespace CharacterLevelDefinitionsUpdateFormModel {

    export function fromDefinitions(
                definitions: ICharacterLevelDefinitionViewModel[]):
            ICharacterLevelDefinitionsUpdateFormModel {
        return {
            experienceThresholds: definitions
                .map(definition => definition.experienceThreshold)
        };
    }

    export function toExperienceDiffs(
                model: ICharacterLevelDefinitionsUpdateFormModel):
            number[] {
        return model.experienceThresholds
            .filter((_, index) => index != 0)
            .map((threshold, index) => threshold - model.experienceThresholds[index]);
    }
}


@Component({
    selector: "levels-update-form",
    templateUrl: "levels-update-form.ts.html",
    styleUrls: ["levels-update-form.ts.css"]
})
export class LevelsUpdateForm
        extends FormComponentBase<ICharacterLevelDefinitionsUpdateFormModel> {

    public constructor(
            formBuilder: FormBuilder) {
        super();

        this._formBuilder = formBuilder;

        this._experienceThresholds = this._formBuilder.array(
            [],
            control => (<number[]>control.value)
                    .some((threshold, index, thresholds) => (index !== 0) && (threshold <= thresholds[index - 1]))
                ? { "invalidSequence": true }
                : null);

        this._form = this._formBuilder.group({
            experienceThresholds: this._experienceThresholds
        });
    }

    public get canAddLevel(): boolean {
        return this._form.enabled;
    }
    public get canDeleteLevel(): boolean {
        return this._form.enabled && (this._experienceThresholds.controls.length > 1);
    }
    public get form(): FormGroup {
        return this._form;
    }

    public addLevel():
            void {
        var priorControlIndex = this._experienceThresholds.controls.length - 1;
        var priorControlValue = parseInt(this._experienceThresholds.controls[priorControlIndex].value);

        this._addLevelControl(isNaN(priorControlValue)
            ? null
            : priorControlValue + 1);
        this._form.markAsDirty();
    }
    public deleteLevel():
            void {
        this._experienceThresholds.removeAt(this._experienceThresholds.controls.length - 1);
        this._form.markAsDirty();
    }

    protected checkCanSave():
            boolean {
        return super.checkCanSave() && this._form.dirty;
    }
    protected loadModel(
                model: ICharacterLevelDefinitionsUpdateFormModel):
            void {

        while (model.experienceThresholds.length > this._experienceThresholds.controls.length) {
            this._addLevelControl(null);
        }

        while (model.experienceThresholds.length < this._experienceThresholds.controls.length) {
            this.deleteLevel();
        }

        super.loadModel(model);
    }

    private _addLevelControl(
                value: number | null):
            void {
        this._experienceThresholds.push(this._formBuilder.control(
            value,
            [
                Validators.required,
                AppValidators.integer,
            ]
        ));
    }

    private readonly _experienceThresholds: FormArray;
    private readonly _form: FormGroup;
    private readonly _formBuilder: FormBuilder;
}
