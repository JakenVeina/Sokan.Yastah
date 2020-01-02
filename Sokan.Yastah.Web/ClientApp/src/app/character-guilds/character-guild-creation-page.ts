import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";

import { FormOnSavingHandler } from "../common/types";

import { ICharacterGuildCreationModel, ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./services";


@Component({
    selector: "character-guild-creation-page",
    templateUrl: "./character-guild-creation-page.ts.html",
    styleUrls: ["./character-guild-creation-page.ts.css"]
})
export class CharacterGuildCreationPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildsService: CharacterGuildsService,
            router: Router) {

        this._guildIdentities = characterGuildsService.observeIdentities();

        this._onSaving = async (model) => {
            let result = await characterGuildsService.create(model);

            if (result.guildId != null) {
                router.navigate([`../${result.guildId}`], { relativeTo: activatedRoute })
            }

            return result.error;
        };
    }

    public get guildIdentities(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guildIdentities;
    }
    public get onSaving(): FormOnSavingHandler<ICharacterGuildCreationModel> {
        return this._onSaving;
    }

    private readonly _guildIdentities: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _onSaving: FormOnSavingHandler<ICharacterGuildCreationModel>;
}
