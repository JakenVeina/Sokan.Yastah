import { HttpClientModule, HttpClientXsrfModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule } from "@angular/router";

import { StoreModule } from "@ngrx/store";

import { AdminModule } from "./admin/module";
import { CharacterGuildsModule } from "./character-guilds/module";

import { AuthenticationInterceptor } from "./auth/interceptor";
import { authenticationStateReducer } from "./auth/state.reducers";

import { HomePage } from "./home/home-page";
import { NavMenuView } from "./nav-menu/nav-menu-view";
import { AppView } from "./app-view";
import { AppRoutes } from "./routes";


@NgModule({
    imports: [
        BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
        FormsModule,
        HttpClientModule,
        HttpClientXsrfModule.withOptions({
            cookieName: "Yastah.Api.Antiforgery.RequestToken",
            headerName: "Yastah.Api.Antiforgery.RequestToken"
        }),
        ReactiveFormsModule,
        RouterModule.forRoot(AppRoutes),
        StoreModule.forRoot({
            authentication: authenticationStateReducer
        }),

        AdminModule,
        CharacterGuildsModule
    ],
    declarations: [
        AppView,
        HomePage,
        NavMenuView
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true }
    ],
    bootstrap: [AppView]
})
export class AppModule { }
