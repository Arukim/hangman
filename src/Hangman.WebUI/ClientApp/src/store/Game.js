const requestGameInfo = 'REQUEST_GAME_INFO';
const responseGameInfo = 'RESPONSE_GAME_INFO';


export const Status = {
    Loading: 'LOADING',
    NotFound: 'NOT_FOUND',
    Init: 'Init'
};

const initialState = {
    status: Status.Loading
};

export const actionCreators = {
    requestGameInfo: id => async (dispatch, getState) => {
        dispatch({ type: requestGameInfo, id });

        const url = `api/hangman/game/${id}`;
        const response = await fetch(url);
        const gameInfo = await response.json();

        dispatch({ type: responseGameInfo, id, gameInfo });
    }
}


export const reducer = (state, action) => {
    state = state || initialState;


    if (action.type === requestGameInfo) {
        return {
            ...state,
            id: action.id,
            status: Status.Loading
        };
    }

    if (action.type === responseGameInfo) {
        return {
            ...state,
            id: action.id,
            gameInfo: action.gameInfo,
            status: Status.Init
        };
    }

    return state;
}