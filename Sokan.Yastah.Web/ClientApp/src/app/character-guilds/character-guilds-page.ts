import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";

import { ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guilds-page",
    templateUrl: "./character-guilds-page.ts.html",
    styleUrls: ["./character-guilds-page.ts.css"]
})
export class CharacterGuildsPage
        implements OnInit {

    public constructor(
            characterGuildsService: CharacterGuildsService) {

        this._characterGuildsService = characterGuildsService;

        this._guilds = this._characterGuildsService.identities;
    }

    public get guilds(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guilds;
    }

    public guildTrackByFn(guild: ICharacterGuildIdentityViewModel): number {
        return guild.id;
    }

    public ngOnInit(): void {
        this._characterGuildsService.reloadIdentities();
    }

    private readonly _characterGuildsService: CharacterGuildsService;
    private readonly _guilds: Observable<ICharacterGuildIdentityViewModel[]>;
}
