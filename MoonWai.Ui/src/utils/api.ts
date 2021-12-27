import { AppDispatch } from '../app/store';
import { setMessage } from '../slices/messageSlice';

export function onError(dispatch: AppDispatch, error: any) {
  const message =
      (error.response && error.response.data && error.response.data.message) ||
      error.message ||
      error.toString();
    
    dispatch(setMessage(message));
}
