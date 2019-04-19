import { push } from 'react-router-redux'

const initialState = { isLoading: false };

const requestCreateGameType = 'REQUEST_CREATE_GAME';

export const actionCreators = {
    createGameEng: () => async (dispatch) => {
        await createGame(dispatch, 0);
    },
    createGameRus: () => async (dispatch) => {
        await createGame(dispatch, 1);
    }
};

const createGame = async (dispatch, lang) => {

    dispatch({ type: requestCreateGameType });

    const url = `api/Hangman/Game/${lang}`;

    const response = await fetch(url, { method: "POST" });
    const data = await response.json();

    dispatch(push(`game/${data.id}`));
}


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