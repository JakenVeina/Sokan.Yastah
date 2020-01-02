import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

import { StoreModule } from "@ngrx/store";

import { CharacterGuildsPage } from "./character-guilds-page";
import { CharacterGuildCreationForm } from "./character-guild-creation-form";
import { CharacterGuildCreationPage } from "./character-guild-creation-page";
import { CharacterGuildDivisionsPage } from "./character-guild-divisions-page";
import { CharacterGuildDivisionCreationForm } from "./character-guild-division-creation-form";
import { CharacterGuildDivisionCreationPage } from "./character-guild-division-creation-page";
import { CharacterGuildDivisionUpdateForm } from "./character-guild-division-update-form";
import { CharacterGuildDivisionUpdatePage } from "./character-guild-division-update-page";
import { CharacterGuildUpdateForm } from "./character-guild-update-form";
import { CharacterGuildUpdatePage } from "./character-guild-update-page";

import { CharacterGuildsRoutes } from "./routes";
import { characterGuildsStateReducer } from "./state.reducers";


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        ReactiveFormsModule,
        RouterModule.forChild(CharacterGuildsRoutes),
        StoreModule.forFeature("characterGuilds", characterGuildsStateReducer)
    ],
    declarations: [
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
    ]
})
export class CharacterGuildsModule { }
