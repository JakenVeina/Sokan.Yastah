import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";

import { ICharacterGuildDivisionIdentityViewModel } from "./models";
import { GuildDivisionsService } from "./services";


@Component({
    selector: "guild-divisions-page",
    templateUrl: "./guild-divisions-page.ts.html",
    styleUrls: ["./guild-divisions-page.ts.css"]
})
export class GuildDivisionsPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: GuildDivisionsService) {

        this._divisions = activatedRoute.parent!.paramMap
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
