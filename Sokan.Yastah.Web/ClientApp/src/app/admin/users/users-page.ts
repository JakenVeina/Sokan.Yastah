import { Component } from "@angular/core";

import { IUserOverviewViewModel } from "./models";
import { UsersService } from "./services";


@Component({
    selector: "users-page",
    templateUrl: "./users-page.ts.html",
    styleUrls: ["./users-page.ts.css"]
})
export class UsersPage {

    public constructor(
            usersService: UsersService) {

        this._users = usersService.fetchOverviews();
    }

    public get users(): Promise<IUserOverviewViewModel[]> {
        return this._users;
    };

    public userTrackByFn(user: IUserOverviewViewModel): number {
        return user.id;
    }

    private readonly _users: Promise<IUserOverviewViewModel[]>;
}
