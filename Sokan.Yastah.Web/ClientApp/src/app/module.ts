import { HttpClientModule, HttpClientXsrfModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule } from "@angular/router";
import { StoreModule } from "@ngrx/store";

import { AdminModule } from "./admin/module";

import { HomeView } from "./home/home-view";
import { NavMenuView } from "./nav-menu/nav-menu-view";
import { AppView } from "./app-view";
import { AppRoutes } from "./routes";
import { EffectsModule } from "@ngrx/effects";

@NgModule({
    imports: [
        BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
        EffectsModule.forRoot([]),
        HttpClientModule,
        HttpClientXsrfModule.withOptions({
            cookieName: "Yastah.Api.Antiforgery.RequestToken",
            headerName: "Yastah.Api.Antiforgery.RequestToken"
        }),
        RouterModule.forRoot(AppRoutes),
        StoreModule.forRoot({}),

        AdminModule
    ],
    declarations: [
        AppView,
        HomeView,
        NavMenuView
    ],
    bootstrap: [AppView]
})
export class AppModule { }
