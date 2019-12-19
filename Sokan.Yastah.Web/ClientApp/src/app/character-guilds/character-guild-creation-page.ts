import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { Observable } from "rxjs";

import { ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

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

        this._activatedRoute = activatedRoute;
        this._router = router;

        this._guilds = characterGuildsService.identities;
    }

    public get guilds(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guilds;
    }

    public onSaved(guildId: number) {
        this._router.navigate([`../${guildId}`], { relativeTo: this._activatedRoute });
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _guilds: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _router: Router;
}
