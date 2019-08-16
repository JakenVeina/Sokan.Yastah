import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { EffectsModule } from "@ngrx/effects";
import { StoreModule } from "@ngrx/store";

import { PermissionsEffects } from "./permissions/effects";
import { RoleCreationForm } from "./roles/role-creation-form";
import { RolesEffects } from "./roles/effects";
import { RoleUpdateForm } from "./roles/role-update-form";
import { RolesView } from "./roles/roles-view";

import { UserUpdateForm } from "./users/user-update-form";
import { UsersEffects } from "./users/effects";
import { UsersView } from "./users/users-view";

import { adminStateReducers } from "./reducers";
import { AdminRoutes } from "./routes";

@NgModule({
    imports: [
        CommonModule,
        EffectsModule.forFeature([
            PermissionsEffects,
            RolesEffects,
            UsersEffects
        ]),
        FormsModule,
        HttpClientModule,
        ReactiveFormsModule,
        RouterModule.forChild(AdminRoutes),
        StoreModule.forFeature("admin", adminStateReducers)
    ],
    declarations: [
        RoleCreationForm,
        RoleUpdateForm,
        RolesView,
        UserUpdateForm,
        UsersView
    ]
})
export class AdminModule { }
