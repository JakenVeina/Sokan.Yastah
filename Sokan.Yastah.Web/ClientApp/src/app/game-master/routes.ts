import { Routes } from "@angular/router";

import { GuildsPage } from "./guilds/guilds-page";
import { GuildCreationPage } from "./guilds/guild-creation-page";
import { GuildDivisionsPage } from "./guilds/guild-divisions-page";
import { GuildDivisionCreationPage } from "./guilds/guild-division-creation-page";
import { GuildDivisionUpdatePage } from "./guilds/guild-division-update-page";
import { GuildUpdatePage } from "./guilds/guild-update-page";
import { LevelsUpdatePage } from "./levels/levels-update-page";

import { GuildsGuard, LevelsGuard } from "./routes.guards";


export const gameMasterRouts: Routes = [
    {
        path: "game-master",
        children: [
            {
                path: "guilds",
                canActivate: [GuildsGuard],
                component: GuildsPage,
                children: [
                    {
                        path: "new",
                        component: GuildCreationPage
                    },
                    {
                        path: ":id",
                        component: GuildUpdatePage,
                        children: [
                            {
                                path: "divisions",
                                component: GuildDivisionsPage,
                                children: [
                                    {
                                        path: "new",
                                        component: GuildDivisionCreationPage
                                    },
                                    {
                                        path: ":id",
                                        component: GuildDivisionUpdatePage
                                    }
                                ]
                            }
                        ]
                    }
                ]
            },
            {
                path: "levels",
                canActivate: [LevelsGuard],
                component: LevelsUpdatePage
            }
        ]
    }
];
