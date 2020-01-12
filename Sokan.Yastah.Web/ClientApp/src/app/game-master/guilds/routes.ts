import { Routes } from "@angular/router";

import { GuildsPage } from "./guilds-page";
import { GuildCreationPage } from "./guild-creation-page";
import { GuildDivisionsPage } from "./guild-divisions-page";
import { GuildDivisionCreationPage } from "./guild-division-creation-page";
import { GuildDivisionUpdatePage } from "./guild-division-update-page";
import { GuildUpdatePage } from "./guild-update-page";

import { GuildsGuard } from "./routes.guards";


export const guildsRoutes: Routes = [
    {
        path: "characters",
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
            }
        ]
    }
];
