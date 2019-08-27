import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { IAppState } from "../state";

import { ReloadTicketAction } from "./actions";

@Injectable()
export class AuthenticationInterceptor
    implements HttpInterceptor {

    public constructor(
            appState: Store<IAppState>) {
        this._appState = appState;
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler):
            Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(tap(e => {
                if (e instanceof HttpResponse) {
                    // No, we cannot easily limit this to ONLY when the cookie changes. Cause the browser doesn't expose anything to let us know.
                    this._appState.dispatch(new ReloadTicketAction());
                }
            }));
    }

    private readonly _appState: Store<IAppState>;
}
