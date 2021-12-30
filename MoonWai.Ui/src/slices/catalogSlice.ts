import axios from 'axios';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { RootState, AppThunk } from '../app/store';

export class BoardSection {
  public boardSectionId: number;
  public name: string;
}

export class Board {
  public boardId: number;
  public boardSection: BoardSection;
  public path: string;
  public name: string;
}

export interface ICatalogState {
  boards: Array<Board>;
}

const initialState: ICatalogState = {
  boards: [],
};

export const selectBoards = (state: RootState) => state.catalog.boards;

export const catalogSlice = createSlice({
  name: 'catalog',
  initialState,
  reducers: {
    boardsRetrieved: (state, action: PayloadAction<Array<Board>>) => {
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
