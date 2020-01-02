import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { FormOnSavingHandler } from "../common/types";

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

        this._guildId = activatedRoute.parent.parent.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._divisionIdentities = this._guildId
            .pipe(switchMap(guildId => characterGuildDivisionsService.observeIdentities(guildId)));

        this._onSaving = this._guildId
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
    public get guildId(): Observable<number> {
        return this._guildId;
    }
    public get onSaving(): Observable<FormOnSavingHandler<ICharacterGuildDivisionCreationModel>> {
        return this._onSaving;
    }

    private readonly _divisionIdentities: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
    private readonly _guildId: Observable<number>;
    private readonly _onSaving: Observable<FormOnSavingHandler<ICharacterGuildDivisionCreationModel>>;
}
