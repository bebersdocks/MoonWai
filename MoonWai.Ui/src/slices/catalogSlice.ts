import axios from 'axios';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { RootState, AppThunk } from '../app/store';

export interface IBoardSection {
  boardSectionId: number;
  name: string;
}

export interface IBoard {
  boardId: number;
  boardSection: IBoardSection;
  path: string;
  name: string;
}

export interface ICatalogState {
  boards: Array<IBoard>;
}

const initialState: ICatalogState = {
  boards: [],
};

export const selectBoards = (state: RootState) => state.catalog.boards;

export const catalogSlice = createSlice({
  name: 'catalog',
  initialState,
  reducers: {
    boardsRetrieved: (state, action: PayloadAction<Array<IBoard>>) => {
      state.boards = action.payload;
    },
  },
});

export const getBoardsAsync = (): AppThunk => async (dispatch) => {
  axios
    .get('api/boards')
    .then(response => dispatch(boardsRetrieved(response.data)));
};

export const { boardsRetrieved } = catalogSlice.actions;

export default catalogSlice.reducer;
