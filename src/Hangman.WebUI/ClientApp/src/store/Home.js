import { push } from 'react-router-redux'

const initialState = { isLoading: false };

const requestCreateGameType = 'REQUEST_CREATE_GAME';

export const actionCreators = {
    createGame: () => async (dispatch, getState) => {
        dispatch({ type: requestCreateGameType });

        const url = `api/Hangman/Game`;

        const response = await fetch(url, { method: "POST" });
        const data = await response.json();

        dispatch(push(`game/${data.id}`));
    }
};


export const reducer = (state, action) => {
    state = state || initialState;

    if (action === requestCreateGameType) {
        return {
            ...state,
            isLoading: true
        };
    }
    return state;
}