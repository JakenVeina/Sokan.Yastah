import { Component } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { combineLatest, Observable } from "rxjs";
import { filter, map, skipWhile, switchMap, take, takeUntil } from "rxjs/operators";

import { FormOnDeletingHandler, FormOnResettingHandler, FormOnSavingHandler } from "../common/types";
import { SubscriberComponentBase } from "../subscriber-component-base";

import { ICharacterGuildDivisionIdentityViewModel, ICharacterGuildDivisionUpdateModel } from "./models";
import { CharacterGuildDivisionsService } from "./services";


@Component({
    selector: "character-guild-division-update-page",
    templateUrl: "./character-guild-division-update-page.ts.html",
    styleUrls: ["./character-guild-division-update-page.ts.css"]
})
export class CharacterGuildDivisionUpdatePage
        extends SubscriberComponentBase {

    public constructor(
            activatedRoute: ActivatedRoute,
            characterGuildDivisionsService: CharacterGuildDivisionsService,
            router: Router) {
        super();

        let guildId = activatedRoute.parent.parent.paramMap
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
                            .pipe(
                                filter(identity => identity != null),
                                take(1))
                            .toPromise()
                        : characterGuildDivisionsService.fetchIdentity(guildId, divisionId))));

        this._onSaving = guildIdAndDivisionId
            .pipe(map(([guildId, divisionId]) =>
                (model) => characterGuildDivisionsService.update(guildId, divisionId, model)));

        guildIdAndDivisionId
            .pipe(
                switchMap(([guildId, divisionId]) => characterGuildDivisionsService.observeIdentity(guildId, divisionId)
                    .pipe(skipWhile(identity => identity == null))),
                filter(identity => identity == null),
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
