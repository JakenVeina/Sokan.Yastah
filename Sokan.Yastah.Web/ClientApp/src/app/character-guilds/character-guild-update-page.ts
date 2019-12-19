import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, merge, Observable, Subject } from "rxjs";
import { map, switchMap, takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../subscriber-component-base";

import { ICharacterGuildIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-update-page",
    templateUrl: "./character-guild-update-page.ts.html",
    styleUrls: ["./character-guild-update-page.ts.css"]
})
export class CharacterGuildUpdatePage
        extends SubscriberComponentBase {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildsService: CharacterGuildsService,
            router: Router) {
        super();

        this._resetting = new Subject();

        this._activatedRoute = activatedRoute;
        this._router = router;

        this._guildIdentities = characterGuildsService.identities;

        let guildId = this._activatedRoute.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._guildIdentity = merge(
            guildId
                .pipe(switchMap(guildId => characterGuildsService.observeIdentity(guildId))),
            this._resetting
                .pipe(map(() => null)));

        combineLatest(
                guildId,
                this._resetting)
            .pipe(takeUntil(this.destroying))
            .subscribe(([guildId]) => characterGuildsService.reloadIdentity(guildId));
    }

    public get guildIdentity(): Observable<ICharacterGuildIdentityViewModel> {
        return this._guildIdentity;
    }
    public get guildIdentities(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._guildIdentities;
    }

    public onDeleted(): void {
        this._router.navigate(["../"], { relativeTo: this._activatedRoute });
    }

    public onResetting(): void {
        this._resetting.next();
    }

    public ngOnDestroy(): void {
        super.ngOnDestroy();

        this._resetting.complete();
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _guildIdentity: Observable<ICharacterGuildIdentityViewModel>;
    private readonly _guildIdentities: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _resetting: Subject<void>;
    private readonly _router: Router;
}
