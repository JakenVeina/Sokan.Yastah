import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";

import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { AuthenticationService } from "./services";


@Injectable()
export class AuthenticationInterceptor
        implements HttpInterceptor {

    public constructor(
            authenticationService: AuthenticationService) {
        this._authenticationService = authenticationService;
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler):
            Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(tap(e => {
                if (e instanceof HttpResponse) {
                    // No, we cannot easily limit this to ONLY when the cookie changes. Cause the browser doesn't expose anything to let us know.
                    this._authenticationService.reloadTicket();
                }
            }));
    }

    private readonly _authenticationService: AuthenticationService;
}
