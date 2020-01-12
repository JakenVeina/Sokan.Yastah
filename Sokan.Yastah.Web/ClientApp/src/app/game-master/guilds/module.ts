import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

import { StoreModule } from "@ngrx/store";

import { GuildsPage } from "./guilds-page";
import { GuildCreationForm } from "./guild-creation-form";
import { GuildCreationPage } from "./guild-creation-page";
import { GuildDivisionsPage } from "./guild-divisions-page";
import { GuildDivisionCreationForm } from "./guild-division-creation-form";
import { GuildDivisionCreationPage } from "./guild-division-creation-page";
import { GuildDivisionUpdateForm } from "./guild-division-update-form";
import { GuildDivisionUpdatePage } from "./guild-division-update-page";
import { GuildUpdateForm } from "./guild-update-form";
import { GuildUpdatePage } from "./guild-update-page";

import { guildsRoutes } from "./routes";
import { guildsStateReducer } from "./state.reducers";


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        ReactiveFormsModule,
        RouterModule.forChild(guildsRoutes),
        StoreModule.forFeature("characterGuilds", guildsStateReducer)
    ],
    declarations: [
        GuildsPage,
        GuildCreationForm,
        GuildCreationPage,
        GuildDivisionsPage,
        GuildDivisionCreationForm,
        GuildDivisionCreationPage,
        GuildDivisionUpdateForm,
        GuildDivisionUpdatePage,
        GuildUpdateForm,
        GuildUpdatePage
    ]
})
export class CharacterGuildsModule { }
