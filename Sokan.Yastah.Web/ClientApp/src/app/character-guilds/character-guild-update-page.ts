import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, Observable } from "rxjs";
import { filter, map, skipWhile, switchMap, take, takeUntil } from "rxjs/operators";

import { FormOnDeletingHandler, FormOnResettingHandler, FormOnSavingHandler } from "../common/types";
import { SubscriberComponentBase } from "../subscriber-component-base";

import { ICharacterGuildIdentityViewModel, ICharacterGuildUpdateModel } from "./models";
import { CharacterGuildsService } from "./services";


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

        let guildId = activatedRoute.paramMap
            .pipe(map(x => Number(x.get("id"))));

        this._otherGuildIdentities = combineLatest(
                guildId,
                characterGuildsService.observeIdentities())
            .pipe(map(([guildId, identities]) => identities.filter(identity => identity.id !== guildId)));

        this._onDeleting = guildId
            .pipe(map(guildId =>
                () => characterGuildsService.delete(guildId)));

        this._onResetting = guildId
            .pipe(map(guildId =>
                (isInit) => (isInit
                    ? characterGuildsService.observeIdentity(guildId)
                        .pipe(
                            filter(identity => identity != null),
                            take(1))
                        .toPromise()
                    : characterGuildsService.fetchIdentity(guildId))));

        this._onSaving = guildId
            .pipe(map(guildId =>
                (model) => characterGuildsService.update(guildId, model)));

        guildId
            .pipe(
                switchMap(guildId => characterGuildsService.observeIdentity(guildId)
                    .pipe(skipWhile(identity => identity == null))),
                filter(identity => identity == null),
                takeUntil(this.destroying))
            .subscribe(() => router.navigate(["../"], { relativeTo: activatedRoute }));
    }

    public get otherGuildIdentities(): Observable<ICharacterGuildIdentityViewModel[]> {
        return this._otherGuildIdentities;
    }
    public get onDeleting(): Observable<FormOnDeletingHandler> {
        return this._onDeleting;
    }
    public get onResetting(): Observable<FormOnResettingHandler<ICharacterGuildUpdateModel>> {
        return this._onResetting;
    }
    public get onSaving(): Observable<FormOnSavingHandler<ICharacterGuildUpdateModel>> {
        return this._onSaving;
    }

    private readonly _otherGuildIdentities: Observable<ICharacterGuildIdentityViewModel[]>;
    private readonly _onDeleting: Observable<FormOnDeletingHandler>;
    private readonly _onResetting: Observable<FormOnResettingHandler<ICharacterGuildUpdateModel>>;
    private readonly _onSaving: Observable<FormOnSavingHandler<ICharacterGuildUpdateModel>>;
}
