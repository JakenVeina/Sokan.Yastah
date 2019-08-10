import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { IRoleCreationModel, IRoleDetailViewModel, IRoleIdentityViewModel, IRoleUpdateModel } from "./models";
import { ApiClient } from "../../api/api-client";

@Injectable({
    providedIn: "root"
})
export class RolesService {

    public constructor(
            apiClient: ApiClient) {
        this._apiClient = apiClient;
    }

    public create(model: IRoleCreationModel): Observable<number> {
        return this._apiClient.post<number>("admin/roles/new", model);
    }

    public delete(roleId: number): Observable<void> {
        return this._apiClient.delete<void>(`admin/roles/${roleId}`);
    }

    public getDetail(roleId: number): Observable<IRoleDetailViewModel> {
        return this._apiClient.get<IRoleDetailViewModel>(`admin/roles/${roleId}/detail`);
    }

    public getIdentities(): Observable<IRoleIdentityViewModel[]> {
        return this._apiClient.get<IRoleIdentityViewModel[]>("admin/roles/identities");
    }

    public update(roleId: number, model: IRoleUpdateModel): Observable<void> {
        return this._apiClient.put<void>(`admin/roles/${roleId}`, model);
    }

    private readonly _apiClient: ApiClient;
}
