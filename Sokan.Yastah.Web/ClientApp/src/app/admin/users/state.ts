import { IUserOverviewViewModel } from "./models";

export interface IUsersState {
    overviews: IUserOverviewViewModel[];
}

export const initialUsersState: IUsersState = {
    overviews: [],
};
