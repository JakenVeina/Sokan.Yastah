import { Routes } from "@angular/router";

import { CharacterGuildsPage } from "./character-guilds-page";
import { CharacterGuildCreationPage } from "./character-guild-creation-page";
import { CharacterGuildDivisionsPage } from "./character-guild-divisions-page";
import { CharacterGuildDivisionCreationPage } from "./character-guild-division-creation-page";
import { CharacterGuildDivisionUpdatePage } from "./character-guild-division-update-page";
import { CharacterGuildUpdatePage } from "./character-guild-update-page";

import { CharacterGuildsGuard } from "./routes.guards";


export const CharacterGuildsRoutes: Routes = [
    {
        path: "characters",
        children: [
            {
                path: "guilds",
                canActivate: [CharacterGuildsGuard],
                component: CharacterGuildsPage,
                children: [
                    {
                        path: "new",
                        component: CharacterGuildCreationPage
                    },
                    {
                        path: ":id",
                        component: CharacterGuildUpdatePage,
                        children: [
                            {
                                path: "divisions",
                                component: CharacterGuildDivisionsPage,
                                children: [
                                    {
                                        path: "new",
                                        component: CharacterGuildDivisionCreationPage
                                    },
                                    {
                                        path: ":id",
                                        component: CharacterGuildDivisionUpdatePage
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ]
    }
];
