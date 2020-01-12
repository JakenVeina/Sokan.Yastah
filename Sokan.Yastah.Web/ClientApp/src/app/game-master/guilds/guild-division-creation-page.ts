import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";

import { ICharacterGuildDivisionCreationModel, ICharacterGuildDivisionIdentityViewModel } from "./models";
import { GuildDivisionsService } from "./services";


@Component({
    selector: "guild-division-creation-page",
    templateUrl: "./guild-division-creation-page.ts.html",
    styleUrls: ["./guild-division-creation-page.ts.css"]
})
export class GuildDivisionCreationPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: GuildDivisionsService,
            router: Router) {

        let guildId = activatedRoute.parent!.parent!.paramMap
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

                    return result.error || null;
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
