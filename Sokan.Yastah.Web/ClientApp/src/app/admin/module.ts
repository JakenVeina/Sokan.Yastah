import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

import { StoreModule } from "@ngrx/store";

import { RoleCreationForm } from "./roles/role-creation-form";
import { RoleCreationPage } from "./roles/role-creation-page";
import { RoleUpdateForm } from "./roles/role-update-form";
import { RoleUpdatePage } from "./roles/role-update-page";
import { RolesPage } from "./roles/roles-page";

import { UserUpdateForm } from "./users/user-update-form";
import { UserUpdatePage } from "./users/user-update-page";
import { UsersPage } from "./users/users-page";

import { adminStateReducers } from "./state.reducers";
import { adminRoutes } from "./routes";


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        ReactiveFormsModule,
        RouterModule.forChild(adminRoutes),
        StoreModule.forFeature("admin", adminStateReducers)
    ],
    declarations: [
        RolesPage,
        RoleCreationForm,
        RoleCreationPage,
        RoleUpdateForm,
        RoleUpdatePage,
        UsersPage,
        UserUpdateForm,
        UserUpdatePage
    ]
})
export class AdminModule { }
