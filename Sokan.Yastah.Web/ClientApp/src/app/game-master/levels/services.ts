import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { of, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

import { ApiClient } from "../../common/api-client";
import { IOperationError } from "../../common/types";

import { ICharacterLevelDefinitionViewModel } from "./models";


namespace LevelsApiPaths {

    export const root
        = "characters/levels";

    export const definitions
        = `${root}/definitions`

    export const experienceDiffs
        = `${root}/experienceDiffs`
}


@Injectable({
    providedIn: "root"
})
export class LevelsService {

    public constructor(
            apiClient: ApiClient) {

        this._apiClient = apiClient;
    }

    public fetchDefinitions():
            Promise<ICharacterLevelDefinitionViewModel[]> {
        return this._apiClient.get<ICharacterLevelDefinitionViewModel[]>(LevelsApiPaths.definitions)
            .toPromise();
    }

    public updateExperienceDiffs(
                experienceDiffs: number[]):
            Promise<IOperationError | null> {
        return this._apiClient.put<void>(LevelsApiPaths.experienceDiffs, experienceDiffs)
            .pipe(
                catchError((error: HttpErrorResponse) => (Math.trunc(error.status / 100) === 4)
                    ? of(error.error)
                    : throwError(error)))
            .toPromise();
    }

    private readonly _apiClient: ApiClient;
}
