import { Injectable } from "@angular/core";
import { Location } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

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
            .pipe(tap(this.updateAuthenticationTicket.bind(this)));
    }

    public get<T>(url: string): Observable<T> {
        return this._httpClient.get<T>(this.buildUrl(url))
            .pipe(tap(this.updateAuthenticationTicket.bind(this)));
    }

    public post<T>(url: string, body: any | null): Observable<T> {
        return this._httpClient.post<T>(this.buildUrl(url), body)
            .pipe(tap(this.updateAuthenticationTicket.bind(this)));
    }

    public put<T>(url: string, body: any | null): Observable<T> {
        return this._httpClient.put<T>(this.buildUrl(url), body)
            .pipe(tap(this.updateAuthenticationTicket.bind(this)));
    }

    private buildUrl(url: string): string {
        return Location.joinWithSlash(ApiClient._apiRoot, url);
    }

    private updateAuthenticationTicket(): void {
        this._authenticationService.updateTicket();
    }

    private readonly _authenticationService: AuthenticationService;
    private readonly _httpClient: HttpClient;
}