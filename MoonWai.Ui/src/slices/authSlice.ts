import axios from 'axios';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { RootState, AppThunk } from '../app/store';

export class User {
  public username: string;
  public defaultBoardId: number;

  constructor(username: string, defaultBoardId: number) {
    this.username = username;
    this.defaultBoardId = defaultBoardId;
  }
}

export interface IAuthState {
  user: User | null;
}

const initialState: IAuthState = {
  // @ts-ignore
  user: JSON.parse(localStorage.getItem('user')),
};

export const selectUser = (state: RootState) => state.auth.user;

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    authSuccess: (state, action: PayloadAction<User>) => {
      state.user = action.payload;
    },
    logout: (state) => {
      state.user = null;
    },
  },
});

export const logoutAsync = (): AppThunk => async (dispatch) => {
  axios.post('api/auth/logout')
    .then(_ => {
      localStorage.removeItem('user');;
      dispatch(logout);
    })
    .catch(err => err);
};

export const { authSuccess, logout } = authSlice.actions;

export default authSlice.reducer;
