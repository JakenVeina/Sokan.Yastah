import { HttpClientModule, HttpClientXsrfModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule } from "@angular/router";
import { EffectsModule } from "@ngrx/effects";
import { StoreModule } from "@ngrx/store";

import { AdminModule } from "./admin/module";

import { AuthenticationEffects } from "./authentication/effects";
import { authenticationStateReducer } from "./authentication/reducers";

import { characterGuildsStateReducer } from "./character-guilds/state.reducers";

import { CharacterGuildsPage } from "./character-guilds/character-guilds-page";
import { CharacterGuildCreationForm } from "./character-guilds/character-guild-creation-form";
import { CharacterGuildCreationPage } from "./character-guilds/character-guild-creation-page";
import { CharacterGuildDivisionsPage } from "./character-guilds/character-guild-divisions-page";
import { CharacterGuildDivisionCreationForm } from "./character-guilds/character-guild-division-creation-form";
import { CharacterGuildDivisionCreationPage } from "./character-guilds/character-guild-division-creation-page";
import { CharacterGuildDivisionUpdateForm } from "./character-guilds/character-guild-division-update-form";
import { CharacterGuildDivisionUpdatePage } from "./character-guilds/character-guild-division-update-page";
import { CharacterGuildUpdateForm } from "./character-guilds/character-guild-update-form";
import { CharacterGuildUpdatePage } from "./character-guilds/character-guild-update-page";

import { HomeView } from "./home/home-view";
import { NavMenuView } from "./nav-menu/nav-menu-view";
import { AppView } from "./app-view";
import { AppRoutes } from "./routes";

import { AuthenticationInterceptor } from "./authentication/interceptor";

@NgModule({
    imports: [
        BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
        EffectsModule.forRoot([
            AuthenticationEffects
        ]),
        FormsModule,
        HttpClientModule,
        HttpClientXsrfModule.withOptions({
            cookieName: "Yastah.Api.Antiforgery.RequestToken",
            headerName: "Yastah.Api.Antiforgery.RequestToken"
        }),
        ReactiveFormsModule,
        RouterModule.forRoot(AppRoutes),
        StoreModule.forRoot({
            authentication: authenticationStateReducer,
            characterGuilds: characterGuildsStateReducer
        }),

        AdminModule
    ],
    declarations: [
        AppView,
        HomeView,
        NavMenuView,
        CharacterGuildsPage,
        CharacterGuildCreationForm,
        CharacterGuildCreationPage,
        CharacterGuildDivisionsPage,
        CharacterGuildDivisionCreationForm,
        CharacterGuildDivisionCreationPage,
        CharacterGuildDivisionUpdateForm,
        CharacterGuildDivisionUpdatePage,
        CharacterGuildUpdateForm,
        CharacterGuildUpdatePage
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthenticationInterceptor, multi: true }
    ],
    bootstrap: [AppView]
})
export class AppModule { }
