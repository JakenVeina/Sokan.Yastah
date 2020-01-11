import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { FormOnResettingHandler, FormOnSavingHandler } from "../common/form-component-base";

import { ICharacterGuildDivisionCreationModel, ICharacterGuildDivisionIdentityViewModel } from "./models";
import { CharacterGuildDivisionsService } from "./services";


@Component({
    selector: "character-guild-division-creation-page",
    templateUrl: "./character-guild-division-creation-page.ts.html",
    styleUrls: ["./character-guild-division-creation-page.ts.css"]
})
export class CharacterGuildDivisionCreationPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: CharacterGuildDivisionsService,
            router: Router) {

        let guildId = activatedRoute.parent.parent.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._divisionIdentities = guildId
            .pipe(switchMap(guildId => characterGuildDivisionsService.observeIdentities(guildId)));

        this._onResetting = () => Promise.resolve({
            name: "New Division"
        });

        this._onSaving = guildId
            .pipe(
                map(guildId => async (model) => {
                    let result = await characterGuildDivisionsService.create(guildId, model);

                    if (result.divisionId != null) {
                        router.navigate([`../${result.divisionId}`], { relativeTo: activatedRoute })
                    }

                    return result.error;
                }));
    }

    public get divisionIdentities(): Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._divisionIdentities;
    }
    public get onResetting(): FormOnResettingHandler<ICharacterGuildDivisionCreationModel> {
        return this._onResetting;
    }
    public get onSaving(): Observable<FormOnSavingHandler<ICharacterGuildDivisionCreationModel>> {
        return this._onSaving;
    }

    private readonly _divisionIdentities: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
    private readonly _onResetting: FormOnResettingHandler<ICharacterGuildDivisionCreationModel>;
    private readonly _onSaving: Observable<FormOnSavingHandler<ICharacterGuildDivisionCreationModel>>;
}
