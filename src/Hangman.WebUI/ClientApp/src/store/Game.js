const requestGameInfo = 'REQUEST_GAME_INFO';
const madeTurn = "MADE_TURN";


export const gameState = 'QUERY_STATE';
export const signalRGuess = 'COMMAND_GUESS';
export const signalRSubscribe = 'COMMAND_SUBSCRIBE';

const initialState = {
};

export const actionCreators = {
    requestGameInfo: id => async (dispatch, getState) => {
        dispatch({ type: requestGameInfo, id });

        const url = `api/hangman/game/${id}`;
        const response = await fetch(url);
        const gameInfo = await response.json();

        dispatch({ type: gameState, ...gameInfo });
        dispatch({ type: signalRSubscribe, id: gameInfo.correlationId });
    },
    onGuessClick: (id, guess) => async (dispatch, getState) => {
        dispatch({ type: signalRGuess, id, guess });

        dispatch({ type: madeTurn });
    }
}


export const reducer = (state, action) => {
    state = state || initialState;


    if (action.type === requestGameInfo) {
        return {
            ...state,
            id: action.id,
            isLoading: true
        };
    }

    if (action.type === gameState) {
        return {
            ...state,
            ...action,
            isLoading: false
        };
    }

    if (action.type === madeTurn) {
        return {
            ...state
        };
    }


    return state;
}