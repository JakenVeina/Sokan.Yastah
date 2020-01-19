import { Component } from "@angular/core";

import { FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";

import { CharacterLevelDefinitionsUpdateFormModel, ICharacterLevelDefinitionsUpdateFormModel } from "./levels-update-form";
import { LevelsService } from "./services";


@Component({
    selector: "levels-update-page",
    templateUrl: "levels-update-page.ts.html",
    styleUrls: ["levels-update-page.ts.css"]
})
export class LevelsUpdatePage {

    public constructor(
            levelsService: LevelsService) {

        this._onResetting = () => levelsService.fetchDefinitions()
            .then(definitions => CharacterLevelDefinitionsUpdateFormModel.fromDefinitions(definitions));

        this._onSaving = (model) => levelsService.updateExperienceDiffs(
            CharacterLevelDefinitionsUpdateFormModel.toExperienceDiffs(model));
    }

    public get onResetting(): FormOnResettingHandler<ICharacterLevelDefinitionsUpdateFormModel> {
        return this._onResetting;
    }
    public get onSaving(): FormOnSavingHandler<ICharacterLevelDefinitionsUpdateFormModel> {
        return this._onSaving;
    }

    private readonly _onResetting: FormOnResettingHandler<ICharacterLevelDefinitionsUpdateFormModel>;
    private readonly _onSaving: FormOnSavingHandler<ICharacterLevelDefinitionsUpdateFormModel>;
}
