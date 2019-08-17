﻿import { Component } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";

import { IAppState } from "../../state";

import { IUserOverviewViewModel } from "./models";
import { ReloadOverviewsAction } from "./actions";

@Component({
    selector: "users-view",
    templateUrl: "./users-view.ts.html",
    styleUrls: ["./users-view.ts.css"]
})
export class UsersView {

    public constructor(
            appState: Store<IAppState>) {
        this.users = appState
            .select(x => x.admin.users.overviews.map(x => ({
                id: x.id,
                username: x.username,
                discriminator: x.discriminator,
                firstSeen: x.firstSeen,
                lastSeen: x.lastSeen
            })));

        appState.dispatch(new ReloadOverviewsAction());
    }

    public readonly users: Observable<IUserOverviewViewModel[]>;

    public getId(user: IUserOverviewViewModel): number {
        return user.id;
    }

    public asdf(x: any, y: any): void {
        x.isExpanded = y;
        console.log(x);
        console.log(y);
    }
}