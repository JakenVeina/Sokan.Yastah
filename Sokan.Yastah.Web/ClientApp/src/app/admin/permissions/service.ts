import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { ApiClient } from "../../api/api-client";

import { IPermissionCategoryDescriptionViewModel } from "./models";

@Injectable({
    providedIn: "root"
})
export class PermissionsService {

    public constructor(
            apiClient: ApiClient) {
        this._apiClient = apiClient;
    }

    public getDescriptions(): Observable<IPermissionCategoryDescriptionViewModel[]> {
        return this._apiClient.get<IPermissionCategoryDescriptionViewModel[]>("admin/permissions/descriptions");
    }

    private readonly _apiClient: ApiClient;
}
