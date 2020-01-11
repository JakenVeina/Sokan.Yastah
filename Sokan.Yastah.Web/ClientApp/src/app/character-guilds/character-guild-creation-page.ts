import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";

import { FormOnResettingHandler, FormOnSavingHandler } from "../common/form-component-base";

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

        this._onResetting = () => Promise.resolve({
            name: "New Guild"
        });

        this._onSaving = async (model) => {
            let result = await characterGuildsService.create(model);

            if (result.guildId != null) {
                router.navigate([`../${result.guildId}`], { relativeTo: activatedRoute })
            }

            return result.error || null;
        };
    }

    public get guildIdentities(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guildIdentities;
    }
    public get onResetting(): FormOnResettingHandler<ICharacterGuildCreationModel> {
        return this._onResetting;
    }
    public get onSaving(): FormOnSavingHandler<ICharacterGuildCreationModel> {
        return this._onSaving;
    }

    private readonly _guildIdentities: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _onResetting: FormOnResettingHandler<ICharacterGuildCreationModel>;
    private readonly _onSaving: FormOnSavingHandler<ICharacterGuildCreationModel>;
}
