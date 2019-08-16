import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { IUserDetailViewModel, IUserOverviewViewModel, IUserUpdateModel } from "./models";
import { ApiClient } from "../../api/api-client";

@Injectable({
    providedIn: "root"
})
export class UsersService {

    public constructor(
            apiClient: ApiClient) {
        this._apiClient = apiClient;
    }

    public getDetail(userId: number): Observable<IUserDetailViewModel> {
        return this._apiClient.get<IUserDetailViewModel>(`admin/users/${userId}/detail`);
    }

    public getOverviews(): Observable<IUserOverviewViewModel[]> {
        return this._apiClient.get<IUserOverviewViewModel[]>("admin/users/overviews");
    }

    public update(userId: number, body: IUserUpdateModel): Observable<void> {
        return this._apiClient.put(`admin/users/${userId}`, body);
    }

    private readonly _apiClient: ApiClient;
}
