import axios from 'axios';

import { createSlice, PayloadAction, ThunkDispatch } from '@reduxjs/toolkit';
import { RootState, AppThunk, AppDispatch } from '../app/store';
import { onError } from '../utils/api'

// @ts-ignore
const user = JSON.parse(localStorage.getItem('user'));

export class User {
  public username: string;
  public defaultBoardId: number;

  constructor(username: string, defaultBoardId: number) {
    this.username = username;
    this.defaultBoardId = defaultBoardId;
  }
}

export interface AuthState {
  user: User | null;
}

const initialState: AuthState = {
  user: user,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    authSuccess: (state, action: PayloadAction<User>) => {
      state.user = action.payload;
    },
    authFail: (state) => {
      state.user = null;
    },
    logout: (state) => {
      state.user = null;
    }
  },
});

export const { authSuccess, authFail, logout } = authSlice.actions;

export const selectUser = (state: RootState) => state.auth.user;

export const loginAsync = (username: string, password: string, trusted: boolean): AppThunk => async (dispatch) => {
  axios.post('api/auth/login', {
    username: username,
    password: password,
    trusted: trusted,
  })
  .then((response) => {
    localStorage.setItem('user', JSON.stringify(response.data));
    dispatch(authSuccess(response.data));
  })
  .catch((error) => onError(dispatch, error));
};

export const registerAsync = (username: string, password: string): AppThunk => async (dispatch) => {
  axios.post('api/auth/register', {
    username: username,
    password: password,
  })
  .then((response) => {
    localStorage.setItem('user', JSON.stringify(response.data));
    dispatch(authSuccess(response.data));
  })
  .catch((error) => onError(dispatch, error));
};

export const logoutAsync = (): AppThunk => async (dispatch) => {
  axios.post('api/auth/logout')
  .then((_response) => {
    localStorage.removeItem('user');;
    dispatch(logout());
  })
  .catch((error) => onError(dispatch, error));
};

export default authSlice.reducer;
