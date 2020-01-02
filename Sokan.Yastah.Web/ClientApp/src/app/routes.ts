import { Routes } from "@angular/router";

import { HomeView } from "./home/home-view";

import { PortalGuard } from "./guards";


export const AppRoutes: Routes = [
    {
        path: "home",
        canActivate: [PortalGuard],
        component: HomeView
    },
    {
        path: "",
        redirectTo: "/home",
        pathMatch: "full"
    }
];
