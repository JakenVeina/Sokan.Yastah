import { Routes } from "@angular/router";

import { HomePage } from "./home/home-page";

import { PortalGuard } from "./routes.guards";


export const AppRoutes: Routes = [
    {
        path: "home",
        canActivate: [PortalGuard],
        component: HomePage
    },
    {
        path: "",
        redirectTo: "/home",
        pathMatch: "full"
    }
];
