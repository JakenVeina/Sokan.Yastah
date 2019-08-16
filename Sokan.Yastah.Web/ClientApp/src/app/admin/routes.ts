import { Routes } from "@angular/router";

import { RolesView } from "./roles/roles-view";
import { RoleCreationForm } from "./roles/role-creation-form";
import { RoleUpdateForm } from "./roles/role-update-form";

import { UsersView } from "./users/users-view";

import { AdminGuard, RolesGuard, UsersGuard } from "./guards";

export const AdminRoutes: Routes = [
    {
        path: "admin",
        canActivate: [AdminGuard],
        children: [
            {
                path: "roles",
                canActivate: [RolesGuard],
                component: RolesView,
                children: [
                    {
                        path: "new",
                        component: RoleCreationForm
                    },
                    {
                        path: ":id",
                        component: RoleUpdateForm
                    }
                ]
            },
            {
                path: "users",
                canActivate: [UsersGuard],
                component: UsersView
            }
        ]
    }
];
