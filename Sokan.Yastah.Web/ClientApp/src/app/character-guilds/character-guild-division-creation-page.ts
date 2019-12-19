import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";
import { map, switchMap } from "rxjs/operators";

import { ICharacterGuildDivisionIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-division-creation-page",
    templateUrl: "./character-guild-division-creation-page.ts.html",
    styleUrls: ["./character-guild-division-creation-page.ts.css"]
})
export class CharacterGuildDivisionCreationPage {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildsService: CharacterGuildsService,
            router: Router) {

        this._activatedRoute = activatedRoute;
        this._router = router;

        this._guildId = this._activatedRoute.parent.parent.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._divisions = this._guildId
            .pipe(switchMap(guildId => characterGuildsService.observeDivisionIdentities(guildId)));
    }

    public get divisions(): Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._divisions;
    }
    public get guildId(): Observable<number> {
        return this._guildId;
    }

    public onSaved(divisionId: number) {
        this._router.navigate([`../${divisionId}`], { relativeTo: this._activatedRoute });
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _divisions: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
    private readonly _guildId: Observable<number>;
    private readonly _router: Router;
}
