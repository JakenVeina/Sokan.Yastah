import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

import { StoreModule } from "@ngrx/store";

import { GuildsPage } from "./guilds/guilds-page";
import { GuildCreationForm } from "./guilds/guild-creation-form";
import { GuildCreationPage } from "./guilds/guild-creation-page";
import { GuildDivisionsPage } from "./guilds/guild-divisions-page";
import { GuildDivisionCreationForm } from "./guilds/guild-division-creation-form";
import { GuildDivisionCreationPage } from "./guilds/guild-division-creation-page";
import { GuildDivisionUpdateForm } from "./guilds/guild-division-update-form";
import { GuildDivisionUpdatePage } from "./guilds/guild-division-update-page";
import { GuildUpdateForm } from "./guilds/guild-update-form";
import { GuildUpdatePage } from "./guilds/guild-update-page";
import { LevelsUpdateForm } from "./levels/levels-update-form";
import { LevelsUpdatePage } from "./levels/levels-update-page";

import { gameMasterRouts } from "./routes";
import { gameMasterStateReducers } from "./state.reducers";


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        ReactiveFormsModule,
        RouterModule.forChild(gameMasterRouts),
        StoreModule.forFeature("gameMaster", gameMasterStateReducers)
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
        GuildUpdatePage,
        LevelsUpdateForm,
        LevelsUpdatePage
    ]
})
export class GameMasterModule { }
