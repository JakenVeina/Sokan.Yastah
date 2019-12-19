import { OnDestroy } from "@angular/core";
import { Observable, Subject } from "rxjs";

export abstract class SubscriberComponentBase
        implements OnDestroy {

    public constructor() {
        this._destroying = new Subject();
    }

    public ngOnDestroy(): void {
        this._destroying.next();
        this._destroying.complete();
    }

    public get destroying(): Observable<void> {
        return this._destroying;
    }

    private readonly _destroying: Subject<void>;
}
