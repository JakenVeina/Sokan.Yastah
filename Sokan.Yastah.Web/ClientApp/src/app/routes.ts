import { Routes } from "@angular/router";

import { HomeView } from "./home/home-view";
import { CharacterGuildsPage } from "./character-guilds/character-guilds-page";
import { CharacterGuildCreationPage } from "./character-guilds/character-guild-creation-page";
import { CharacterGuildDivisionsPage } from "./character-guilds/character-guild-divisions-page";
import { CharacterGuildDivisionCreationPage } from "./character-guilds/character-guild-division-creation-page";
import { CharacterGuildDivisionUpdatePage } from "./character-guilds/character-guild-division-update-page";
import { CharacterGuildUpdatePage } from "./character-guilds/character-guild-update-page";

import { PortalGuard, CharacterGuildsGuard } from "./guards";

export const AppRoutes: Routes = [
    {
        path: "home",
        canActivate: [PortalGuard],
        component: HomeView
    },
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
    },
    {
        path: "",
        redirectTo: "/home",
        pathMatch: "full"
    }
];
