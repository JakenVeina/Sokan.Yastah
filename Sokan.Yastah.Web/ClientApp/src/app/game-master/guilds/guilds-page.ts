import { Component } from "@angular/core";

import { Observable } from "rxjs";

import { ICharacterGuildIdentityViewModel } from "./models";
import { GuildsService } from "./services";


@Component({
    selector: "guilds-page",
    templateUrl: "./guilds-page.ts.html",
    styleUrls: ["./guilds-page.ts.css"]
})
export class GuildsPage {

    public constructor(
            characterGuildsService: GuildsService) {

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
