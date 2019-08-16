import { Component } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";

import { IRoleIdentityViewModel } from "./models";
import { ReloadIdentitiesAction } from "./actions";
import { IAppState } from "../../state";

@Component({
    selector: "roles-view",
    templateUrl: "./roles-view.ts.html",
    styleUrls: ["./roles-view.ts.css"]
})
export class RolesView {

    public constructor(
            appState: Store<IAppState>) {

        this.roles = appState
            .select(x => x.admin.roles.identities);

        appState.dispatch(new ReloadIdentitiesAction());
    }

    public readonly roles: Observable<IRoleIdentityViewModel[]>;

    public getId(role: IRoleIdentityViewModel): number {
        return role.id;
    }
}
