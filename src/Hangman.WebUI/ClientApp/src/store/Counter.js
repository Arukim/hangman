const incrementCountType = 'INCREMENT_COUNT';
const decrementCountType = 'DECREMENT_COUNT';
const signalRIncrementCountType = 'SIGNALR_INCREMENT_COUNT';
const signalRDecrementCountType = 'SIGNALR_DECREMENT_COUNT';
const initialState = { count: 0 };

export const actionCreators = {
    increment: () => (dispatch, getState) => {
        dispatch({ type: incrementCountType });
        dispatch({ type: signalRIncrementCountType });
    },
    decrement: () => (dispatch, getState) => {
        dispatch({ type: decrementCountType });
        dispatch({ type: signalRDecrementCountType });
    }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === incrementCountType) {
    return { ...state, count: state.count + 1 };
  }

  if (action.type === decrementCountType) {
    return { ...state, count: state.count - 1 };
  }

  return state;
};
