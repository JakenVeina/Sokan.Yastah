import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";

import { ICharacterGuildDivisionIdentityViewModel } from "./models";
import { CharacterGuildDivisionsService } from "./services";


@Component({
    selector: "character-guild-divisions-page",
    templateUrl: "./character-guild-divisions-page.ts.html",
    styleUrls: ["./character-guild-divisions-page.ts.css"]
})
export class CharacterGuildDivisionsPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: CharacterGuildDivisionsService) {

        this._divisions = activatedRoute.parent.paramMap
            .pipe(
                map(x => Number(x.get("id"))),
                tap(guildId => setTimeout(() => characterGuildDivisionsService.fetchIdentities(guildId))),
                switchMap(guildId => characterGuildDivisionsService.observeIdentities(guildId)));
    }

    public get divisions(): Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._divisions;
    }

    public divisionTrackByFn(division: ICharacterGuildDivisionIdentityViewModel): number {
        return division.id;
    }

    private readonly _divisions: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
}
