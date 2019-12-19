import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, merge, Observable, Subject } from "rxjs";
import { map, switchMap, takeUntil } from "rxjs/operators";

import { SubscriberComponentBase } from "../subscriber-component-base";

import { ICharacterGuildDivisionIdentityViewModel } from "./models";
import { CharacterGuildsService } from "./service";

@Component({
    selector: "character-guild-division-update-page",
    templateUrl: "./character-guild-division-update-page.ts.html",
    styleUrls: ["./character-guild-division-update-page.ts.css"]
})
export class CharacterGuildDivisionUpdatePage
        extends SubscriberComponentBase {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildsService: CharacterGuildsService,
            router: Router) {
        super();

        this._resetting = new Subject();

        this._activatedRoute = activatedRoute;
        this._router = router;

        this._guildId = this._activatedRoute.parent.parent.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._divisionIdentities = this._guildId
            .pipe(switchMap(guildId => characterGuildsService.observeDivisionIdentities(guildId)));

        let guildIdAndDivisionId = combineLatest(
            this._guildId,
            this._activatedRoute.paramMap
                .pipe(map(x => Number(x.get("id")))));

        this._divisionIdentity = merge(
            guildIdAndDivisionId
                .pipe(switchMap(([guildId, divisionId]) => characterGuildsService.observeDivisionIdentity(guildId, divisionId))),
            this._resetting
                .pipe(map(() => null)));

        combineLatest(
                guildIdAndDivisionId,
                this._resetting)
            .pipe(takeUntil(this.destroying))
            .subscribe(([[guildId, divisionId]]) => characterGuildsService.reloadDivisionIdentity(guildId, divisionId));
    }

    public get divisionIdentity(): Observable<ICharacterGuildDivisionIdentityViewModel> {
        return this._divisionIdentity;
    }
    public get divisionIdentities(): Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._divisionIdentities;
    }
    public get guildId(): Observable<number> {
        return this._guildId;
    }

    public onDeleted(): void {
        this._router.navigate(["../"], { relativeTo: this._activatedRoute });
    }

    public onResetting(): void {
        this._resetting.next();
    }

    public ngOnDestroy(): void {
        this._resetting.complete();
        super.ngOnDestroy();
    }

    private readonly _activatedRoute: ActivatedRoute;
    private readonly _divisionIdentity: Observable<ICharacterGuildDivisionIdentityViewModel>;
    private readonly _divisionIdentities: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
    private readonly _guildId: Observable<number>;
    private readonly _resetting: Subject<void>;
    private readonly _router: Router;
}
