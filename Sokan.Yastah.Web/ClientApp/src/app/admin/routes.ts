import { Routes } from "@angular/router";

import { RolesPage } from "./roles/roles-page";
import { RoleCreationPage } from "./roles/role-creation-page";
import { RoleUpdatePage } from "./roles/role-update-page";

import { UsersPage } from "./users/users-page";
import { UserUpdatePage } from "./users/user-update-page";

import { AdminGuard, RolesGuard, UsersGuard } from "./routes.guards";


export const adminRoutes: Routes = [
    {
        path: "admin",
        canActivate: [AdminGuard],
        children: [
            {
                path: "roles",
                canActivate: [RolesGuard],
                component: RolesPage,
                children: [
                    {
                        path: "new",
                        component: RoleCreationPage
                    },
                    {
                        path: ":id",
                        component: RoleUpdatePage
                    }
                ]
            },
            {
                path: "users",
                canActivate: [UsersGuard],
                component: UsersPage,
                children: [
                    {
                        path: ":id",
                        component: UserUpdatePage
                    }
                ]
            }
        ]
    }
];
