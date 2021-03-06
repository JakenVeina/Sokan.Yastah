﻿import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, from, Observable } from "rxjs";
import { filter, map, switchMap, take, takeUntil } from "rxjs/operators";

import { FormOnDeletingHandler, FormOnResettingHandler, FormOnSavingHandler } from "../../common/form-component-base";
import { SubscriberComponentBase } from "../../common/subscriber-component-base";
import { isNotNullOrUndefined } from "../../common/types";

import { ICharacterGuildDivisionIdentityViewModel, ICharacterGuildDivisionUpdateModel } from "./models";
import { GuildDivisionsService } from "./services";


@Component({
    selector: "guild-division-update-page",
    templateUrl: "./guild-division-update-page.ts.html",
    styleUrls: ["./guild-division-update-page.ts.css"]
})
export class GuildDivisionUpdatePage
        extends SubscriberComponentBase {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: GuildDivisionsService,
            router: Router) {
        super();

        let guildId = activatedRoute.parent!.parent!.paramMap
            .pipe(map(x => Number(x.get("id"))));

        let divisionId = activatedRoute.paramMap
            .pipe(map(x => Number(x.get("id"))));

        let guildIdAndDivisionId = combineLatest(
            guildId,
            divisionId);

        this._otherDivisionIdentities = combineLatest(
                divisionId,
                guildId
                    .pipe(switchMap(guildId => characterGuildDivisionsService.observeIdentities(guildId))))
            .pipe(map(([divisionId, identities]) => identities.filter(identity => identity.id !== divisionId)));

        this._onDeleting = guildIdAndDivisionId
            .pipe(map(([guildId, divisionId]) =>
                () => characterGuildDivisionsService.delete(guildId, divisionId)));

        this._onResetting = guildIdAndDivisionId
            .pipe(
                map(([guildId, divisionId]) =>
                    (isInit) => (isInit
                        ? characterGuildDivisionsService.observeIdentity(guildId, divisionId)
                            .pipe(take(1))
                            .toPromise()
                        : from(characterGuildDivisionsService.fetchIdentity(guildId, divisionId))
                            .pipe(filter(isNotNullOrUndefined))
                            .toPromise())));

        this._onSaving = guildIdAndDivisionId
            .pipe(map(([guildId, divisionId]) =>
                (model) => characterGuildDivisionsService.update(guildId, divisionId, model)));

        guildIdAndDivisionId
            .pipe(
                switchMap(([guildId, divisionId]) => characterGuildDivisionsService.onDeleted(guildId, divisionId)),
                takeUntil(this.destroying))
            .subscribe(() => router.navigate(["../"], { relativeTo: activatedRoute }));
    }

    public get otherDivisionIdentities(): Observable<ICharacterGuildDivisionIdentityViewModel[]> {
        return this._otherDivisionIdentities;
    }
    public get onDeleting(): Observable<FormOnDeletingHandler> {
        return this._onDeleting;
    }
    public get onResetting(): Observable<FormOnResettingHandler<ICharacterGuildDivisionUpdateModel>> {
        return this._onResetting;
    }
    public get onSaving(): Observable<FormOnSavingHandler<ICharacterGuildDivisionUpdateModel>> {
        return this._onSaving;
    }

    private readonly _otherDivisionIdentities: Observable<ICharacterGuildDivisionIdentityViewModel[]>;
    private readonly _onDeleting: Observable<FormOnDeletingHandler>;
    private readonly _onResetting: Observable<FormOnResettingHandler<ICharacterGuildDivisionUpdateModel>>;
    private readonly _onSaving: Observable<FormOnSavingHandler<ICharacterGuildDivisionUpdateModel>>;
}
