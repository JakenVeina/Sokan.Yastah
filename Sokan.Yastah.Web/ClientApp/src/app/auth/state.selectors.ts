import { createFeatureSelector, createSelector, MemoizedSelector } from "@ngrx/store";

import { IAppState } from "../state";

import { IAuthenticationTicket } from "./models";
import { IAuthenticationState } from "./state";


const authenticationState
    = createFeatureSelector<IAppState, IAuthenticationState>("authentication");


export namespace AuthenticationSelectors {

    const currentTicket: MemoizedSelector<IAppState, IAuthenticationTicket>
        = createSelector(
            authenticationState,
            state => state.currentTicket);


    export const avatarUri: MemoizedSelector<IAppState, string | null>
        = createSelector(
            currentTicket,
            ticket => (ticket == null)
                ? null
                : (ticket.avatarHash == null)
                    ? `https://cdn.discordapp.com/embed/avatars/0.png`
                    : `https://cdn.discordapp.com/avatars/${ticket.userId}/${ticket.avatarHash}.png?size=32`);

    export const hasAdmin: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && Object.values(ticket.grantedPermissions)
                    .some(x => x.startsWith("Administration.")));

    export const hasAdminManagePermissions: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && Object.values(ticket.grantedPermissions)
                    .some(x => x === "Administration.ManagePermissions"));

    export const hasAdminManageRoles: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && Object.values(ticket.grantedPermissions)
                    .some(x => x === "Administration.ManageRoles"));

    export const hasAdminManageUsers: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && Object.values(ticket.grantedPermissions)
                    .some(x => x === "Administration.ManageUsers"));

    export const hasAnyPermissions: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && (Object.keys(ticket.grantedPermissions).length > 0));

    export const hasCharacterAdminManageGuilds: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null)
                && Object.values(ticket.grantedPermissions)
                    .some(x => x === "CharacterAdministration.ManageGuilds"));

    export const isAuthenticated: MemoizedSelector<IAppState, boolean>
        = createSelector(
            currentTicket,
            ticket => (ticket != null));

    export const username: MemoizedSelector<IAppState, string | null>
        = createSelector(
            currentTicket,
            ticket => ticket && ticket.username);
}
