import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { of, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

import { ApiClient } from "../../common/api-client";
import { IOperationError } from "../../common/types";

import { IUserDetailViewModel, IUserOverviewViewModel, IUserUpdateModel } from "./models";


const usersApiPath
    = "admin/users";

function userApiPath(userId: number): string {
    return `${usersApiPath}/${userId}`;
}


@Injectable({
    providedIn: "root"
})
export class UsersService {

    public constructor(
            apiClient: ApiClient) {

        this._apiClient = apiClient;

    }

    public fetchDetail(
                userId: number):
            Promise<IUserDetailViewModel | null> {
        return this._apiClient.get<IUserDetailViewModel | null>(`${userApiPath(userId)}/detail`)
            .pipe(catchError((error: HttpErrorResponse) => (error.status === 404)
                ? of(null)
                : throwError(error)))
            .toPromise();
    }

    public fetchOverviews():
            Promise<IUserOverviewViewModel[]> {
        return this._apiClient.get<IUserOverviewViewModel[]>(`${usersApiPath}/overviews`)
            .toPromise();
    }

    public update(
                userId: number,
                model: IUserUpdateModel):
            Promise<IOperationError | null> {
        return this._apiClient.put<void>(userApiPath(userId), model)
            .pipe(catchError((error: HttpErrorResponse) => (Math.trunc(error.status / 100) === 4)
                ? of(error.error)
                : throwError(error)))
            .toPromise();
    }

    private readonly _apiClient: ApiClient;
}
