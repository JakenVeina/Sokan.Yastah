import { Injectable } from "@angular/core";
import { Location } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";

import { AuthenticationService } from "../authentication/authentication-service";

@Injectable({
    providedIn: "root"
})
export class ApiClient {
    private static readonly _apiRoot: string
        = "/api";

    public constructor(
            authenticationService: AuthenticationService,
            httpClient: HttpClient) {
        this._authenticationService = authenticationService;
        this._httpClient = httpClient;
    }

    public delete<T>(url: string): Observable<T> {
        return this._httpClient.delete<T>(this.buildUrl(url))
            .pipe(finalize(() => this._authenticationService.updateTicket()));
    }

    public get<T>(url: string): Observable<T> {
        return this._httpClient.get<T>(this.buildUrl(url))
            .pipe(finalize(() => this._authenticationService.updateTicket()));
    }

    public post<T>(url: string, body: any | null): Observable<T> {
        return this._httpClient.post<T>(this.buildUrl(url), body)
            .pipe(finalize(() => this._authenticationService.updateTicket()));
    }

    public put<T>(url: string, body: any | null): Observable<T> {
        return this._httpClient.put<T>(this.buildUrl(url), body)
            .pipe(finalize(() => this._authenticationService.updateTicket()));
    }

    private buildUrl(url: string): string {
        return Location.joinWithSlash(ApiClient._apiRoot, url);
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _httpClient: HttpClient;
}