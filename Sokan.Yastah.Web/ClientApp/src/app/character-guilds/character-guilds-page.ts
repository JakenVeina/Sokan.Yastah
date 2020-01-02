import { Component } from "@angular/core";

import { Observable } from "rxjs";

import { ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./services";


@Component({
    selector: "character-guilds-page",
    templateUrl: "./character-guilds-page.ts.html",
    styleUrls: ["./character-guilds-page.ts.css"]
})
export class CharacterGuildsPage {

    public constructor(
            characterGuildsService: CharacterGuildsService) {

        this._guilds = characterGuildsService.observeIdentities();

        setTimeout(() => characterGuildsService.fetchIdentities());
    }

    public get guilds(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guilds;
    }

    public guildTrackByFn(guild: ICharacterGuildIdentityViewModel): number {
        return guild.id;
    }

    private readonly _guilds: Observable<ICharacterGuildIdentityViewModel[]>;
}
